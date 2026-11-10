using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Common;   // InspectionSkill: adjust to your namespace
using Inspection.Domain.Entities.Inspector;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly InspectionDbContext _db;

    public SkillRepository(InspectionDbContext db) => _db = db;

    public Task<List<InspectionSkill>> GetAllAsync(CancellationToken ct) =>
        _db.InspectionSkills.AsNoTracking().OrderBy(s => s.Code).ToListAsync(ct);

    public Task<List<InspectionSkill>> GetActiveAsync(CancellationToken ct) =>
        _db.InspectionSkills.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.Code).ToListAsync(ct);

    public Task<InspectionSkill?> GetByIdAsync(Guid skillId, CancellationToken ct) =>
        _db.InspectionSkills.FirstOrDefaultAsync(s => s.SkillId == skillId, ct);   // tracked: Update reuses this

    public Task<bool> CodeExistsAsync(string code, Guid? excludingSkillId, CancellationToken ct)
    {
        var query = _db.InspectionSkills.AsNoTracking().Where(s => s.Code == code);
        if (excludingSkillId is Guid id) query = query.Where(s => s.SkillId != id);
        return query.AnyAsync(ct);
    }

    public Task<int> CountInspectorsUsingAsync(Guid skillId, CancellationToken ct) =>
        _db.InspectorSkills.CountAsync(s => s.SkillId == skillId, ct);

    public async Task AddAsync(InspectionSkill skill, CancellationToken ct)
    {
        _db.InspectionSkills.Add(skill);
        await _db.SaveChangesAsync(ct);
    }

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public async Task DeleteAsync(InspectionSkill skill, CancellationToken ct)
    {
        _db.InspectionSkills.Remove(skill);
        await _db.SaveChangesAsync(ct);
    }
}