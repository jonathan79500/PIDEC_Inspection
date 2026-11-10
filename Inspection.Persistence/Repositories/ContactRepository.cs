using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Contacts;    // Contact: adjust to your namespace
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly InspectionDbContext _db;

    public ContactRepository(InspectionDbContext db) => _db = db;

    public Task<List<Contact>> GetByProject_TypesAsync(Guid projectId, IReadOnlyCollection<int>? contactTypes, CancellationToken ct)
    {
        // EXISTS instead of a join: ProjectContacts has no unique (ProjectId, ContactId), so a contact
        // linked twice to the same project must still come back once.
        var query = _db.Contacts
            .AsNoTracking()
            .Where(c => _db.ProjectContacts.Any(pc => pc.ProjectId == projectId && pc.ContactId == c.ContactId));

        if (contactTypes is { Count: > 0 })
            query = query.Where(c => c.ContactType.HasValue && contactTypes.Contains(c.ContactType.Value));

        return query.OrderBy(c => c.Name).ToListAsync(ct);
    }

    public Task<List<Contact>> GetByProjectAsync(Guid projectId,  CancellationToken ct)
    {
        // EXISTS instead of a join: ProjectContacts has no unique (ProjectId, ContactId), so a contact
        // linked twice to the same project must still come back once.
        var query = _db.Contacts
            .AsNoTracking()
            .Where(c => _db.ProjectContacts.Any(pc => pc.ProjectId == projectId));

        return query.OrderBy(c => c.Name).ToListAsync(ct);
    }

    public Task<List<Contact>> GetByTypesAsync(IReadOnlyCollection<int> contactTypes, CancellationToken ct) =>
        _db.Contacts
            .AsNoTracking()
            .Where(c => c.ContactType != null && contactTypes.Contains(c.ContactType.Value))
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<List<Contact>> GetAllAsync(CancellationToken ct)
    {

        var query = _db.Contacts
              .AsNoTracking()
              .Where(c => c.ContactType != null)
              .OrderBy(c => c.Name);
           

        var sql = query.ToQueryString();

        var result = await query.ToListAsync(ct);
        return result;
    }
    

}