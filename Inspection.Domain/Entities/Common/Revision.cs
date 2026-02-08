using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class Revision
    {
        public Guid RevisionId { get; set; }
        public Guid? RequestId { get; set; }
        public int? VersionNumber { get; set; }
        public int? RevisionNumber { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public Contact? SubmittedBy { get; set; }
        public string? Comment { get; set; }
        public string? Status { get; set; } = "Pending";
        public bool? IsFinal { get; set; }
        public Request? Request { get; set; }

        public List<Circulation>? Circulations { get; set; } = new List<Circulation>();       
        
        //Navigate SubTypes like InspectionNotifiction or InspectionReport.....
        public List<InspectionNotification.Entities.InspectionNotification>? InspectionNotifications { get; set; }
        public List<InspectionReport.InspectionReport>? InspectionReports { get; set; }

    }
}
