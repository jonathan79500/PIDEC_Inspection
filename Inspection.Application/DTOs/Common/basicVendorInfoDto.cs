using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class BasicVendorInfoDto
    {
        //All Contacts with them ,vendor works (Supplier Contacts, Sub Supplier Contacts, Pidec Contacts,Owner Contacts)
       // public List<ContactDto> contacts { get; set; }       

        public BasicVendorInfoDto() { }
        public string? Name { get; set; }// = string.Empty;
        public string? UserName { get; set; } //= string.Empty;
        public int? DomainId { get; set; }
        public List<ProjectDto>? Projects { get; set; } = [];
        public List<OrderDto>? Orders { get; set; } = [];
        public List<InspectionTypeDto>? InspectionTypes { get; set; } = [];
        public List<InspectionSubjectDto>? InspectionSubjects { get; set; } = [];
        public List<OrderInspectionSubjectDto>? OrderInspectionSubjects { get; set; } = [];
        public List<CirculationDto>? Participated_Circulations { get; set; } = [];
        public List<OrderContactDto>? OrderContacts { get; set; } = [];
        public List<InspectionNotificationDto>? ReceivedInspectionNotifications { get; set; } = [];
        public List<OrderSettingExceptionDto>? OrderSettingsExceptions { get; set; } = [];
        public List<ContactDto> exceptedContacts { get; set; } = [];   
        public List<DocumentTypeDto> DocumentTypes { get; set; }
        public List<DocumentTypeVisibilityDto> DocumentTypeVisibilities { get; set; }
        public List<ProjectContactDto>? ProjectContacts { get; set; } = [];

    }
}
