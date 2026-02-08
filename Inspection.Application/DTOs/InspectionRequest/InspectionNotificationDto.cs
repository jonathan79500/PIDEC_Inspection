using Inspection.Application.DTOs.Common;
using System;
using Inspection.Domain.Entities;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System.Collections.Generic;
using Inspection.Domain.Entities.InspectionNotification.Enums;


namespace Inspection.Application.DTOs.InspectionRequest
{
    public class InspectionNotificationDto //: BaseDto
    {
        public InspectionNotificationDto() { }

        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public OrderDto? Order { get; set; }
        public string? OrderName { get; set; }
        public string? EquipmentTagNo { get; set; }
        public Guid RevisionId { get; set; }
        public RevisionDto Revision { get; set; } = null!;
        public string? NotificationNumber { get; set; }
        public DateTime? ProposedDate { get; set; }
        public List<AddressDto>? Locations { get; set; }

        public DateTime? RequestInspectionDateStart { get; set; } = DateTime.Now;
        public DateTime? RequestInspectionDateEnd { get; set; } = DateTime.Now;
        public string? RequestInspectionPersianDateStart { get; set; } = string.Empty;
        public string? RequestInspectionPersianDateEnd { get; set; } = string.Empty;
        public int? RequestInspectionDueDate { get; set; } = 0;
        public string? InspectionTimeComment { get; set; } = string.Empty;
        public string? InspectionAction { get; set; } = string.Empty;
        public string? InspectionActionComment { get; set; } = string.Empty;

        public DateTime? IssueDate { get; set; }
        public string? PersianIssueDate { get; set; }
        public string? Description { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }

        public List<InspectionTypeDto>? InspectionTypes { get; set; }
        public List<InspectionSubjectDto>? InspectionSubjects { get; set; }
        public List<ContactDto>? Contacts { get; set; }
        public ContactType? ContactType { get; private set; }
        //public List<AttachmentDto>? Attachments { get; set; }
        //public List<CirculationDto>? Circulations { get; set; }

    }
}
