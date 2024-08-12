using Inspection.Application.DTOs.Common;
using System;
using Inspection.Domain.Entities;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System.Collections.Generic;


namespace Inspection.Application.DTOs.InspectionRequest
{
    public class InspectionNotificationDto : BaseDto
    {
        public string Name { get; set; }
        public string InspectionSubject { get; private set; }
        public Order? Order { get; set; }
        public string? City { get; set; }
        public string? NotificationNumber { get; set; }
        public Vendor? Vendor { get; set; }
        public InspectionTypeDto? InspectionTypeDto { get; set; }
        public DateTime? ProposedDate { get; set; }
        public string? Place { get; set; }
        public string? Address { get; set; }
        public Contact? ContactPerson { get; set; }
        public DateTime? ContractShippingDueDate { get; set; }
        public DateTime? ActualShippingDueDate { get; set; }
        public string? Description { get; set; }
        public Vendor Owner { get; private set; }        
        public Address? Location { get; private set; }              

        public readonly List<Contact>? Contacts = new List<Contact>();


        public readonly List<Attachment>? Attachments = new List<Attachment>();

    }
}
