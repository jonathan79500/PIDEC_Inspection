using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.InspectionReport.Enums;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.InspectionReport
{
    public class InspectionReportDto
    {
        public Guid InspectionReportId { get; set; }
        public string InspectionReportNumber { get; set; }
        public int CurrentStatus { get; set; }
        public ContactDto[] InspectorContacts { get; set; }
        public InspectionNotificationDto RelatedInspectionNotification { get; set; }
        public Guid RelatedInspectionNotificationId { get; set; }

        public List<InspectionReportAttachmentDto> InspectionReportAttachments { get; set; }

        public InspectionReportDto() { }
    }
}
