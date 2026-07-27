using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class Document
    {
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public AttachmentId? AttachmentId { get; set; }
        public string? OwnerType { get; set; } = string.Empty;
        public Guid? OwnerId { get; set; } 
        public Guid DocumentTypeId { get; set; }
        public string? Status { get; set; } = string.Empty;      
        public string? ApprovedBy { get; set; } = string.Empty;
        public DateTime? ApprovedDate { get; set; } = new DateTime();
        public DateTime? IssueDate { get; set; } = new DateTime();
        public DateTime? ExpireDate { get; set; } = new DateTime();
        public string Version { get; set; } = "0";
    }
}
