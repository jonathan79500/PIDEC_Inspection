using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Persistence;

public record DocumentWithType(Document Document, string DocumentTypeName);

// LinkFound = false -> the owner had no such document.
// OrphanedDocument is set when the document was used by no other owner and was deleted too
// (the caller then deletes its file).
public record UnlinkResult(bool LinkFound, Document? OrphanedDocument);

// Data access only. No business rules here: those live in the Application layer (see DocumentRules).
public interface IDocumentRepository
{
    // ----- document types -----
    // Active types; when a contact type is given, only those that have a DocumentTypeVisibility row for it
    Task<List<DocumentType>> GetActiveTypesAsync(int? contactTypeCode, CancellationToken ct);
    Task<DocumentType?> GetTypeAsync(Guid documentTypeId, CancellationToken ct);
    Task<bool> IsTypeVisibleAsync(Guid documentTypeId, int contactTypeCode, CancellationToken ct);

    // ----- documents -----
    Task<List<DocumentWithType>> GetByOwnerAsync(DocumentOwnerType ownerType, Guid ownerId, CancellationToken ct);
    Task<bool> OwnerHasDocumentOfTypeAsync(DocumentOwnerType ownerType, Guid ownerId, Guid documentTypeId, CancellationToken ct);

    // read-only; null when the document is not linked to this owner
    Task<Document?> GetForOwnerAsync(Guid documentId, DocumentOwnerType ownerType, Guid ownerId, CancellationToken ct);

    // saves the document and its owner link together
    Task AddAsync(Document document, DocumentLink link, CancellationToken ct);

    // removes the owner link; also removes the document itself when no other owner uses it. Saves.
    Task<UnlinkResult> UnlinkAsync(Guid documentId, DocumentOwnerType ownerType, Guid ownerId, CancellationToken ct);

    // For deleting an owner (e.g. an inspector) together with its documents: stages the removals on the
    // current unit of work (nothing saved yet) and returns the documents whose files must be deleted after the save.
    Task<List<Document>> StageRemoveAllForOwnerAsync(DocumentOwnerType ownerType, Guid ownerId, CancellationToken ct);
}