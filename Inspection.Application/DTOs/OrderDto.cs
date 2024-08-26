using Inspection.Application.DTOs.Common;
using Inspection.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspection.Application.DTOs
{
    public class OrderDto:BaseDto
    {
        public Guid Id { get; set; }
        public ProjectDto ProjectDto { get; set; }
        public string No { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public readonly List<VendorDocumentDto> vendorDocuments = new List<VendorDocumentDto>();
    }
}
