using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Persistence.Repositories;

public class DocumentOwnerResolver : IDocumentOwnerResolver
{
    private readonly InspectionDbContext _db;

    public DocumentOwnerResolver(InspectionDbContext db) => _db = db;

    public async Task<(bool Exists, int? ContactTypeCode)> ResolveAsync(
        DocumentOwnerType type, Guid ownerId, CancellationToken ct)
    {
        switch (type)
        {
            case DocumentOwnerType.Inspector:
                // ASSUMPTION: your Contact entity has an int ContactTypeCode. Rename if it differs.
                var row = await _db.Inspectors.AsNoTracking()
                    .Where(i => i.InspectorId == ownerId)
                    .Select(i => new { i.InspectionCompanyContact.ContactType })
                    .FirstOrDefaultAsync(ct);
                return row is null ? (false, null) : (true, row.ContactType);

            default:
                throw new NotSupportedException($"Owner type {type} is not supported yet.");
        }
    }
}