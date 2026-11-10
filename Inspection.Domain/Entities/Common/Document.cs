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


        // OwnerType and OwnerId removed — ownership now lives in DocumentLink

        public Guid DocumentTypeId { get; set; }

        // NEW: filled when the DocumentType has RequiresName / RequiresNumber
        public string? Name { get; set; }
        public string? Number { get; set; }

        public string? Status { get; set; }          // PendingReview | PendingApproval | Active
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }  // null instead of new DateTime()
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Version { get; set; } = "1";
    }
}
