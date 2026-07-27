using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs
{
    public class DocumentTypeDto
    {
        public Guid DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public int DocumentTypeCode { get; set; }
        public bool RequiresApproval { get; set; }
        public bool RequiresReview { get; set; }
        public bool RequiresName { get; set; }
        public bool RequiresNumber { get; set; }
        public bool MultipleAllowed { get; set; }
        public bool IsActive { get; set; }
        public string AllowedExtensions { get; set; } = "*";
        public int MaxSizeMB { get; set; } = 1024;
    }
}
