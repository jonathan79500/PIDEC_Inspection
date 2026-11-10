using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.Exceptions;
using Inspection.Domain.Entities.Common;   // DocumentType: adjust to your namespace
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Documents;

// ===================== requests =====================
public record GetDocumentTypesAdminQuery : IRequest<List<DocumentTypeAdminDto>>;
public record GetDocumentTypeByIdQuery(Guid DocumentTypeId) : IRequest<DocumentTypeAdminDto>;
public record CreateDocumentTypeCommand(DocumentTypeSaveDto Data) : IRequest<DocumentTypeAdminDto>;
public record UpdateDocumentTypeCommand(Guid DocumentTypeId, DocumentTypeSaveDto Data) : IRequest<DocumentTypeAdminDto>;
public record DeleteDocumentTypeCommand(Guid DocumentTypeId) : IRequest;

// ===================== handlers =====================
public class GetDocumentTypesAdminHandler : IRequestHandler<GetDocumentTypesAdminQuery, List<DocumentTypeAdminDto>>
{
    private readonly IDocumentTypeRepository _repository;
    public GetDocumentTypesAdminHandler(IDocumentTypeRepository repository) => _repository = repository;

    public async Task<List<DocumentTypeAdminDto>> Handle(GetDocumentTypesAdminQuery request, CancellationToken ct)
    {
        var types = await _repository.GetAllAsync(ct);
        var dtos = types.Adapt<List<DocumentTypeAdminDto>>();

        // the catalogue of document types is small, so one pair of lookups per row is fine
        foreach (var dto in dtos)
        {
            dto.VisibleToContactTypes = await _repository.GetVisibleContactTypesAsync(dto.DocumentTypeId, ct);
            dto.DocumentCount = await _repository.CountDocumentsUsingAsync(dto.DocumentTypeId, ct);
        }

        return dtos;
    }
}

public class GetDocumentTypeByIdHandler : IRequestHandler<GetDocumentTypeByIdQuery, DocumentTypeAdminDto>
{
    private readonly IDocumentTypeRepository _repository;
    public GetDocumentTypeByIdHandler(IDocumentTypeRepository repository) => _repository = repository;

    public async Task<DocumentTypeAdminDto> Handle(GetDocumentTypeByIdQuery request, CancellationToken ct)
    {
        var type = await _repository.GetByIdAsync(request.DocumentTypeId, ct)
                   ?? throw new NotFoundException("Document type not found.");

        var dto = type.Adapt<DocumentTypeAdminDto>();
        dto.VisibleToContactTypes = await _repository.GetVisibleContactTypesAsync(type.DocumentTypeId, ct);
        dto.DocumentCount = await _repository.CountDocumentsUsingAsync(type.DocumentTypeId, ct);
        return dto;
    }
}

public class CreateDocumentTypeHandler : IRequestHandler<CreateDocumentTypeCommand, DocumentTypeAdminDto>
{
    private readonly IDocumentTypeRepository _repository;
    public CreateDocumentTypeHandler(IDocumentTypeRepository repository) => _repository = repository;

    public async Task<DocumentTypeAdminDto> Handle(CreateDocumentTypeCommand request, CancellationToken ct)
    {
        var dto = request.Data;

        if (await _repository.CodeExistsAsync(dto.DocumentTypeCode, excludingTypeId: null, ct))
            throw new BusinessRuleException($"A document type with code {dto.DocumentTypeCode} already exists.");

        var type = new DocumentType { DocumentTypeId = Guid.NewGuid() };
        Apply(type, dto);

        await _repository.AddAsync(type, ct);
        _repository.ReplaceVisibility(type.DocumentTypeId, dto.VisibleToContactTypes.Distinct().ToList());
        await _repository.SaveAsync(ct);

        var result = type.Adapt<DocumentTypeAdminDto>();
        result.VisibleToContactTypes = await _repository.GetVisibleContactTypesAsync(type.DocumentTypeId, ct);
        return result;
    }

    internal static void Apply(DocumentType type, DocumentTypeSaveDto dto)
    {
        type.DocumentTypeName = dto.DocumentTypeName.Trim();
        type.DocumentTypeCode = dto.DocumentTypeCode;
        type.RequiresApproval = dto.RequiresApproval;
        type.RequiresReview = dto.RequiresReview;
        type.RequiresName = dto.RequiresName;
        type.RequiresNumber = dto.RequiresNumber;
        type.MultipleAllowed = dto.MultipleAllowed;
        type.IsActive = dto.IsActive;
        type.AllowedExtensions = string.IsNullOrWhiteSpace(dto.AllowedExtensions) ? "*" : dto.AllowedExtensions.Trim();
        type.MaxSizeMB = dto.MaxSizeMB;
    }
}

public class UpdateDocumentTypeHandler : IRequestHandler<UpdateDocumentTypeCommand, DocumentTypeAdminDto>
{
    private readonly IDocumentTypeRepository _repository;
    public UpdateDocumentTypeHandler(IDocumentTypeRepository repository) => _repository = repository;

    public async Task<DocumentTypeAdminDto> Handle(UpdateDocumentTypeCommand request, CancellationToken ct)
    {
        var dto = request.Data;

        var type = await _repository.GetByIdAsync(request.DocumentTypeId, ct)
                   ?? throw new NotFoundException("Document type not found.");

        if (await _repository.CodeExistsAsync(dto.DocumentTypeCode, excludingTypeId: type.DocumentTypeId, ct))
            throw new BusinessRuleException($"A document type with code {dto.DocumentTypeCode} already exists.");

        CreateDocumentTypeHandler.Apply(type, dto);
        _repository.ReplaceVisibility(type.DocumentTypeId, dto.VisibleToContactTypes.Distinct().ToList());
        await _repository.SaveAsync(ct);

        var result = type.Adapt<DocumentTypeAdminDto>();
        result.VisibleToContactTypes = await _repository.GetVisibleContactTypesAsync(type.DocumentTypeId, ct);
        result.DocumentCount = await _repository.CountDocumentsUsingAsync(type.DocumentTypeId, ct);
        return result;
    }
}

// Deleting a type that documents already use would strand those documents (and, depending on your FK,
// would be rejected by the database anyway), so it is blocked here with a clear message instead.
// Deactivating (IsActive = false) is always safe: existing documents keep working, new uploads are refused.
public class DeleteDocumentTypeHandler : IRequestHandler<DeleteDocumentTypeCommand>
{
    private readonly IDocumentTypeRepository _repository;
    public DeleteDocumentTypeHandler(IDocumentTypeRepository repository) => _repository = repository;

    public async Task Handle(DeleteDocumentTypeCommand request, CancellationToken ct)
    {
        var type = await _repository.GetByIdAsync(request.DocumentTypeId, ct)
                   ?? throw new NotFoundException("Document type not found.");

        var inUse = await _repository.CountDocumentsUsingAsync(type.DocumentTypeId, ct);
        if (inUse > 0)
            throw new BusinessRuleException(
                $"This document type is used by {inUse} document(s) and cannot be deleted. Deactivate it instead.");

        await _repository.DeleteAsync(type, ct);
    }
}