using Inspection.Application.DTOs.Common;
using Inspection.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspection.Application.DTOs
{
    public class OrderDto:BaseDto
    {
        public OrderDto() { }

        //public Guid Id { get; set; }
        public ProjectDto? Project { get; set; }= new ProjectDto();
        //public string ProjectId { get; set; }
        public string? No { get; set; } = string.Empty;
        public string? Title { get; set; }=string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Version { get; set; } = string.Empty;
        public string? EquipmentTagNo { get; set; }=string.Empty;
        public string? EdmsId { get; set; } = string.Empty;
        public string? ITP_NO { get; set; } = string.Empty;
        public bool? IsAccessibleByEmployer { get; set; } 
        public string? VendorCompanyName { get; set; }
        public bool? HasReadPermission { get; set; }
        public List<VendorDocumentDto>? VendorDocuments { get; set; } = [];
        public List<InspectionSubjectDto>? InspectionSubjects { get; set; } = [];
        public List<ContactDto>? Contacts {  get; set; } = [];
        public List<ContactDto>? ExceptionContacts { get; set; }        
    }
}
