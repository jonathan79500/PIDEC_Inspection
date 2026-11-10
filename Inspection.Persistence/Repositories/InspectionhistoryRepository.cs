using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification;   // Revision: adjust to your namespace
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Repositories;

public class InspectionHistoryRepository : IInspectionHistoryRepository
{
    private readonly InspectionDbContext _db;

    public InspectionHistoryRepository(InspectionDbContext db) => _db = db;

    public Task<List<Revision>> GetRevisionsByRequestIdAsync(Guid requestId, CancellationToken ct) =>
        _db.Revisions
            .AsNoTracking()
            .Where(r => r.RequestId == requestId)
            .Include(r => r.SubmittedBy)
            .Include(r => r.Circulations).ThenInclude(c => c.PerformerContactPerson)
            .OrderBy(r => r.RevisionNumber)
            .ToListAsync(ct);
}