using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;   // DocumentType, DocumentTypeVisibility: adjust to your namespace

namespace Inspection.Application.Contracts.Persistence;

// Admin / configuration side of document types: full CRUD and which contact types may use each type.
// The consumer side (active types visible to one contact type, used when attaching a document) stays
// in IDocumentRepository, since it belongs to the upload use case, not to managing the catalogue.
public interface IDocumentTypeRepository
{
    Task<List<DocumentType>> GetAllAsync(CancellationToken ct);
    Task<DocumentType?> GetByIdAsync(Guid documentTypeId, CancellationToken ct);   // tracked (for update)

    Task<bool> CodeExistsAsync(int code, Guid? excludingTypeId, CancellationToken ct);
    Task<int> CountDocumentsUsingAsync(Guid documentTypeId, CancellationToken ct);

    Task<List<int>> GetVisibleContactTypesAsync(Guid documentTypeId, CancellationToken ct);
    // Replaces the DocumentTypeVisibility rows for this type with exactly this set. Does not save.
    void ReplaceVisibility(Guid documentTypeId, IReadOnlyCollection<int> contactTypeCodes);

    Task AddAsync(DocumentType type, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
    Task DeleteAsync(DocumentType type, CancellationToken ct);
}