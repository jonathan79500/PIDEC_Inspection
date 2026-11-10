using Inspection.Application.DTOs;
using Inspection.Application.Exceptions;      // adjust: wherever NotFoundException / BusinessRuleException live
using Inspection.Domain.Entities.Common;
using System;
using System.IO;
using System.Linq;

namespace Inspection.Application.Features.Documents;

// The business rules for documents. Pure code: no database, no disk, easy to unit test.
public static class DocumentRules
{
    // The status a new document starts with
    public static string InitialStatus(DocumentType type) =>
        type.RequiresReview ? DocumentStatuses.PendingReview
        : type.RequiresApproval ? DocumentStatuses.PendingApproval
        : DocumentStatuses.Active;

    // Checks name / number / dates / file against the document type. Throws BusinessRuleException.
    public static void ValidateUpload(DocumentType type, string? name, string? number, UploadDocumentRequest req)
    {
        if (type.RequiresName && string.IsNullOrEmpty(name))
            throw new BusinessRuleException("A name is required for this document type.");
        if (type.RequiresNumber && string.IsNullOrEmpty(number))
            throw new BusinessRuleException("A number is required for this document type.");

        if (req.IssueDate is not null && req.ExpireDate is not null && req.ExpireDate < req.IssueDate)
            throw new BusinessRuleException("The expiry date cannot be before the issue date.");

        var file = req.File;
        if (file.Length == 0)
            throw new BusinessRuleException("The file is empty.");
        if (file.Length > (long)type.MaxSizeMB * 1024 * 1024)
            throw new BusinessRuleException($"The file is larger than {type.MaxSizeMB} MB.");

        var allowed = ParseExtensions(type.AllowedExtensions);
        if (allowed.Length > 0 && !allowed.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            throw new BusinessRuleException($"Allowed file types: {string.Join(", ", allowed)}.");
    }

    // "*" (or empty) = any file type. Otherwise ".pdf,.docx" / "pdf;docx" / "*.pdf" all work.
    public static string[] ParseExtensions(string? allowed)
    {
        if (string.IsNullOrWhiteSpace(allowed)) return Array.Empty<string>();

        var parts = allowed
            .Split(new[] { ',', ';', ' ', '|' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(p => p.ToLowerInvariant())
            .ToList();

        if (parts.Contains("*") || parts.Contains("*.*")) return Array.Empty<string>();

        return parts
            .Select(p => p.TrimStart('*'))
            .Select(p => p.StartsWith('.') ? p : "." + p)
            .Distinct()
            .ToArray();
    }

    // Date-only values are stored as UTC midnight (no time-zone shift)
    public static DateTime? AsUtcDate(DateTime? d) =>
        d is null ? null : DateTime.SpecifyKind(d.Value.Date, DateTimeKind.Utc);
}