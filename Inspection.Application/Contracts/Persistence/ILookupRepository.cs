using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Persistence;

public interface ILookupRepository
{
    Task<List<InspectionSkill>> GetActiveSkillsAsync(CancellationToken ct);
    Task<List<Contact>> GetContactsAsync(CancellationToken ct);
}
