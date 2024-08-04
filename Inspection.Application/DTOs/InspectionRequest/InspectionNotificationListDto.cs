using Inspection.Application.DTOs.Common;
using Inspection.Domain;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspection.Application.DTOs.InspectionRequest
{
    public class InspectionNotificationListDto : BaseDto
    {
        //public string Name { get; set; }
        //public int Order_Id { get; set; }
        //public string Order_NO { get; set; }
        //public string Order_Title { get; set; }
        //public int InspectionType_Id { get; set; }
        //public DateTime ProposedDate { get; set; }


        public string Name { get;  set; }
        public Order? Order { get;  set; }
        public string? City { get;  set; }
        public string? NotificationNumber { get;  set; }
        public Vendor? Vendor { get;  set; }
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
