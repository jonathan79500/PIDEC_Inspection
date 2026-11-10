using System;
using System.Collections.Generic;

namespace Inspection.Application.DTOs;

public class CirculationHistoryDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }

    public string? PerformerName { get; set; }      // Contacts.Name, via PerformerContactPersonContactId
    public int? PerformerContactType { get; set; }
    public int? SequenceNo { get; set; }

    public int? ActionType { get; set; }             // 1 = Approve, 2 = Reject, 3 = Backward (see onAction() in the Angular form)
    public int? QCActionType { get; set; }
    public int? PreviousActionType { get; set; }

    public string? PerformedActionDate { get; set; }            // the stored display string
    public DateTime? PerformedActionDateGregorian { get; set; } // year 1 (0001-01-01) means "no action yet"

    public string? Comment { get; set; }
    public string? CreatorComment { get; set; }

    // Internal correspondence under this circulation (same shape, one level deep in current usage,
    // though ParentId technically supports further nesting - see BuildTree in the repository).
    public List<CirculationHistoryDto> ChildCirculations { get; set; } = new();
}

public class RevisionHistoryDto
{
    public Guid RevisionId { get; set; }
    public int? VersionNumber { get; set; }
    public int? RevisionNumber { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string? SubmittedByName { get; set; }   // Contacts.Name, via SubmittedByContactId
    public string? Comment { get; set; }
    public string? Status { get; set; }
    public bool? IsFinal { get; set; }

    // Top-level circulations for this revision (ParentId == null), ordered by SequenceNo.
    public List<CirculationHistoryDto> Circulations { get; set; } = new();
}