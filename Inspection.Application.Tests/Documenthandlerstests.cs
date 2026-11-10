using Inspection.Application.Contracts.Infrastructure;   // IAttachmentStore, AttachmentInfo: adjust to your namespace
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.Exceptions;
using Inspection.Application.Features.Documents;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using NSubstitute;

namespace Inspection.Application.Tests.Documents;

public class GetDocumentTypesHandlerTests
{
    private readonly IDocumentRepository _repo = Substitute.For<IDocumentRepository>();

    [Fact]
    public async Task Passes_the_contact_type_on_and_maps_the_allowed_extensions()
    {
        var type = TestData.DocType(name: "Grade", extensions: "pdf;docx", maxMb: 20);
        _repo.GetActiveTypesAsync(7, Arg.Any<CancellationToken>()).Returns(new List<DocumentType> { type });
        var handler = new GetDocumentTypesHandler(_repo);

        var result = await handler.Handle(new GetDocumentTypesQuery(7), CancellationToken.None);

        var dto = Assert.Single(result);
        Assert.Equal("Grade", dto.DocumentTypeName);
        Assert.Equal(20, dto.MaxSizeMB);
        Assert.Equal(new[] { ".pdf", ".docx" }, dto.AllowedExtensions);
    }

    [Fact]
    public async Task Any_file_type_becomes_an_empty_extension_list()
    {
        _repo.GetActiveTypesAsync(null, Arg.Any<CancellationToken>())
             .Returns(new List<DocumentType> { TestData.DocType(extensions: "*") });
        var handler = new GetDocumentTypesHandler(_repo);

        var result = await handler.Handle(new GetDocumentTypesQuery(null), CancellationToken.None);

        Assert.Empty(result.Single().AllowedExtensions);
    }
}

public class GetDocumentsHandlerTests
{
    private readonly IDocumentRepository _repo = Substitute.For<IDocumentRepository>();
    private readonly IAttachmentStore _store = Substitute.For<IAttachmentStore>();

    [Fact]
    public async Task Combines_document_type_name_and_file_info_and_marks_missing_files()
    {
        var ownerId = Guid.NewGuid();
        var withFile = new Document { DocumentId = Guid.NewGuid(), Name = "A", Status = "Active", Version = "1" };
        var withoutFile = new Document { DocumentId = Guid.NewGuid(), Name = "B", Status = "Active", Version = "1" };

        _repo.GetByOwnerAsync(DocumentOwnerType.Inspector, ownerId, Arg.Any<CancellationToken>())
             .Returns(new List<DocumentWithType> { new(withFile, "Resume"), new(withoutFile, "Grade") });
        _store.GetInfoAsync(withFile, Arg.Any<CancellationToken>()).Returns(new AttachmentInfo("a.pdf", "application/pdf", 10));
        _store.GetInfoAsync(withoutFile, Arg.Any<CancellationToken>()).Returns((AttachmentInfo?)null);
        var handler = new GetDocumentsHandler(_repo, _store);

        var result = await handler.Handle(new GetDocumentsQuery(DocumentOwnerType.Inspector, ownerId), CancellationToken.None);

        Assert.Equal(2, result.Count);

        Assert.Equal("Resume", result[0].DocumentTypeName);
        Assert.Equal("a.pdf", result[0].FileName);
        Assert.Equal(10, result[0].SizeBytes);
        Assert.Equal(withFile.DocumentId, result[0].DocumentId);

        Assert.Equal("Grade", result[1].DocumentTypeName);
        Assert.Equal("(file missing)", result[1].FileName);
        Assert.Equal(0, result[1].SizeBytes);
    }

    [Fact]
    public async Task Returns_an_empty_list_when_the_owner_has_no_documents()
    {
        _repo.GetByOwnerAsync(Arg.Any<DocumentOwnerType>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
             .Returns(new List<DocumentWithType>());
        var handler = new GetDocumentsHandler(_repo, _store);

        var result = await handler.Handle(new GetDocumentsQuery(DocumentOwnerType.Inspector, Guid.NewGuid()), CancellationToken.None);

        Assert.Empty(result);
    }
}

public class DownloadDocumentHandlerTests
{
    private readonly IDocumentRepository _repo = Substitute.For<IDocumentRepository>();
    private readonly IAttachmentStore _store = Substitute.For<IAttachmentStore>();
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Document _document = new() { DocumentId = Guid.NewGuid(), Version = "1" };

    private DownloadDocumentQuery Query() =>
        new(_document.DocumentId, DocumentOwnerType.Inspector, _ownerId);

    [Fact]
    public async Task Throws_NotFound_when_the_document_is_not_linked_to_the_owner()
    {
        _repo.GetForOwnerAsync(_document.DocumentId, DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
             .Returns((Document?)null);
        var handler = new DownloadDocumentHandler(_repo, _store);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(Query(), CancellationToken.None));
    }

    [Fact]
    public async Task Throws_NotFound_when_the_file_is_missing_from_the_store()
    {
        _repo.GetForOwnerAsync(_document.DocumentId, DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
             .Returns(_document);
        _store.OpenAsync(_document, Arg.Any<CancellationToken>())
              .Returns(Task.FromResult<(Stream Content, AttachmentInfo Info)?>(null));
        var handler = new DownloadDocumentHandler(_repo, _store);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(Query(), CancellationToken.None));

        Assert.Contains("missing", ex.Message);
    }

    [Fact]
    public async Task Returns_the_stream_content_type_and_file_name()
    {
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        _repo.GetForOwnerAsync(_document.DocumentId, DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
             .Returns(_document);
        _store.OpenAsync(_document, Arg.Any<CancellationToken>())
              .Returns(Task.FromResult<(Stream Content, AttachmentInfo Info)?>(
                  (stream, new AttachmentInfo("cv.pdf", "application/pdf", 3))));
        var handler = new DownloadDocumentHandler(_repo, _store);

        var file = await handler.Handle(Query(), CancellationToken.None);

        Assert.Same(stream, file.Content);
        Assert.Equal("application/pdf", file.ContentType);
        Assert.Equal("cv.pdf", file.FileName);
    }
}

public class DeleteDocumentHandlerTests
{
    private readonly IDocumentRepository _repo = Substitute.For<IDocumentRepository>();
    private readonly IAttachmentStore _store = Substitute.For<IAttachmentStore>();
    private readonly Guid _documentId = Guid.NewGuid();
    private readonly Guid _ownerId = Guid.NewGuid();

    private Task Send() =>
        new DeleteDocumentHandler(_repo, _store)
            .Handle(new DeleteDocumentCommand(_documentId, DocumentOwnerType.Inspector, _ownerId), CancellationToken.None);

    [Fact]
    public async Task Throws_NotFound_when_the_owner_has_no_such_document()
    {
        _repo.UnlinkAsync(_documentId, DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
             .Returns(new UnlinkResult(false, null));

        await Assert.ThrowsAsync<NotFoundException>(Send);
        await _store.DidNotReceive().DeleteAsync(Arg.Any<Document>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Deletes_the_file_when_no_other_owner_uses_the_document()
    {
        var orphan = new Document { DocumentId = _documentId, Version = "1" };
        _repo.UnlinkAsync(_documentId, DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
             .Returns(new UnlinkResult(true, orphan));

        await Send();

        await _store.Received(1).DeleteAsync(orphan, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Keeps_the_file_when_another_owner_still_uses_the_document()
    {
        _repo.UnlinkAsync(_documentId, DocumentOwnerType.Inspector, _ownerId, Arg.Any<CancellationToken>())
             .Returns(new UnlinkResult(true, null));

        await Send();

        await _store.DidNotReceive().DeleteAsync(Arg.Any<Document>(), Arg.Any<CancellationToken>());
    }
}