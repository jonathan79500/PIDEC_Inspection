using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inspection.Application.DTOs;

// ---------- response: full record, for a document-types management screen ----------
public class DocumentTypeAdminDto
{
    public Guid DocumentTypeId { get; set; }
    public string DocumentTypeName { get; set; } = "";
    public int DocumentTypeCode { get; set; }
    public bool RequiresApproval { get; set; }
    public bool RequiresReview { get; set; }
    public bool RequiresName { get; set; }
    public bool RequiresNumber { get; set; }
    public bool MultipleAllowed { get; set; }
    public bool IsActive { get; set; }
    public string AllowedExtensions { get; set; } = "*";   // raw pattern, as stored (admin edits this directly)
    public int MaxSizeMB { get; set; }

    // Which contact types may use this document type (DocumentTypeVisibility). Empty = visible to nobody yet.
    public List<int> VisibleToContactTypes { get; set; } = new();

    public int DocumentCount { get; set; }   // how many documents currently use this type
}

// ---------- request: create / update ----------
public class DocumentTypeSaveDto
{
    [Required, StringLength(150)]
    public string DocumentTypeName { get; set; } = "";

    [Required]
    public int DocumentTypeCode { get; set; }

    public bool RequiresApproval { get; set; }
    public bool RequiresReview { get; set; }
    public bool RequiresName { get; set; }
    public bool RequiresNumber { get; set; }
    public bool MultipleAllowed { get; set; } = true;
    public bool IsActive { get; set; } = true;

    [Required, StringLength(200)]
    public string AllowedExtensions { get; set; } = "*";

    [Range(1, 10240)]
    public int MaxSizeMB { get; set; } = 10;

    public List<int> VisibleToContactTypes { get; set; } = new();
}