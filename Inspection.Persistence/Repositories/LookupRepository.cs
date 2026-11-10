using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Inspector;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Persistence.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly InspectionDbContext _db;

        public LookupRepository(InspectionDbContext db) => _db = db;

        public Task<List<InspectionSkill>> GetActiveSkillsAsync(CancellationToken ct) =>
            _db.InspectionSkills.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.Code).ToListAsync(ct);

        public Task<List<Contact>> GetContactsAsync(CancellationToken ct) =>
            _db.Contacts.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);
    }
}
