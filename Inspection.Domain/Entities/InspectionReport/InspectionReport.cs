using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport.Enums;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionReport
{
    public class InspectionReport:AggregateRoot<Guid>
    {        
        public InspectionReportId  InspectionReportId { get; set; }
        public InspectionReportNumber InspectionReportNumber { get; set; }
        public InspectionReportStates CurrentStatus { get; set; }
        public Contact[] InspectorContacts { get; set; }
        
        [ForeignKey(nameof(RelatedInspectionNotificationId))]
        public InspectionNotification.Entities.InspectionNotification RelatedInspectionNotification { get; set; }
        public InspectionNotificationId RelatedInspectionNotificationId { get; set; }

        public List<InspectionReportAttachment> inspectionReportAttachments { get; set; }

        public InspectionReport() { }
    }
}
