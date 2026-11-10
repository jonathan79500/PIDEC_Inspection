using Inspection.Application.DTOs;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Inspection.Application.Tests;

internal static class TestData
{
    public static DocumentType DocType(
        string name = "Resume", bool active = true, bool multiple = false,
        bool review = false, bool approval = false,
        bool requiresName = false, bool requiresNumber = false,
        string extensions = "*", int maxMb = 10) => new()
        {
            DocumentTypeId = Guid.NewGuid(),
            DocumentTypeName = name,
            DocumentTypeCode = 1,
            IsActive = active,
            MultipleAllowed = multiple,
            RequiresReview = review,
            RequiresApproval = approval,
            RequiresName = requiresName,
            RequiresNumber = requiresNumber,
            AllowedExtensions = extensions,
            MaxSizeMB = maxMb
        };

    public static IFormFile FakeFile(string fileName = "cv.pdf", long length = 1024)
    {
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns(fileName);
        file.Length.Returns(length);
        return file;
    }

    public static UploadDocumentRequest Upload(
        Guid ownerId, Guid documentTypeId, IFormFile? file = null,
        string? name = null, string? number = null,
        DateTime? issue = null, DateTime? expire = null) => new()
        {
            File = file ?? FakeFile(),
            OwnerType = DocumentOwnerType.Inspector,
            OwnerId = ownerId,
            DocumentTypeId = documentTypeId,
            Name = name,
            Number = number,
            IssueDate = issue,
            ExpireDate = expire
        };
}