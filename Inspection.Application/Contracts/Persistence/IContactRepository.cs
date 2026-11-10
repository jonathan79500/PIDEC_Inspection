using Inspection.Domain.Entities.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Persistence;

public interface IContactRepository
{
    // Contacts linked to the project through ProjectContacts; optionally only one contact type. Ordered by name.
    Task<List<Contact>> GetByProject_TypesAsync(Guid projectId, IReadOnlyCollection<int>? contactType, CancellationToken ct);

    // All contacts whose ContactType is one of the given codes. Ordered by name.
    Task<List<Contact>> GetByTypesAsync(IReadOnlyCollection<int> contactTypes, CancellationToken ct);


    // Contacts linked to the project through ProjectContacts; optionally only one contact type. Ordered by name.
    Task<List<Contact>> GetByProjectAsync(Guid projectId, CancellationToken ct);
    

    Task<List<Contact>> GetAllAsync(CancellationToken ct);
}


