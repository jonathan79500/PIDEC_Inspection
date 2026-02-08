using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport.Enums;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionReport
{
    public class InspectionReport:AggregateRoot<InspectionReportId>
    {        
        public InspectionReportId  InspectionReportId { get; set; }
        public Guid RevisionId { get; set; }
        public Revision Revision { get; set; } = null!;
        public InspectionReportNumber InspectionReportNumberParts { get; set; }
        public string InspectionReportNumber { get; set; }
        public InspectionReportStates CurrentStatus { get; set; }
        public List<Inspector.Inspector> Inspectors { get; set; }
        public ReportType ReportType { get; set; }
        public string ReportNotes   { get; set; }
        public DateTime IssueDate { get; set; }
        public string PersianIssueDate { get; set; }
        public List<InspectionReportPerson> InspectionPersons { get; set; }
        public Contact Creator { get; set; }
        public Guid CreatedBy { get; set; } // Creator User ID (Creator Is Inspector SecondPart/ThirdPart)
        
        [ForeignKey(nameof(RelatedInspectionNotificationId))]
        public InspectionNotification.Entities.InspectionNotification RelatedInspectionNotification { get; set; }
        public InspectionNotificationId RelatedInspectionNotificationId { get; set; }
        public Guid RequestIdOfRelatedInspectionNotification { get; set; }

        public List<InspectionReportAttachment> Attachments { get; set; }

        public InspectionReport() { }
    }
}
