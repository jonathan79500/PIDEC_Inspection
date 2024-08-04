using System;


namespace Inspection.Application.Features.InspectionNotifications.Common.CreateInspectionNotificationDto;

public class CreateInspectionNotificationDto 
{
    public string Name { get; set; }
    public int Order_Id { get; set; }    
    public int InspectionType_Id { get; set; }
    public DateTime ProposedDate { get; set; }
    public string Place { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
}
