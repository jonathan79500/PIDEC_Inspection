using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectionNotificationDataSummaryDto
    {
        public InspectionNotificationDataSummaryDto()
        {

        }
        public string RequestId { get; set; }
        public string CirculationId { get; set; }
        public string RevisionId { get; set; }

        public string SequenceNo { get; set; }
        public string InspectionNotificationId { get; set; }
        public string? RelatedProjectName { get; set; }
        public string? RelatedOrderName { get; set; }
        public string? RelatedOrderNumber { get; set; }
        public string? NotificationNumber { get; set; }
        public string? IssueDate { get; set; }
        public string? PersianIssueDate { get; set; }
        public string? InspectionStartDate { get; set; }
        public string? PersianInspectionStartDate { get; set; }
        public string? InspectionEndDate { get; set; }
        public string? PersianInspectionEndDate { get; set; }
        public string? NotificationTypes { get; set; }
        public string? NotificationSubjects { get; set;}
        public string? NotificationPIDECContacts { get; set; }
        public string? NotificationOwnerContacts { get; set; }
        public string? NotificationMCContacts { get; set; }

    }
}
