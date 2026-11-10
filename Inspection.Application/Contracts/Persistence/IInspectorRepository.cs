using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Persistence;


public interface IInspectorRepository
{
    // read-only, with contact and skills loaded
   public Task<List<Inspector>> GetAllAsync(CancellationToken ct);
    public Task<Inspector?> GetByIdAsync(Guid inspectorId, CancellationToken ct);

    // tracked, with skills loaded (for update / delete)
    public Task<Inspector?> GetForUpdateAsync(Guid inspectorId, CancellationToken ct);

    public Task<bool> ContactExistsAsync(Guid contactId, CancellationToken ct);
    public Task<int> CountActiveSkillsAsync(IReadOnlyCollection<Guid> skillIds, CancellationToken ct);

    // Makes inspector.Skills match "wanted": removes, updates level, adds. Does not save.
    public void ReplaceSkills(Inspector inspector, IReadOnlyCollection<InspectorSkill> wanted);

    public Task AddAsync(Inspector inspector, CancellationToken ct);
    public Task SaveAsync(CancellationToken ct);
    public Task DeleteAsync(Inspector inspector, CancellationToken ct);

    Task<List<Inspector>> GetByApprovalStateAsync(int? qcApprovalState, CancellationToken ct);
    // IInspectorRepository.cs — add this member
    Task<List<Inspector>> GetFilteredAsync(Guid? inspectionCompanyContactId, int? qcApprovalState, CancellationToken ct);
}


//public interface IInspectorRepository : IGenericRepository<Inspector, InspectorId>
//{
//    public Task<Inspector> Add(Inspector entity)
//    {
//        throw new NotImplementedException();
//    }

//    public Task Delete(Inspector entity)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<Inspector> Get(Guid id)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<IReadOnlyList<Inspector>> GetAll()
//    {
//        throw new NotImplementedException();
//    }

//    public Task<Inspector> Update(Inspector entity)
//    {
//        throw new NotImplementedException();
//    }
//}

