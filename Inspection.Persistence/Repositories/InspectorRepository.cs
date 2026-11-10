using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;

namespace Inspection.Persistence.Repositories;


public class InspectorRepository : IInspectorRepository
{
    private readonly InspectionDbContext _db;

    public InspectorRepository(InspectionDbContext db) => _db = db;

    public Task<List<Inspector>> GetAllAsync(CancellationToken ct) =>
        _db.Inspectors
            .AsNoTracking()
            .Include(i => i.InspectionCompanyContact)
            .Include(i => i.Skills).ThenInclude(s => s.Skill)
            .OrderBy(i => i.LastName).ThenBy(i => i.FirstName)
            .ToListAsync(ct);

    public Task<Inspector?> GetByIdAsync(Guid inspectorId, CancellationToken ct) =>
        _db.Inspectors
            .AsNoTracking()
            .Include(i => i.InspectionCompanyContact)
            .Include(i => i.Skills).ThenInclude(s => s.Skill)
            .FirstOrDefaultAsync(i => i.InspectorId == inspectorId, ct);

    public Task<Inspector?> GetForUpdateAsync(Guid inspectorId, CancellationToken ct) =>
        _db.Inspectors
            .Include(i => i.Skills)
            .FirstOrDefaultAsync(i => i.InspectorId == inspectorId, ct);

    public Task<bool> ContactExistsAsync(Guid contactId, CancellationToken ct) =>
        _db.Contacts.AnyAsync(c => c.ContactId == contactId, ct);

    public Task<int> CountActiveSkillsAsync(IReadOnlyCollection<Guid> skillIds, CancellationToken ct) =>
        _db.InspectionSkills.CountAsync(s => skillIds.Contains(s.SkillId) && s.IsActive, ct);

    public void ReplaceSkills(Inspector inspector, IReadOnlyCollection<InspectorSkill> wanted)
    {
        var wantedBySkill = wanted.ToDictionary(s => s.SkillId);

        foreach (var old in inspector.Skills.Where(s => !wantedBySkill.ContainsKey(s.SkillId)).ToList())
        {
            inspector.Skills.Remove(old);
            _db.InspectorSkills.Remove(old);
        }

        foreach (var existing in inspector.Skills)
            existing.Level = wantedBySkill[existing.SkillId].Level;

        var have = inspector.Skills.Select(s => s.SkillId).ToHashSet();
        foreach (var added in wanted.Where(w => !have.Contains(w.SkillId)))
            inspector.Skills.Add(added);
    }

    public async Task AddAsync(Inspector inspector, CancellationToken ct)
    {
        _db.Inspectors.Add(inspector);
        await _db.SaveChangesAsync(ct);
    }

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Inspector inspector, CancellationToken ct)
    {
        _db.Inspectors.Remove(inspector);   // InspectorSkill rows are removed by cascade
        await _db.SaveChangesAsync(ct);
    }

    public Task<List<Inspector>> GetByApprovalStateAsync(int? qcApprovalState, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<Inspector>> GetFilteredAsync(
    Guid? inspectionCompanyContactId, int? qcApprovalState, CancellationToken ct)
    {
        var query = _db.Inspectors
            .AsNoTracking()
            .Include(i => i.InspectionCompanyContact)
            .Include(i => i.Skills).ThenInclude(s => s.Skill)
            .AsQueryable();

        if (inspectionCompanyContactId is Guid contactId)
            query = query.Where(i => i.InspectionCompanyContactId == contactId);

        if (qcApprovalState is int state)
            query = query.Where(i => i.QCApprovalState == state);

        return query.OrderBy(i => i.LastName).ThenBy(i => i.FirstName).ToListAsync(ct);
    }
}


//public class InspectorRepository : IGenericRepository<Inspector, InspectorId>, IInspectorRepository
//{

//    private readonly InspectionDbContext _dbContext;
//    private IServiceScopeFactory _serviceProvider;
//    private IEDMSService _eDMSService;
//    private IUserManagerRepository _userManagerRepository;

//    public InspectorRepository(InspectionDbContext dbContext, IServiceScopeFactory serviceProvider, IEDMSService eDMSService, IUserManagerRepository userManagerRepository) : base()
//    {
//        _dbContext = dbContext;
//        _serviceProvider = serviceProvider;
//        _eDMSService = eDMSService;
//        _userManagerRepository = userManagerRepository;
//    }


//    public async Task<Inspector> CreateNewInspector(Inspector newInspector)
//    {
//        try
//        {
//            if (newInspector == null)
//            {
//                throw new ArgumentNullException("new inspector has some errors");
//            }

//            var addedInspector = await _dbContext.Inspectors.AddAsync(newInspector);
//            await _dbContext.SaveChangesAsync();
//            return await Task.FromResult(addedInspector.Entity);
//        }
//        catch (Exception exp)
//        {
//            return null;
//        }
//    }


//     Task<Inspector> IGenericRepository<Inspector, InspectorId>.Add(Inspector newInspector)
//    {

//      throw new ArgumentNullException("new inspector has some errors");

//    }

//    Task IGenericRepository<Inspector, InspectorId>.Delete(Inspector entity)
//    {
//        throw new NotImplementedException();
//    }

//    Task<Inspector> IGenericRepository<Inspector, InspectorId>.Get(InspectorId id)
//    {
//        throw new NotImplementedException();
//    }

//    Task<IReadOnlyList<Inspector>> IGenericRepository<Inspector, InspectorId>.GetAll()
//    {
//        throw new NotImplementedException();
//    }

//    Task<Inspector> IGenericRepository<Inspector, InspectorId>.Update(Inspector entity)
//    {
//        throw new NotImplementedException();
//    }
//}
