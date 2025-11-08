using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectionReportAttachmentDto
    {
        public Guid Id { get; set; }  
        public Guid InspectionReportId { get; set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public byte[] Content { get; private set; }
    }
}
