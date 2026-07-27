using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs
{
    public class DocumentDto
    {

        public Guid DocumentId { get; set; } = Guid.NewGuid();
        public Guid? AttachmentId { get; set; }
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
