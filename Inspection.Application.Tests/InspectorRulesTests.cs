using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Features.Inspectors;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.Inspector.Enums;
using NSubstitute;

namespace Inspection.Application.Tests.Inspectors;

// InspectorRules is internal: add <InternalsVisibleTo Include="Inspection.Application.Tests" /> to the Application csproj.
public class InspectorRulesTests
{
    private readonly IInspectorRepository _repo = Substitute.For<IInspectorRepository>();

    private static InspectorSkillSaveDto Skill(Guid id) =>
        new() { SkillId = id, Level = InspectorSkillLevel.Experienced };

    private Task<string?> Validate(List<InspectorSkillSaveDto> skills, params Guid[] existing) =>
        InspectorRules.ValidateSkillsAsync(_repo, skills, existing.ToHashSet(), CancellationToken.None);

    [Fact]
    public async Task Requires_at_least_one_skill()
    {
        var error = await Validate(new List<InspectorSkillSaveDto>());

        Assert.Contains("at least one skill", error);
    }

    [Fact]
    public async Task Rejects_the_same_skill_twice()
    {
        var id = Guid.NewGuid();

        var error = await Validate(new() { Skill(id), Skill(id) });

        Assert.Contains("only be added once", error);
        await _repo.DidNotReceive().CountActiveSkillsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Accepts_new_skills_that_are_all_active()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        _repo.CountActiveSkillsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>()).Returns(2);

        var error = await Validate(new() { Skill(a), Skill(b) });

        Assert.Null(error);
    }

    [Fact]
    public async Task Rejects_when_a_new_skill_is_unknown_or_inactive()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        _repo.CountActiveSkillsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>()).Returns(1); // only one is active

        var error = await Validate(new() { Skill(a), Skill(b) });

        Assert.Contains("inactive", error);
    }

    [Fact]
    public async Task Skills_the_inspector_already_has_are_not_checked_again()
    {
        // A skill that was deactivated after it was assigned must not block saving the inspector
        var existing = Guid.NewGuid();

        var error = await Validate(new() { Skill(existing) }, existing);

        Assert.Null(error);
        await _repo.DidNotReceive().CountActiveSkillsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Only_the_new_skills_are_sent_to_the_active_check()
    {
        var existing = Guid.NewGuid();
        var added = Guid.NewGuid();
        _repo.CountActiveSkillsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>()).Returns(1);

        var error = await Validate(new() { Skill(existing), Skill(added) }, existing);

        Assert.Null(error);
        await _repo.Received(1).CountActiveSkillsAsync(
            Arg.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 1 && ids.Contains(added)),
            Arg.Any<CancellationToken>());
    }
}