using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport.Enums;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionReportAttachmentId = Inspection.Domain.Entities.ValueObjects.InspectionReportAttachmentId;

namespace Inspection.Domain.Entities.InspectionReport
{
    public class InspectionReportAttachment
    {
        public InspectionReportAttachmentId InspectionReportAttachmentId { get; set; }
        public InspectionReportId InspectionReportId { get; set; }
        public InspectionReport InspectionReport { get; set; }
    
        public ReportAttachmentType ReportAttachmentType { get; set; }
       
        public AttachmentId? AttachmentId { get; set; }
        public Attachment? Attachment { get; set; }

       

        public InspectionReportAttachment(InspectionReportAttachmentId inspectionReportAttachmentId) : base()
        {
            InspectionReportAttachmentId = inspectionReportAttachmentId;
        }

        private InspectionReportAttachment(string name, string description, byte[] content, int version, string edmsId, string pdfSize) : base()
        {
            AttachmentId.Create(Guid.NewGuid());
          
        }

        public InspectionReportAttachment()
        {
        }

    }
}
