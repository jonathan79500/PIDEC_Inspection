using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.InspectionRequest
{
    public class CreateInspectionNotificationDto :BaseDto
    {
        public string? Name { get; set; }
        public string? InspectionSubject { get; set; }
       // public Order? Order { get; set; }
        public Guid OrderId { get; set; }
        public string? City { get; set; }
        public string? NotificationNumber { get; set; }

        public Guid OwnerId { get; set; }
        public Guid InspectionTypeId { get; set; }
        public DateTime? ProposedDate { get; set; }
        public string? Place { get; set; }
        public Guid LocationId { get; set; }
        // public string? Address { get; set; }
        //public Contact? ContactPerson { get; set; }
        public DateTime? ContractShippingDueDate { get; set; }
        public DateTime? ActualShippingDueDate { get; set; }
        public string? Description { get; set; }
        
        //public Address? Location { get; set; }

        //public readonly List<ContactDto>? Contacts = new List<ContactDto>();


        //public readonly List<AttachmentDto>? Attachments = new List<AttachmentDto>();

        //public Vendor? Vendor { get; set; }
        //public InspectionTypeDto? InspectionTypeDto { get; set; }
        //public DateTime? ProposedDate { get; set; }
        //public string? Place { get; set; }
        //public string? Address { get; set; }
        //public Contact? ContactPerson { get; set; }
        //public DateTime? ContractShippingDueDate { get; set; }
        //public DateTime? ActualShippingDueDate { get; set; }
        //public string? Description { get; set; }
        //public Vendor? Owner { get; set; }
        //public Address? Location { get; set; }

        //public readonly List<ContactDto>? Contacts = new List<ContactDto>();


        //public readonly List<AttachmentDto>? Attachments = new List<AttachmentDto>();
    }
}
