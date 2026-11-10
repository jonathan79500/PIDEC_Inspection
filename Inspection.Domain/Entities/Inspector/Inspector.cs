using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class Inspector
    {
        public Guid InspectorId { get; set; }

        public Guid InspectionCompanyContactId { get; set; }
        public Contact InspectionCompanyContact { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Description { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public string QCReviewedBy { get; set; }
        public DateTime QCReviewedDate { get; set; }
        public string QCComment { get; set; }
        public int QCApprovalState { get; set; } = 2; // 0 = Rejected, 1 = Approved, 2 = NoAction
        public int VersionNumber { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpireDate { get; set; } = DateTime.UtcNow;

        public List<InspectorSkill> Skills { get; set; } = new();
        public List<InspectionNotificationInspector>? InspectionNotifications { get; set; }
    }
}
