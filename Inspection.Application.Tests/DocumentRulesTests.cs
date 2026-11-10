using Inspection.Application.Exceptions;

using Inspection.Application.Features.Documents;
using Inspection.Domain.Entities.Common;

namespace Inspection.Application.Tests.Documents;

public class DocumentRulesTests
{
    // ---------- InitialStatus ----------
    [Theory]
    [InlineData(true, true, DocumentStatuses.PendingReview)]      // review wins over approval
    [InlineData(true, false, DocumentStatuses.PendingReview)]
    [InlineData(false, true, DocumentStatuses.PendingApproval)]
    [InlineData(false, false, DocumentStatuses.Active)]
    public void InitialStatus_follows_review_and_approval_flags(bool review, bool approval, string expected)
    {
        var type = TestData.DocType(review: review, approval: approval);

        Assert.Equal(expected, DocumentRules.InitialStatus(type));
    }

    // ---------- ParseExtensions ----------
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("*", "")]                       // any file type
    [InlineData("*.*", "")]
    [InlineData(".pdf,.docx", ".pdf,.docx")]
    [InlineData("pdf;docx", ".pdf,.docx")]      // no dots, other separator
    [InlineData("*.PDF, *.jpg", ".pdf,.jpg")]   // wildcard, case, spaces
    [InlineData(".pdf .pdf|.PDF", ".pdf")]      // duplicates
    public void ParseExtensions_normalizes_the_list(string? input, string expectedJoined)
    {
        var result = DocumentRules.ParseExtensions(input);

        Assert.Equal(expectedJoined, string.Join(",", result));
    }

    // ---------- ValidateUpload ----------
    [Fact]
    public void ValidateUpload_accepts_a_valid_request()
    {
        var type = TestData.DocType(extensions: ".pdf", maxMb: 5);
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId, TestData.FakeFile("cv.PDF", 2048));

        var ex = Record.Exception(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Null(ex);
    }

    [Fact]
    public void ValidateUpload_requires_a_name_when_the_type_says_so()
    {
        var type = TestData.DocType(requiresName: true);
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId);

        var ex = Assert.Throws<BusinessRuleException>(() => DocumentRules.ValidateUpload(type, "", null, req));

        Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateUpload_requires_a_number_when_the_type_says_so()
    {
        var type = TestData.DocType(requiresNumber: true);
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId);

        var ex = Assert.Throws<BusinessRuleException>(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Contains("number", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateUpload_rejects_an_expiry_before_the_issue_date()
    {
        var type = TestData.DocType();
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId,
            issue: new DateTime(2026, 5, 1), expire: new DateTime(2026, 4, 30));

        var ex = Assert.Throws<BusinessRuleException>(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Contains("expiry", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateUpload_allows_an_expiry_on_the_issue_date()
    {
        var type = TestData.DocType();
        var day = new DateTime(2026, 5, 1);
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId, issue: day, expire: day);

        var ex = Record.Exception(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Null(ex);
    }

    [Fact]
    public void ValidateUpload_rejects_an_empty_file()
    {
        var type = TestData.DocType();
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId, TestData.FakeFile(length: 0));

        var ex = Assert.Throws<BusinessRuleException>(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Contains("empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateUpload_rejects_a_file_over_the_size_limit()
    {
        var type = TestData.DocType(maxMb: 1);
        var oneByteTooBig = 1024L * 1024 + 1;
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId, TestData.FakeFile(length: oneByteTooBig));

        var ex = Assert.Throws<BusinessRuleException>(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Contains("1 MB", ex.Message);
    }

    [Fact]
    public void ValidateUpload_accepts_a_file_exactly_at_the_size_limit()
    {
        var type = TestData.DocType(maxMb: 1);
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId, TestData.FakeFile(length: 1024L * 1024));

        var ex = Record.Exception(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Null(ex);
    }

    [Fact]
    public void ValidateUpload_rejects_an_extension_that_is_not_allowed()
    {
        var type = TestData.DocType(extensions: ".pdf,.docx");
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId, TestData.FakeFile("virus.exe"));

        var ex = Assert.Throws<BusinessRuleException>(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Contains(".pdf", ex.Message);
    }

    [Fact]
    public void ValidateUpload_extension_check_ignores_case()
    {
        var type = TestData.DocType(extensions: ".pdf");
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId, TestData.FakeFile("SCAN.PDF"));

        var ex = Record.Exception(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Null(ex);
    }

    [Fact]
    public void ValidateUpload_accepts_any_extension_when_the_type_allows_everything()
    {
        var type = TestData.DocType(extensions: "*");
        var req = TestData.Upload(Guid.NewGuid(), type.DocumentTypeId, TestData.FakeFile("anything.xyz"));

        var ex = Record.Exception(() => DocumentRules.ValidateUpload(type, null, null, req));

        Assert.Null(ex);
    }

    // ---------- AsUtcDate ----------
    [Fact]
    public void AsUtcDate_returns_null_for_null()
    {
        Assert.Null(DocumentRules.AsUtcDate(null));
    }

    [Fact]
    public void AsUtcDate_drops_the_time_and_marks_the_value_as_utc()
    {
        var result = DocumentRules.AsUtcDate(new DateTime(2026, 5, 1, 23, 59, 0, DateTimeKind.Local));

        Assert.Equal(new DateTime(2026, 5, 1), result!.Value);
        Assert.Equal(DateTimeKind.Utc, result.Value.Kind);
    }
}