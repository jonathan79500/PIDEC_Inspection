using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Common;   // DocumentType, DocumentTypeVisibility, Document: adjust to your namespace
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly InspectionDbContext _db;

    public DocumentTypeRepository(InspectionDbContext db) => _db = db;

    public Task<List<DocumentType>> GetAllAsync(CancellationToken ct) =>
        _db.DocumentTypes.AsNoTracking().OrderBy(t => t.DocumentTypeName).ToListAsync(ct);

    public Task<DocumentType?> GetByIdAsync(Guid documentTypeId, CancellationToken ct) =>
        _db.DocumentTypes.FirstOrDefaultAsync(t => t.DocumentTypeId == documentTypeId, ct);   // tracked: Update reuses this

    public Task<bool> CodeExistsAsync(int code, Guid? excludingTypeId, CancellationToken ct)
    {
        var query = _db.DocumentTypes.AsNoTracking().Where(t => t.DocumentTypeCode == code);
        if (excludingTypeId is Guid id) query = query.Where(t => t.DocumentTypeId != id);
        return query.AnyAsync(ct);
    }

    public Task<int> CountDocumentsUsingAsync(Guid documentTypeId, CancellationToken ct) =>
        _db.Documents.CountAsync(d => d.DocumentTypeId == documentTypeId, ct);

    public Task<List<int>> GetVisibleContactTypesAsync(Guid documentTypeId, CancellationToken ct) =>
        _db.DocumentTypeVisibilities
            .AsNoTracking()
            .Where(v => v.DocumentTypeId == documentTypeId)
            .OrderBy(v => v.ContactTypeCode)
            .Select(v => v.ContactTypeCode)
            .ToListAsync(ct);

    public void ReplaceVisibility(Guid documentTypeId, IReadOnlyCollection<int> contactTypeCodes)
    {
        var existing = _db.DocumentTypeVisibilities.Where(v => v.DocumentTypeId == documentTypeId);
        _db.DocumentTypeVisibilities.RemoveRange(existing);

        foreach (var code in contactTypeCodes)
            _db.DocumentTypeVisibilities.Add(new DocumentTypeVisibility
            {
                DocumentTypeId = documentTypeId,
                ContactTypeCode = code
            });
    }

    public async Task AddAsync(DocumentType type, CancellationToken ct)
    {
        _db.DocumentTypes.Add(type);
        await _db.SaveChangesAsync(ct);
    }

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public async Task DeleteAsync(DocumentType type, CancellationToken ct)
    {
        var visibility = _db.DocumentTypeVisibilities.Where(v => v.DocumentTypeId == type.DocumentTypeId);
        _db.DocumentTypeVisibilities.RemoveRange(visibility);
        _db.DocumentTypes.Remove(type);
        await _db.SaveChangesAsync(ct);
    }
}