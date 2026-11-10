using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly InspectionDbContext _db;

    public DocumentRepository(InspectionDbContext db) => _db = db;

    // ---------- document types ----------
    public Task<List<DocumentType>> GetActiveTypesAsync(int? contactTypeCode, CancellationToken ct)
    {
        var query = _db.DocumentTypes.AsNoTracking().Where(t => t.IsActive);

        if (contactTypeCode is int code)
        {
            query = query.Where(t => _db.DocumentTypeVisibilities
                .Any(v => v.DocumentTypeId == t.DocumentTypeId && v.ContactTypeCode == code));
        }

        return query.OrderBy(t => t.DocumentTypeName).ToListAsync(ct);
    }

    public Task<DocumentType?> GetTypeAsync(Guid documentTypeId, CancellationToken ct) =>
        _db.DocumentTypes.AsNoTracking().FirstOrDefaultAsync(t => t.DocumentTypeId == documentTypeId, ct);

    public Task<bool> IsTypeVisibleAsync(Guid documentTypeId, int contactTypeCode, CancellationToken ct) =>
        _db.DocumentTypeVisibilities.AnyAsync(
            v => v.DocumentTypeId == documentTypeId && v.ContactTypeCode == contactTypeCode, ct);

    // ---------- documents ----------
    public async Task<List<DocumentWithType>> GetByOwnerAsync(DocumentOwnerType ownerType, Guid ownerId, CancellationToken ct)
    {
        var rows = await (
            from l in _db.DocumentLinks.AsNoTracking()
            where l.OwnerType == (int)ownerType && l.OwnerId == ownerId
            join d in _db.Documents.AsNoTracking() on l.DocumentId equals d.DocumentId
            join t in _db.DocumentTypes.AsNoTracking() on d.DocumentTypeId equals t.DocumentTypeId
            select new { Doc = d, TypeName = t.DocumentTypeName }
        )
        .OrderBy(r => r.TypeName).ThenBy(r => r.Doc.IssueDate)
        .ToListAsync(ct);

        return rows.Select(r => new DocumentWithType(r.Doc, r.TypeName)).ToList();
    }

    public Task<bool> OwnerHasDocumentOfTypeAsync(
        DocumentOwnerType ownerType, Guid ownerId, Guid documentTypeId, CancellationToken ct) =>
        (
            from l in _db.DocumentLinks
            join d in _db.Documents on l.DocumentId equals d.DocumentId
            where l.OwnerType == (int)ownerType && l.OwnerId == ownerId && d.DocumentTypeId == documentTypeId
            select l.DocumentId
        ).AnyAsync(ct);

    public Task<Document?> GetForOwnerAsync(Guid documentId, DocumentOwnerType ownerType, Guid ownerId, CancellationToken ct) =>
        (
            from l in _db.DocumentLinks.AsNoTracking()
            where l.DocumentId == documentId && l.OwnerType == (int)ownerType && l.OwnerId == ownerId
            join d in _db.Documents.AsNoTracking() on l.DocumentId equals d.DocumentId
            select d
        ).FirstOrDefaultAsync(ct);

    public async Task AddAsync(Document document, DocumentLink link, CancellationToken ct)
    {
        _db.Documents.Add(document);
        _db.DocumentLinks.Add(link);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<UnlinkResult> UnlinkAsync(Guid documentId, DocumentOwnerType ownerType, Guid ownerId, CancellationToken ct)
    {
        var link = await _db.DocumentLinks.FirstOrDefaultAsync(
            l => l.DocumentId == documentId && l.OwnerType == (int)ownerType && l.OwnerId == ownerId, ct);
        if (link is null) return new UnlinkResult(false, null);

        _db.DocumentLinks.Remove(link);

        var usedElsewhere = await _db.DocumentLinks.AnyAsync(
            l => l.DocumentId == documentId && !(l.OwnerType == (int)ownerType && l.OwnerId == ownerId), ct);

        Document? orphan = null;
        if (!usedElsewhere)
        {
            orphan = await _db.Documents.FirstOrDefaultAsync(d => d.DocumentId == documentId, ct);
            if (orphan is not null) _db.Documents.Remove(orphan);
        }

        await _db.SaveChangesAsync(ct);
        return new UnlinkResult(true, orphan);
    }

    public async Task<List<Document>> StageRemoveAllForOwnerAsync(DocumentOwnerType ownerType, Guid ownerId, CancellationToken ct)
    {
        var links = await _db.DocumentLinks
            .Where(l => l.OwnerType == (int)ownerType && l.OwnerId == ownerId)
            .ToListAsync(ct);
        if (links.Count == 0) return new List<Document>();

        var ids = links.Select(l => l.DocumentId).ToList();
        _db.DocumentLinks.RemoveRange(links);

        var sharedIds = await _db.DocumentLinks
            .Where(l => ids.Contains(l.DocumentId) && !(l.OwnerType == (int)ownerType && l.OwnerId == ownerId))
            .Select(l => l.DocumentId)
            .Distinct()
            .ToListAsync(ct);

        var orphanIds = ids.Except(sharedIds).ToList();
        var orphans = await _db.Documents.Where(d => orphanIds.Contains(d.DocumentId)).ToListAsync(ct);
        _db.Documents.RemoveRange(orphans);
        return orphans;
    }
}