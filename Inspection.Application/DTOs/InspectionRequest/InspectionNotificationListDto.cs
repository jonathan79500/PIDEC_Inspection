using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.Orders;
using System;


namespace Inspection.Application.DTOs.InspectionRequest
{
    public class InspectionNotificationListDto : BaseDto
    {
    

        public string Name { get;  set; }
        public Order? Order { get;  set; }
        public string? City { get;  set; }
        public string? NotificationNumber { get;  set; }     
        public InspectionType? InspectionType { get; set; }
        public DateTime? ProposedDate { get;  set; }
        public string? Place { get;  set; }
        public string? Address { get;  set; }
        public Contact? ContactPerson { get;  set; }
        public DateTime? ContractShippingDueDate { get; set; }
        public DateTime? ActualShippingDueDate { get; set; }
        public string? Description { get; set; }

    }
}
