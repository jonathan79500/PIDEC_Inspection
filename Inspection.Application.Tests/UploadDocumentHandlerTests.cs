using Inspection.Application.Contracts.Infrastructure;   // IAttachmentStore, AttachmentInfo: adjust to your namespace
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.Exceptions;
using Inspection.Application.Features.Documents;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;   // AttachmentId
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Inspection.Application.Tests.Documents;

public class UploadDocumentHandlerTests
{
    private const int ContactType = 5;

    private readonly IDocumentRepository _repo = Substitute.For<IDocumentRepository>();
    private readonly IAttachmentStore _store = Substitute.For<IAttachmentStore>();
    private readonly IDocumentOwnerResolver _owners = Substitute.For<IDocumentOwnerResolver>();
    private readonly UploadDocumentHandler _handler;

    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly DocumentType _type = TestData.DocType(name: "Resume");
    private readonly IFormFile _file = TestData.FakeFile("cv.pdf", 2048);

    public UploadDocumentHandlerTests()
    {
        _handler = new UploadDocumentHandler(_repo, _store, _owners);

        // Happy path by default; each test breaks one thing
        _owners.ResolveAsync(DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
               .Returns((true, (int?)ContactType));
        _repo.GetTypeAsync(_type.DocumentTypeId, Arg.Any<CancellationToken>()).Returns(_type);
        _repo.IsTypeVisibleAsync(_type.DocumentTypeId, ContactType, Arg.Any<CancellationToken>()).Returns(true);
        _repo.OwnerHasDocumentOfTypeAsync(Arg.Any<DocumentOwnerType>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
             .Returns(false);
        _store.SaveAsync(_file, Arg.Any<CancellationToken>()).Returns(new AttachmentId(Guid.NewGuid()));
        _store.GetInfoAsync(Arg.Any<Document>(), Arg.Any<CancellationToken>())
              .Returns(new AttachmentInfo("cv.pdf", "application/pdf", 2048));
    }

    private Task<DocumentDto> Send(UploadDocumentRequest req) =>
        _handler.Handle(new UploadDocumentCommand(req), CancellationToken.None);

    private UploadDocumentRequest Request(string? name = null, string? number = null,
                                          DateTime? issue = null, DateTime? expire = null) =>
        TestData.Upload(_ownerId, _type.DocumentTypeId, _file, name, number, issue, expire);

    private async Task AssertNothingWasStored()
    {
        await _store.DidNotReceive().SaveAsync(Arg.Any<IFormFile>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().AddAsync(Arg.Any<Document>(), Arg.Any<DocumentLink>(), Arg.Any<CancellationToken>());
    }

    // ---------- failures before anything is stored ----------
    [Fact]
    public async Task Throws_NotFound_when_the_owner_does_not_exist()
    {
        _owners.ResolveAsync(DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
               .Returns((false, (int?)null));

        await Assert.ThrowsAsync<NotFoundException>(() => Send(Request()));
        await AssertNothingWasStored();
    }

    [Fact]
    public async Task Throws_when_the_document_type_does_not_exist()
    {
        _repo.GetTypeAsync(_type.DocumentTypeId, Arg.Any<CancellationToken>()).Returns((DocumentType?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => Send(Request()));

        Assert.Contains("Unknown", ex.Message);
        await AssertNothingWasStored();
    }

    [Fact]
    public async Task Throws_when_the_document_type_is_inactive()
    {
        _type.IsActive = false;

        await Assert.ThrowsAsync<BusinessRuleException>(() => Send(Request()));
        await AssertNothingWasStored();
    }

    [Fact]
    public async Task Throws_when_the_type_is_not_visible_for_the_contact_type()
    {
        _repo.IsTypeVisibleAsync(_type.DocumentTypeId, ContactType, Arg.Any<CancellationToken>()).Returns(false);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => Send(Request()));

        Assert.Contains("not available", ex.Message);
        await AssertNothingWasStored();
    }

    [Fact]
    public async Task Skips_the_visibility_check_when_the_owner_has_no_contact_type()
    {
        _owners.ResolveAsync(DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
               .Returns((true, (int?)null));

        await Send(Request());

        await _repo.DidNotReceive().IsTypeVisibleAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Throws_when_a_business_rule_is_broken_and_stores_nothing()
    {
        _type.RequiresName = true;

        await Assert.ThrowsAsync<BusinessRuleException>(() => Send(Request(name: "  ")));
        await AssertNothingWasStored();
    }

    [Fact]
    public async Task Throws_when_only_one_document_of_the_type_is_allowed_and_one_exists()
    {
        _type.MultipleAllowed = false;
        _repo.OwnerHasDocumentOfTypeAsync(DocumentOwnerType.Inspector, _ownerId, _type.DocumentTypeId, Arg.Any<CancellationToken>())
             .Returns(true);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => Send(Request()));

        Assert.Contains("Only one", ex.Message);
        await AssertNothingWasStored();
    }

    [Fact]
    public async Task Does_not_check_for_existing_documents_when_multiple_are_allowed()
    {
        _type.MultipleAllowed = true;

        await Send(Request());

        await _repo.DidNotReceive().OwnerHasDocumentOfTypeAsync(
            Arg.Any<DocumentOwnerType>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    // ---------- success ----------
    [Fact]
    public async Task Saves_the_document_and_the_owner_link()
    {
        Document? savedDocument = null;
        DocumentLink? savedLink = null;
        _ = _repo.AddAsync(Arg.Do<Document>(d => savedDocument = d), Arg.Do<DocumentLink>(l => savedLink = l), Arg.Any<CancellationToken>());

        await Send(Request(name: "  My CV  ", number: " 42 ",
                           issue: new DateTime(2026, 1, 15, 10, 30, 0), expire: new DateTime(2027, 1, 15)));

        Assert.NotNull(savedDocument);
        Assert.NotEqual(Guid.Empty, savedDocument!.DocumentId);
        Assert.Equal(_type.DocumentTypeId, savedDocument.DocumentTypeId);
        Assert.Equal("My CV", savedDocument.Name);          // trimmed
        Assert.Equal("42", savedDocument.Number);
        Assert.Equal("1", savedDocument.Version);
        Assert.NotNull(savedDocument.AttachmentId);
        Assert.Equal(new DateTime(2026, 1, 15), savedDocument.IssueDate);   // time dropped
        Assert.Equal(DateTimeKind.Utc, savedDocument.IssueDate!.Value.Kind);

        Assert.NotNull(savedLink);
        Assert.Equal(savedDocument.DocumentId, savedLink!.DocumentId);
        Assert.Equal((int)DocumentOwnerType.Inspector, savedLink.OwnerType);
        Assert.Equal(_ownerId, savedLink.OwnerId);
    }

    [Theory]
    [InlineData(false, false, DocumentStatuses.Active)]
    [InlineData(false, true, DocumentStatuses.PendingApproval)]
    [InlineData(true, false, DocumentStatuses.PendingReview)]
    public async Task Starts_with_the_status_the_type_requires(bool review, bool approval, string expectedStatus)
    {
        _type.RequiresReview = review;
        _type.RequiresApproval = approval;
        Document? saved = null;
        _ = _repo.AddAsync(Arg.Do<Document>(d => saved = d), Arg.Any<DocumentLink>(), Arg.Any<CancellationToken>());

        var dto = await Send(Request());

        Assert.Equal(expectedStatus, saved!.Status);
        Assert.Equal(expectedStatus, dto.Status);
    }

    [Fact]
    public async Task Returns_a_dto_with_the_type_name_and_the_stored_file_info()
    {
        var dto = await Send(Request(name: "My CV"));

        Assert.Equal("Resume", dto.DocumentTypeName);
        Assert.Equal("My CV", dto.Name);
        Assert.Equal("cv.pdf", dto.FileName);
        Assert.Equal(2048, dto.SizeBytes);
        Assert.Equal(_type.DocumentTypeId, dto.DocumentTypeId);
    }

    // ---------- rollback ----------
    [Fact]
    public async Task Deletes_the_stored_file_when_saving_the_rows_fails()
    {
        _repo.AddAsync(Arg.Any<Document>(), Arg.Any<DocumentLink>(), Arg.Any<CancellationToken>())
             .ThrowsAsync(new InvalidOperationException("db down"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => Send(Request()));

        await _store.Received(1).DeleteAsync(Arg.Any<Document>(), Arg.Any<CancellationToken>());
    }
}