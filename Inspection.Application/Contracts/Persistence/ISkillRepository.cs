using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.Common;   // InspectionSkill: adjust to your namespace
using System.Threading;

namespace Inspection.Application.Contracts.Persistence;

public interface ISkillRepository
{
    Task<List<InspectionSkill>> GetAllAsync(CancellationToken ct);
    Task<List<InspectionSkill>> GetActiveAsync(CancellationToken ct);
    Task<InspectionSkill?> GetByIdAsync(Guid skillId, CancellationToken ct);

    Task<bool> CodeExistsAsync(string code, Guid? excludingSkillId, CancellationToken ct);
    Task<int> CountInspectorsUsingAsync(Guid skillId, CancellationToken ct);

    Task AddAsync(InspectionSkill skill, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
    Task DeleteAsync(InspectionSkill skill, CancellationToken ct);
}
