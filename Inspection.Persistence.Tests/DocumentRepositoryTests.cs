using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using Inspection.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Tests;

public class DocumentRepositoryTests
{
    private readonly TestDb _db = new();
    private static readonly CancellationToken Ct = CancellationToken.None;
    private const DocumentOwnerType Inspector = DocumentOwnerType.Inspector;

    // ---------- seeding helpers ----------
    private static DocumentType NewType(string name, bool active = true) => new()
    {
        DocumentTypeId = Guid.NewGuid(),
        DocumentTypeName = name,
        DocumentTypeCode = Random.Shared.Next(1000, 99999),
        IsActive = active,
        AllowedExtensions = "*",
        MaxSizeMB = 10
    };

    private static Document NewDoc(Guid typeId, DateTime? issue = null) => new()
    {
        DocumentId = Guid.NewGuid(),
        DocumentTypeId = typeId,
        Status = "Active",
        Version = "1",
        IssueDate = issue
    };

    private static DocumentLink NewLink(Document doc, Guid ownerId) => new()
    {
        DocumentId = doc.DocumentId,
        OwnerType = (int)Inspector,
        OwnerId = ownerId
    };

    private async Task Seed(params object[] entities)
    {
        await using var db = _db.Create();
        db.AddRange(entities);
        await db.SaveChangesAsync(Ct);
    }

    private DocumentRepository Repo(InspectionDbContext db) => new(db);

    // ---------- document types ----------
    [Fact]
    public async Task GetActiveTypes_returns_only_active_types_ordered_by_name()
    {
        await Seed(NewType("Resume"), NewType("Grade"), NewType("Old", active: false));
        await using var db = _db.Create();

        var types = await Repo(db).GetActiveTypesAsync(null, Ct);

        Assert.Equal(new[] { "Grade", "Resume" }, types.Select(t => t.DocumentTypeName));
    }

    [Fact]
    public async Task GetActiveTypes_with_a_contact_type_returns_only_the_visible_ones()
    {
        var resume = NewType("Resume");
        var grade = NewType("Grade");
        var hidden = NewType("Hidden");
        await Seed(resume, grade, hidden,
            new DocumentTypeVisibility { DocumentTypeId = resume.DocumentTypeId, ContactTypeCode = 5 },
            new DocumentTypeVisibility { DocumentTypeId = grade.DocumentTypeId, ContactTypeCode = 5 },
            new DocumentTypeVisibility { DocumentTypeId = hidden.DocumentTypeId, ContactTypeCode = 9 });
        await using var db = _db.Create();

        var types = await Repo(db).GetActiveTypesAsync(5, Ct);

        Assert.Equal(new[] { "Grade", "Resume" }, types.Select(t => t.DocumentTypeName));
    }

    [Fact]
    public async Task GetActiveTypes_skips_inactive_types_even_when_they_are_visible()
    {
        var old = NewType("Old", active: false);
        await Seed(old, new DocumentTypeVisibility { DocumentTypeId = old.DocumentTypeId, ContactTypeCode = 5 });
        await using var db = _db.Create();

        var types = await Repo(db).GetActiveTypesAsync(5, Ct);

        Assert.Empty(types);
    }

    [Fact]
    public async Task GetType_finds_a_type_by_id_and_returns_null_for_an_unknown_id()
    {
        var type = NewType("Resume");
        await Seed(type);
        await using var db = _db.Create();

        Assert.Equal("Resume", (await Repo(db).GetTypeAsync(type.DocumentTypeId, Ct))!.DocumentTypeName);
        Assert.Null(await Repo(db).GetTypeAsync(Guid.NewGuid(), Ct));
    }

    [Fact]
    public async Task IsTypeVisible_is_true_only_for_the_configured_contact_type()
    {
        var type = NewType("Resume");
        await Seed(type, new DocumentTypeVisibility { DocumentTypeId = type.DocumentTypeId, ContactTypeCode = 5 });
        await using var db = _db.Create();
        var repo = Repo(db);

        Assert.True(await repo.IsTypeVisibleAsync(type.DocumentTypeId, 5, Ct));
        Assert.False(await repo.IsTypeVisibleAsync(type.DocumentTypeId, 6, Ct));
    }

    // ---------- reading documents ----------
    [Fact]
    public async Task GetByOwner_returns_only_that_owners_documents_with_type_names_in_order()
    {
        var owner = Guid.NewGuid();
        var other = Guid.NewGuid();
        var resume = NewType("Resume");
        var grade = NewType("Grade");

        var resumeNew = NewDoc(resume.DocumentTypeId, new DateTime(2026, 6, 1));
        var resumeOld = NewDoc(resume.DocumentTypeId, new DateTime(2025, 1, 1));
        var gradeDoc = NewDoc(grade.DocumentTypeId);
        var othersDoc = NewDoc(resume.DocumentTypeId);

        await Seed(resume, grade, resumeNew, resumeOld, gradeDoc, othersDoc,
            NewLink(resumeNew, owner), NewLink(resumeOld, owner), NewLink(gradeDoc, owner), NewLink(othersDoc, other));
        await using var db = _db.Create();

        var rows = await Repo(db).GetByOwnerAsync(Inspector, owner, Ct);

        Assert.Equal(3, rows.Count);
        Assert.Equal(new[] { "Grade", "Resume", "Resume" }, rows.Select(r => r.DocumentTypeName));
        Assert.Equal(resumeOld.DocumentId, rows[1].Document.DocumentId);   // older issue date first within a type
        Assert.Equal(resumeNew.DocumentId, rows[2].Document.DocumentId);
        Assert.DoesNotContain(rows, r => r.Document.DocumentId == othersDoc.DocumentId);
    }

    [Fact]
    public async Task OwnerHasDocumentOfType_checks_owner_and_type()
    {
        var owner = Guid.NewGuid();
        var resume = NewType("Resume");
        var grade = NewType("Grade");
        var doc = NewDoc(resume.DocumentTypeId);
        await Seed(resume, grade, doc, NewLink(doc, owner));
        await using var db = _db.Create();
        var repo = Repo(db);

        Assert.True(await repo.OwnerHasDocumentOfTypeAsync(Inspector, owner, resume.DocumentTypeId, Ct));
        Assert.False(await repo.OwnerHasDocumentOfTypeAsync(Inspector, owner, grade.DocumentTypeId, Ct));          // other type
        Assert.False(await repo.OwnerHasDocumentOfTypeAsync(Inspector, Guid.NewGuid(), resume.DocumentTypeId, Ct)); // other owner
    }

    [Fact]
    public async Task GetForOwner_returns_the_document_only_for_a_linked_owner()
    {
        var owner = Guid.NewGuid();
        var type = NewType("Resume");
        var doc = NewDoc(type.DocumentTypeId);
        await Seed(type, doc, NewLink(doc, owner));
        await using var db = _db.Create();
        var repo = Repo(db);

        Assert.Equal(doc.DocumentId, (await repo.GetForOwnerAsync(doc.DocumentId, Inspector, owner, Ct))!.DocumentId);
        Assert.Null(await repo.GetForOwnerAsync(doc.DocumentId, Inspector, Guid.NewGuid(), Ct));
        Assert.Null(await repo.GetForOwnerAsync(Guid.NewGuid(), Inspector, owner, Ct));
    }

    // ---------- writing ----------
    [Fact]
    public async Task Add_saves_the_document_and_its_link()
    {
        var owner = Guid.NewGuid();
        var type = NewType("Resume");
        var doc = NewDoc(type.DocumentTypeId);

        await using (var db = _db.Create())
            await Repo(db).AddAsync(doc, NewLink(doc, owner), Ct);

        await using var check = _db.Create();
        Assert.True(await check.Documents.AnyAsync(d => d.DocumentId == doc.DocumentId, Ct));
        Assert.True(await check.DocumentLinks.AnyAsync(l => l.DocumentId == doc.DocumentId && l.OwnerId == owner, Ct));
    }

    // ---------- unlink ----------
    [Fact]
    public async Task Unlink_reports_when_the_owner_has_no_such_document()
    {
        await using var db = _db.Create();

        var result = await Repo(db).UnlinkAsync(Guid.NewGuid(), Inspector, Guid.NewGuid(), Ct);

        Assert.False(result.LinkFound);
        Assert.Null(result.OrphanedDocument);
    }

    [Fact]
    public async Task Unlink_deletes_the_document_when_no_other_owner_uses_it()
    {
        var owner = Guid.NewGuid();
        var type = NewType("Resume");
        var doc = NewDoc(type.DocumentTypeId);
        await Seed(type, doc, NewLink(doc, owner));

        UnlinkResult result;
        await using (var db = _db.Create())
            result = await Repo(db).UnlinkAsync(doc.DocumentId, Inspector, owner, Ct);

        Assert.True(result.LinkFound);
        Assert.Equal(doc.DocumentId, result.OrphanedDocument!.DocumentId);   // caller deletes the file
        await using var check = _db.Create();
        Assert.False(await check.Documents.AnyAsync(d => d.DocumentId == doc.DocumentId, Ct));
        Assert.False(await check.DocumentLinks.AnyAsync(l => l.DocumentId == doc.DocumentId, Ct));
    }

    [Fact]
    public async Task Unlink_keeps_the_document_when_another_owner_still_uses_it()
    {
        var owner = Guid.NewGuid();
        var otherOwner = Guid.NewGuid();
        var type = NewType("Resume");
        var doc = NewDoc(type.DocumentTypeId);
        await Seed(type, doc, NewLink(doc, owner), NewLink(doc, otherOwner));

        UnlinkResult result;
        await using (var db = _db.Create())
            result = await Repo(db).UnlinkAsync(doc.DocumentId, Inspector, owner, Ct);

        Assert.True(result.LinkFound);
        Assert.Null(result.OrphanedDocument);                                // the file must stay
        await using var check = _db.Create();
        Assert.True(await check.Documents.AnyAsync(d => d.DocumentId == doc.DocumentId, Ct));
        Assert.False(await check.DocumentLinks.AnyAsync(l => l.DocumentId == doc.DocumentId && l.OwnerId == owner, Ct));
        Assert.True(await check.DocumentLinks.AnyAsync(l => l.DocumentId == doc.DocumentId && l.OwnerId == otherOwner, Ct));
    }

    // ---------- removing all documents of an owner ----------
    [Fact]
    public async Task StageRemoveAll_returns_only_documents_that_nobody_else_uses_and_saves_nothing_yet()
    {
        var owner = Guid.NewGuid();
        var otherOwner = Guid.NewGuid();
        var type = NewType("Resume");
        var ownDoc = NewDoc(type.DocumentTypeId);
        var sharedDoc = NewDoc(type.DocumentTypeId);
        await Seed(type, ownDoc, sharedDoc,
            NewLink(ownDoc, owner), NewLink(sharedDoc, owner), NewLink(sharedDoc, otherOwner));

        await using var db = _db.Create();
        var orphans = await Repo(db).StageRemoveAllForOwnerAsync(Inspector, owner, Ct);

        Assert.Equal(new[] { ownDoc.DocumentId }, orphans.Select(d => d.DocumentId));

        // nothing is saved until the caller saves (the inspector delete does it in one go)
        await using (var before = _db.Create())
        {
            Assert.Equal(2, await before.DocumentLinks.CountAsync(l => l.OwnerId == owner, Ct));
            Assert.True(await before.Documents.AnyAsync(d => d.DocumentId == ownDoc.DocumentId, Ct));
        }

        await db.SaveChangesAsync(Ct);

        await using var after = _db.Create();
        Assert.Equal(0, await after.DocumentLinks.CountAsync(l => l.OwnerId == owner, Ct));
        Assert.False(await after.Documents.AnyAsync(d => d.DocumentId == ownDoc.DocumentId, Ct));
        Assert.True(await after.Documents.AnyAsync(d => d.DocumentId == sharedDoc.DocumentId, Ct));       // still used by otherOwner
        Assert.True(await after.DocumentLinks.AnyAsync(l => l.DocumentId == sharedDoc.DocumentId && l.OwnerId == otherOwner, Ct));
    }

    [Fact]
    public async Task StageRemoveAll_returns_an_empty_list_for_an_owner_without_documents()
    {
        await using var db = _db.Create();

        var orphans = await Repo(db).StageRemoveAllForOwnerAsync(Inspector, Guid.NewGuid(), Ct);

        Assert.Empty(orphans);
    }
}