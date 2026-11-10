using System;
using System.Collections.Generic;


namespace Inspection.Application.DTOs.Inspector;

public class InspectorDto
{
    public Guid InspectorId { get; set; }
    public Guid InspectionCompanyContactId { get; set; }
    public string? InspectionCompanyContactName { get; set; }
    public bool IsActive { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Description { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    
    public string QCReviewedBy { get; set; }
    public DateTime QCReviewedDate { get; set; }
    public string QCComment { get; set; }

    public int QCApprovalState { get; set; }
    public int VersionNumber { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public List<InspectorSkillDto> Skills { get; set; } = new();
}
