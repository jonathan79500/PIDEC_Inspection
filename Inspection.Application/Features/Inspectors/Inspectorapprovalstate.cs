namespace Inspection.Application.Features.Inspectors;

// Mirrors the numbers already used by Inspector.QCApprovalState (int) / InspectorDto.QCApprovalState (int).
// Kept as a separate enum for readability in code; the stored column stays a plain int, so no DB change.
public enum InspectorApprovalState
{
    Rejected = 0,
    Approved = 1,
    NoAction = 2
}