using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;   // IDocumentRepository, IAttachmentStore, IDocumentOwnerResolver (adjust if they live elsewhere)
using Inspection.Application.DTOs;
using Inspection.Application.Exceptions;              // adjust: NotFoundException / BusinessRuleException
using Inspection.Application.Features.BasicInfo.Documents;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Documents;

// ===================== queries (read) =====================
public record GetDocumentTypesQuery(int? ContactTypeCode) : IRequest<List<DocumentTypeDto>>;
public record GetDocumentsQuery(DocumentOwnerType OwnerType, Guid OwnerId) : IRequest<List<DocumentDto>>;
public record DownloadDocumentQuery(Guid DocumentId, DocumentOwnerType OwnerType, Guid OwnerId) : IRequest<DocumentFile>;

// ===================== commands (write) =====================
public record UploadDocumentCommand(UploadDocumentRequest Data) : IRequest<DocumentDto>;
public record DeleteDocumentCommand(Guid DocumentId, DocumentOwnerType OwnerType, Guid OwnerId) : IRequest;

public record DocumentFile(Stream Content, string ContentType, string FileName);

// ===================== handlers =====================
public class GetDocumentTypesHandler : IRequestHandler<GetDocumentTypesQuery, List<DocumentTypeDto>>
{
    private readonly IDocumentRepository _repository;
    public GetDocumentTypesHandler(IDocumentRepository repository) => _repository = repository;

    public async Task<List<DocumentTypeDto>> Handle(GetDocumentTypesQuery request, CancellationToken ct)
    {
        var types = await _repository.GetActiveTypesAsync(request.ContactTypeCode, ct);
        List<DocumentTypeDto> documentTypesDtos = types.Adapt<List<DocumentTypeDto>>();
        return documentTypesDtos;
    }
}

public class GetDocumentsHandler : IRequestHandler<GetDocumentsQuery, List<DocumentDto>>
{
    private readonly IDocumentRepository _repository;
    private readonly IAttachmentStore _store;

    public GetDocumentsHandler(IDocumentRepository repository, IAttachmentStore store)
    {
        _repository = repository;
        _store = store;
    }

    public async Task<List<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken ct)
    {
        var rows = await _repository.GetByOwnerAsync(request.OwnerType, request.OwnerId, ct);

        var result = new List<DocumentDto>();
        foreach (var row in rows)
        {
            var info = await _store.GetInfoAsync(row.Document, ct);
            result.Add(new DocumentView(row.Document, row.DocumentTypeName, info).Adapt<DocumentDto>());
        }

        return result;
    }
}

public class DownloadDocumentHandler : IRequestHandler<DownloadDocumentQuery, DocumentFile>
{
    private readonly IDocumentRepository _repository;
    private readonly IAttachmentStore _store;

    public DownloadDocumentHandler(IDocumentRepository repository, IAttachmentStore store)
    {
        _repository = repository;
        _store = store;
    }

    public async Task<DocumentFile> Handle(DownloadDocumentQuery request, CancellationToken ct)
    {
        var document = await _repository.GetForOwnerAsync(request.DocumentId, request.OwnerType, request.OwnerId, ct)
                       ?? throw new NotFoundException("Document not found.");

        var opened = await _store.OpenAsync(document, ct)
                     ?? throw new NotFoundException("The file is missing.");

        return new DocumentFile(opened.Content, opened.Info.ContentType, opened.Info.FileName);
    }
}

// The use case: check everything, store the file, then save the database rows.
public class UploadDocumentHandler : IRequestHandler<UploadDocumentCommand, DocumentDto>
{
    private readonly IDocumentRepository _repository;
    private readonly IAttachmentStore _store;
    private readonly IDocumentOwnerResolver _owners;

    public UploadDocumentHandler(IDocumentRepository repository, IAttachmentStore store, IDocumentOwnerResolver owners)
    {
        _repository = repository;
        _store = store;
        _owners = owners;
    }

    public async Task<DocumentDto> Handle(UploadDocumentCommand command, CancellationToken ct)
    {
        var req = command.Data;

        // 1. the owner must exist; its contact type decides which document types it may use
        var (exists, contactTypeCode) = await _owners.ResolveAsync(req.OwnerType, req.OwnerId, ct);
        if (!exists) throw new NotFoundException("The owner was not found.");

        // 2. the document type must exist, be active and be visible for that contact type
        var type = await _repository.GetTypeAsync(req.DocumentTypeId, ct);
        if (type is null || !type.IsActive)
            throw new BusinessRuleException("Unknown or inactive document type.");

        if (contactTypeCode is int code && !await _repository.IsTypeVisibleAsync(type.DocumentTypeId, code, ct))
            throw new BusinessRuleException("This document type is not available here.");

        // 3. name / number / dates / file rules (pure logic)
        var name = req.Name?.Trim();
        var number = req.Number?.Trim();
        DocumentRules.ValidateUpload(type, name, number, req);

        // 4. only one document of this type per owner?
        if (!type.MultipleAllowed &&
            await _repository.OwnerHasDocumentOfTypeAsync(req.OwnerType, req.OwnerId, type.DocumentTypeId, ct))
        {
            throw new BusinessRuleException(
                $"Only one \"{type.DocumentTypeName}\" document is allowed. Delete the existing one first.");
        }

        // 5. store the file, then the rows
        var attachmentId = await _store.SaveAsync(req.File, ct);

        var document = new Document
        {
            DocumentId = Guid.NewGuid(),
            AttachmentId = attachmentId,
            DocumentTypeId = type.DocumentTypeId,
            Name = name,
            Number = number,
            Status = DocumentRules.InitialStatus(type),
            IssueDate = DocumentRules.AsUtcDate(req.IssueDate),
            ExpireDate = DocumentRules.AsUtcDate(req.ExpireDate),
            Version = "1"
        };
        var link = new DocumentLink
        {
            DocumentId = document.DocumentId,
            OwnerType = (int)req.OwnerType,
            OwnerId = req.OwnerId
        };

        try
        {
            await _repository.AddAsync(document, link, ct);
        }
        catch
        {
            await _store.DeleteAsync(document, ct);   // do not leave an orphan file behind
            throw;
        }

        var info = await _store.GetInfoAsync(document, ct);
        return new DocumentView(document, type.DocumentTypeName, info).Adapt<DocumentDto>();
    }
}

// Removes the owner's link. The document and its file are deleted when no other owner uses it.
public class DeleteDocumentHandler : IRequestHandler<DeleteDocumentCommand>
{
    private readonly IDocumentRepository _repository;
    private readonly IAttachmentStore _store;

    public DeleteDocumentHandler(IDocumentRepository repository, IAttachmentStore store)
    {
        _repository = repository;
        _store = store;
    }

    public async Task Handle(DeleteDocumentCommand request, CancellationToken ct)
    {
        var result = await _repository.UnlinkAsync(request.DocumentId, request.OwnerType, request.OwnerId, ct);
        if (!result.LinkFound) throw new NotFoundException("Document not found.");

        // Remove the file only after the database change succeeded
        if (result.OrphanedDocument is not null)
            await _store.DeleteAsync(result.OrphanedDocument, ct);
    }
}