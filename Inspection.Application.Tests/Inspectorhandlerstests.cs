using Inspection.Application.Contracts.Infrastructure;   // IAttachmentStore: adjust to your namespace
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Exceptions;
using Inspection.Application.Features.Inspectors;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.Inspector.Enums;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Linq;

namespace Inspection.Application.Tests.Inspectors;

internal static class InspectorTestData
{
    public static InspectorSaveDto SaveDto(Guid contactId, params (Guid SkillId, InspectorSkillLevel Level)[] skills) => new()
    {
        InspectionCompanyContactId = contactId,
        FirstName = "  Ada ",
        LastName = "Lovelace ",
        Position = "NDT inspector",
        ExpireDate = new DateTime(2027, 1, 1),
        Skills = skills.Select(s => new InspectorSkillSaveDto { SkillId = s.SkillId, Level = s.Level }).ToList()
    };
}

// ===================================================== queries
public class GetInspectorsHandlerTests
{
    [Fact]
    public async Task Maps_every_inspector_to_a_dto()
    {
        var repo = Substitute.For<IInspectorRepository>();
        repo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Inspector>
        {
            new() { InspectorId = Guid.NewGuid(), FirstName = "Ada", LastName = "Lovelace", Name = "Ada Lovelace" },
            new() { InspectorId = Guid.NewGuid(), FirstName = "Alan", LastName = "Turing", Name = "Alan Turing" }
        });
        var handler = new GetInspectorsHandler(repo);

        var result = await handler.Handle(new GetInspectorsQuery(), CancellationToken.None);

        Assert.Equal(new[] { "Ada Lovelace", "Alan Turing" }, result.Select(r => r.Name));
    }
}

public class GetInspectorByIdHandlerTests
{
    private readonly IInspectorRepository _repo = Substitute.For<IInspectorRepository>();

    [Fact]
    public async Task Throws_NotFound_for_an_unknown_inspector()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Inspector?)null);
        var handler = new GetInspectorByIdHandler(_repo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetInspectorByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Returns_the_inspector_as_a_dto()
    {
        var id = Guid.NewGuid();
        _repo.GetByIdAsync(id, Arg.Any<CancellationToken>())
             .Returns(new Inspector { InspectorId = id, FirstName = "Ada", Position = "QA" });
        var handler = new GetInspectorByIdHandler(_repo);

        var dto = await handler.Handle(new GetInspectorByIdQuery(id), CancellationToken.None);

        Assert.Equal(id, dto.InspectorId);
        Assert.Equal("Ada", dto.FirstName);
        Assert.Equal("QA", dto.Position);
    }
}

// ===================================================== create
public class CreateInspectorHandlerTests
{
    private readonly IInspectorRepository _repo = Substitute.For<IInspectorRepository>();
    private readonly CreateInspectorHandler _handler;
    private readonly Guid _contactId = Guid.NewGuid();
    private readonly Guid _skillA = Guid.NewGuid();
    private readonly Guid _skillB = Guid.NewGuid();

    public CreateInspectorHandlerTests()
    {
        _handler = new CreateInspectorHandler(_repo);
        _repo.ContactExistsAsync(_contactId, Arg.Any<CancellationToken>()).Returns(true);
        _repo.CountActiveSkillsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>()).Returns(2);
    }

    private Task<InspectorDto> Send(InspectorSaveDto dto) =>
        _handler.Handle(new CreateInspectorCommand(dto), CancellationToken.None);

    [Fact]
    public async Task Throws_when_the_contact_does_not_exist()
    {
        _repo.ContactExistsAsync(_contactId, Arg.Any<CancellationToken>()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => Send(InspectorTestData.SaveDto(_contactId, (_skillA, InspectorSkillLevel.Experienced))));

        Assert.Contains("contact", ex.Message, StringComparison.OrdinalIgnoreCase);
        await _repo.DidNotReceive().AddAsync(Arg.Any<Inspector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Throws_when_no_skill_is_given()
    {
        await Assert.ThrowsAsync<BusinessRuleException>(() => Send(InspectorTestData.SaveDto(_contactId)));

        await _repo.DidNotReceive().AddAsync(Arg.Any<Inspector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Throws_when_a_skill_is_given_twice()
    {
        await Assert.ThrowsAsync<BusinessRuleException>(() => Send(InspectorTestData.SaveDto(
            _contactId, (_skillA, InspectorSkillLevel.Internship), (_skillA, InspectorSkillLevel.Professional))));
    }

    [Fact]
    public async Task Throws_when_a_skill_is_inactive_or_unknown()
    {
        _repo.CountActiveSkillsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>()).Returns(1);

        await Assert.ThrowsAsync<BusinessRuleException>(() => Send(InspectorTestData.SaveDto(
            _contactId, (_skillA, InspectorSkillLevel.Experienced), (_skillB, InspectorSkillLevel.Experienced))));
    }

    [Fact]
    public async Task Creates_the_inspector_with_server_controlled_fields_and_the_given_skills()
    {
        Inspector? added = null;
        _repo.When(r => r.AddAsync(Arg.Any<Inspector>(), Arg.Any<CancellationToken>()))
             .Do(call => added = call.Arg<Inspector>());
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(_ => added);

        var before = DateTime.UtcNow;
        var dto = await Send(InspectorTestData.SaveDto(
            _contactId, (_skillA, InspectorSkillLevel.Professional), (_skillB, InspectorSkillLevel.Internship)));

        Assert.NotNull(added);
        Assert.NotEqual(Guid.Empty, added!.InspectorId);
        Assert.Equal(1, added.VersionNumber);
        Assert.True(added.CreatedDate >= before.AddSeconds(-1));
        Assert.Equal(_contactId, added.InspectionCompanyContactId);
        Assert.Equal("Ada", added.FirstName);                 // trimmed
        Assert.Equal("Lovelace", added.LastName);
        Assert.Equal("Ada Lovelace", added.Name);             // falls back to first + last name

        _repo.Received(1).ReplaceSkills(added, Arg.Is<IReadOnlyCollection<InspectorSkill>>(wanted =>
            wanted.Count == 2 &&
            wanted.Any(s => s.SkillId == _skillA && s.Level == InspectorSkillLevel.Professional) &&
            wanted.Any(s => s.SkillId == _skillB && s.Level == InspectorSkillLevel.Internship)));

        Assert.Equal(added.InspectorId, dto.InspectorId);
        Assert.Equal("Ada Lovelace", dto.Name);
    }

    [Fact]
    public async Task Keeps_the_display_name_when_one_is_given()
    {
        Inspector? added = null;
        _repo.When(r => r.AddAsync(Arg.Any<Inspector>(), Arg.Any<CancellationToken>()))
             .Do(call => added = call.Arg<Inspector>());
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(_ => added);
        var data = InspectorTestData.SaveDto(_contactId, (_skillA, InspectorSkillLevel.Experienced));
        data.Name = "  A. Lovelace ";

        await Send(data);

        Assert.Equal("A. Lovelace", added!.Name);
    }
}

// ===================================================== update
public class UpdateInspectorHandlerTests
{
    private readonly IInspectorRepository _repo = Substitute.For<IInspectorRepository>();
    private readonly UpdateInspectorHandler _handler;
    private readonly Guid _id = Guid.NewGuid();
    private readonly Guid _contactId = Guid.NewGuid();
    private readonly Guid _existingSkill = Guid.NewGuid();
    private readonly Guid _newSkill = Guid.NewGuid();
    private readonly Inspector _inspector;

    public UpdateInspectorHandlerTests()
    {
        _handler = new UpdateInspectorHandler(_repo);
        _inspector = new Inspector
        {
            InspectorId = _id,
            FirstName = "Old",
            LastName = "Name",
            VersionNumber = 3,
            Skills = new List<InspectorSkill>
            {
                new() { SkillId = _existingSkill, Level = InspectorSkillLevel.Internship }
            }
        };
        _repo.GetForUpdateAsync(_id, Arg.Any<CancellationToken>()).Returns(_inspector);
        _repo.GetByIdAsync(_id, Arg.Any<CancellationToken>()).Returns(_inspector);
        _repo.ContactExistsAsync(_contactId, Arg.Any<CancellationToken>()).Returns(true);
        _repo.CountActiveSkillsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>()).Returns(1);
    }

    private Task<InspectorDto> Send(InspectorSaveDto dto) =>
        _handler.Handle(new UpdateInspectorCommand(_id, dto), CancellationToken.None);

    [Fact]
    public async Task Throws_NotFound_for_an_unknown_inspector()
    {
        _repo.GetForUpdateAsync(_id, Arg.Any<CancellationToken>()).Returns((Inspector?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => Send(InspectorTestData.SaveDto(_contactId, (_existingSkill, InspectorSkillLevel.Experienced))));
        await _repo.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Throws_when_the_contact_does_not_exist()
    {
        _repo.ContactExistsAsync(_contactId, Arg.Any<CancellationToken>()).Returns(false);

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => Send(InspectorTestData.SaveDto(_contactId, (_existingSkill, InspectorSkillLevel.Experienced))));
        await _repo.DidNotReceive().SaveAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Only_newly_added_skills_must_be_active()
    {
        await Send(InspectorTestData.SaveDto(
            _contactId,
            (_existingSkill, InspectorSkillLevel.Professional),    // already assigned: not re-checked
            (_newSkill, InspectorSkillLevel.Experienced)));

        await _repo.Received(1).CountActiveSkillsAsync(
            Arg.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 1 && ids.Contains(_newSkill)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Copies_the_fields_bumps_the_version_and_saves_once()
    {
        var data = InspectorTestData.SaveDto(
            _contactId,
            (_existingSkill, InspectorSkillLevel.Professional),
            (_newSkill, InspectorSkillLevel.Experienced));

        var dto = await Send(data);

        Assert.Equal("Ada", _inspector.FirstName);              // trimmed value copied onto the tracked entity
        Assert.Equal("NDT inspector", _inspector.Position);
        Assert.Equal("Ada Lovelace", _inspector.Name);
        Assert.Equal(_contactId, _inspector.InspectionCompanyContactId);
        Assert.Equal(4, _inspector.VersionNumber);
        Assert.Equal(_id, _inspector.InspectorId);              // the id is never overwritten

        _repo.Received(1).ReplaceSkills(_inspector, Arg.Is<IReadOnlyCollection<InspectorSkill>>(wanted =>
            wanted.Count == 2 &&
            wanted.Any(s => s.SkillId == _existingSkill && s.Level == InspectorSkillLevel.Professional) &&
            wanted.Any(s => s.SkillId == _newSkill)));
        await _repo.Received(1).SaveAsync(Arg.Any<CancellationToken>());

        Assert.Equal(4, dto.VersionNumber);
    }
}

// ===================================================== delete
public class DeleteInspectorHandlerTests
{
    private readonly IInspectorRepository _inspectors = Substitute.For<IInspectorRepository>();
    private readonly IDocumentRepository _documents = Substitute.For<IDocumentRepository>();
    private readonly IAttachmentStore _store = Substitute.For<IAttachmentStore>();
    private readonly Guid _id = Guid.NewGuid();
    private readonly Inspector _inspector;

    public DeleteInspectorHandlerTests()
    {
        _inspector = new Inspector { InspectorId = _id };
        _inspectors.GetForUpdateAsync(_id, Arg.Any<CancellationToken>()).Returns(_inspector);
    }

    private Task Send() =>
        new DeleteInspectorHandler(_inspectors, _documents, _store)
            .Handle(new DeleteInspectorCommand(_id), CancellationToken.None);

    [Fact]
    public async Task Throws_NotFound_for_an_unknown_inspector()
    {
        _inspectors.GetForUpdateAsync(_id, Arg.Any<CancellationToken>()).Returns((Inspector?)null);

        await Assert.ThrowsAsync<NotFoundException>(Send);
        await _inspectors.DidNotReceive().DeleteAsync(Arg.Any<Inspector>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Stages_the_documents_then_deletes_the_inspector_then_the_files()
    {
        var doc1 = new Document { DocumentId = Guid.NewGuid(), Version = "1" };
        var doc2 = new Document { DocumentId = Guid.NewGuid(), Version = "1" };
        _documents.StageRemoveAllForOwnerAsync(DocumentOwnerType.Inspector, _id, Arg.Any<CancellationToken>())
                  .Returns(new List<Document> { doc1, doc2 });

        await Send();

        Received.InOrder(() =>
        {
            _documents.StageRemoveAllForOwnerAsync(DocumentOwnerType.Inspector, _id, Arg.Any<CancellationToken>());
            _inspectors.DeleteAsync(_inspector, Arg.Any<CancellationToken>());
            _store.DeleteAsync(doc1, Arg.Any<CancellationToken>());
            _store.DeleteAsync(doc2, Arg.Any<CancellationToken>());
        });
    }

    [Fact]
    public async Task Does_not_delete_any_file_when_the_database_delete_fails()
    {
        var doc = new Document { DocumentId = Guid.NewGuid(), Version = "1" };
        _documents.StageRemoveAllForOwnerAsync(DocumentOwnerType.Inspector, _id, Arg.Any<CancellationToken>())
                  .Returns(new List<Document> { doc });
        _inspectors.DeleteAsync(_inspector, Arg.Any<CancellationToken>())
                   .ThrowsAsync(new InvalidOperationException("db down"));

        await Assert.ThrowsAsync<InvalidOperationException>(Send);

        await _store.DidNotReceive().DeleteAsync(Arg.Any<Document>(), Arg.Any<CancellationToken>());
    }
}