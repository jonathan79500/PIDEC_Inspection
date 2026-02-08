using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using System;
using System.Collections.Generic;

namespace Inspection.Application.DTOs
{
    public class RequestDto
    {
        public RequestDto() { }

        public Guid? Id { get; set; }
        public ContactDto? CreatorContactPerson { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public Guid CurrentRevisionId { get; set; }

        public List<RevisionDto> Revisions { get; set; } = new List<RevisionDto>();
        public string? RequestType { get; set; }
        public string? CurrentStatus { get; set; }
    }

    
}
