using Inspection.Domain.Entities.Contacts;   // Contact: adjust to your namespace
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.Inspector.Enums;
using Inspection.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Tests;

public class InspectorRepositoryTests
{
    private readonly TestDb _db = new();
    private static readonly CancellationToken Ct = CancellationToken.None;

    private static InspectionSkill NewSkill(string code, bool active = true) => new()
    {
        SkillId = Guid.NewGuid(),
        Code = code,
        Name = code + " test",
        IsActive = active
    };

    private static InspectorSkill Link(InspectionSkill skill, InspectorSkillLevel level) => new()
    {
        InspectorSkillId = Guid.NewGuid(),
        SkillId = skill.SkillId,
        Level = level
    };

    // GetById / GetAll Include the contact, and an inspector always has one, so a contact row must exist.
    // (Set any other required Contact properties here.)
    private static Contact NewContact() => new() { ContactId = Guid.NewGuid(), Name = "ACME Inspection" };

    private async Task<Guid> SeedInspector(params InspectorSkill[] skills)
    {
        var contact = NewContact();
        var inspector = new Inspector
        {
            InspectorId = Guid.NewGuid(),
            InspectionCompanyContactId = contact.ContactId,
            FirstName = "Ada",
            LastName = "Lovelace",
            Skills = skills.ToList()
        };
        await using var db = _db.Create();
        db.AddRange(contact, inspector);
        await db.SaveChangesAsync(Ct);
        return inspector.InspectorId;
    }

    private async Task SeedSkills(params InspectionSkill[] skills)
    {
        await using var db = _db.Create();
        db.AddRange(skills);
        await db.SaveChangesAsync(Ct);
    }

    // ---------- counting active skills ----------
    [Fact]
    public async Task CountActiveSkills_counts_only_active_skills_from_the_given_ids()
    {
        var vt = NewSkill("VT");
        var ut = NewSkill("UT");
        var rt = NewSkill("RT", active: false);
        await SeedSkills(vt, ut, rt);
        await using var db = _db.Create();

        var count = await new InspectorRepository(db).CountActiveSkillsAsync(
            new[] { vt.SkillId, rt.SkillId, Guid.NewGuid() }, Ct);

        Assert.Equal(1, count);   // VT is active, RT is inactive, the third id does not exist
    }

    // ---------- reading ----------
    [Fact]
    public async Task GetById_loads_the_skills_with_their_code_and_name()
    {
        var ut = NewSkill("UT");
        await SeedSkills(ut);
        var id = await SeedInspector(Link(ut, InspectorSkillLevel.Professional));
        await using var db = _db.Create();

        var inspector = await new InspectorRepository(db).GetByIdAsync(id, Ct);

        var link = Assert.Single(inspector!.Skills);
        Assert.Equal("UT", link.Skill.Code);
        Assert.Equal(InspectorSkillLevel.Professional, link.Level);
    }

    [Fact]
    public async Task GetById_returns_null_for_an_unknown_inspector()
    {
        await using var db = _db.Create();

        Assert.Null(await new InspectorRepository(db).GetByIdAsync(Guid.NewGuid(), Ct));
    }

    [Fact]
    public async Task GetAll_orders_by_last_name_then_first_name()
    {
        var contact = NewContact();
        await using (var db = _db.Create())
        {
            var c = contact.ContactId;
            db.AddRange(
                contact,
                new Inspector { InspectorId = Guid.NewGuid(), InspectionCompanyContactId = c, FirstName = "Zed", LastName = "Adams" },
                new Inspector { InspectorId = Guid.NewGuid(), InspectionCompanyContactId = c, FirstName = "Amy", LastName = "Brown" },
                new Inspector { InspectorId = Guid.NewGuid(), InspectionCompanyContactId = c, FirstName = "Ann", LastName = "Adams" });
            await db.SaveChangesAsync(Ct);
        }
        await using var read = _db.Create();

        var all = await new InspectorRepository(read).GetAllAsync(Ct);

        Assert.Equal(new[] { "Ann Adams", "Zed Adams", "Amy Brown" }, all.Select(i => $"{i.FirstName} {i.LastName}"));
    }

    // ---------- ReplaceSkills ----------
    [Fact]
    public async Task ReplaceSkills_removes_updates_and_adds_so_the_inspector_matches_the_wanted_list()
    {
        var vt = NewSkill("VT");   // kept, level changes
        var rt = NewSkill("RT");   // removed
        var ut = NewSkill("UT");   // added
        await SeedSkills(vt, rt, ut);
        var id = await SeedInspector(
            Link(vt, InspectorSkillLevel.Internship),
            Link(rt, InspectorSkillLevel.Experienced));

        await using (var db = _db.Create())
        {
            var repo = new InspectorRepository(db);
            var inspector = (await repo.GetForUpdateAsync(id, Ct))!;

            repo.ReplaceSkills(inspector, new List<InspectorSkill>
            {
                Link(vt, InspectorSkillLevel.Professional),
                Link(ut, InspectorSkillLevel.Experienced)
            });
            await repo.SaveAsync(Ct);
        }

        await using var check = _db.Create();
        var skills = await check.InspectorSkills.Where(s => s.InspectorId == id).ToListAsync(Ct);
        Assert.Equal(2, skills.Count);
        Assert.Equal(InspectorSkillLevel.Professional, skills.Single(s => s.SkillId == vt.SkillId).Level);
        Assert.Equal(InspectorSkillLevel.Experienced, skills.Single(s => s.SkillId == ut.SkillId).Level);
        Assert.DoesNotContain(skills, s => s.SkillId == rt.SkillId);
    }

    [Fact]
    public async Task ReplaceSkills_keeps_the_existing_link_row_when_only_the_level_changes()
    {
        var vt = NewSkill("VT");
        await SeedSkills(vt);
        var existingLink = Link(vt, InspectorSkillLevel.Internship);
        var id = await SeedInspector(existingLink);

        await using (var db = _db.Create())
        {
            var repo = new InspectorRepository(db);
            var inspector = (await repo.GetForUpdateAsync(id, Ct))!;

            repo.ReplaceSkills(inspector, new List<InspectorSkill> { Link(vt, InspectorSkillLevel.Professional) });
            await repo.SaveAsync(Ct);
        }

        await using var check = _db.Create();
        var row = await check.InspectorSkills.SingleAsync(s => s.InspectorId == id, Ct);
        Assert.Equal(existingLink.InspectorSkillId, row.InspectorSkillId);   // updated, not recreated
        Assert.Equal(InspectorSkillLevel.Professional, row.Level);
    }

    // ---------- add / delete ----------
    [Fact]
    public async Task Add_saves_a_new_inspector_with_its_skills()
    {
        var vt = NewSkill("VT");
        await SeedSkills(vt);
        var inspector = new Inspector
        {
            InspectorId = Guid.NewGuid(),
            InspectionCompanyContactId = Guid.NewGuid(),
            FirstName = "Ada"
        };

        await using (var db = _db.Create())
        {
            var repo = new InspectorRepository(db);
            repo.ReplaceSkills(inspector, new List<InspectorSkill> { Link(vt, InspectorSkillLevel.Experienced) });
            await repo.AddAsync(inspector, Ct);
        }

        await using var check = _db.Create();
        Assert.True(await check.Inspectors.AnyAsync(i => i.InspectorId == inspector.InspectorId, Ct));
        Assert.Equal(1, await check.InspectorSkills.CountAsync(s => s.InspectorId == inspector.InspectorId, Ct));
    }

    [Fact]
    public async Task Delete_removes_the_inspector()
    {
        var id = await SeedInspector();

        await using (var db = _db.Create())
        {
            var repo = new InspectorRepository(db);
            var inspector = (await repo.GetForUpdateAsync(id, Ct))!;
            await repo.DeleteAsync(inspector, Ct);
        }

        await using var check = _db.Create();
        Assert.False(await check.Inspectors.AnyAsync(i => i.InspectorId == id, Ct));
    }
}