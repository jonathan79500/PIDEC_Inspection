using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionReport
{
    public class InspectionReportAttachment
    {
        public AttachmentId InspectionReportAttachmentId { get; set; }
        public InspectionReportId InspectionReportId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public byte[] Content { get; set; }
        public int Version { get; set; }
        public string edmsId { get; set; }
        public string PDFSize { get; set; }

        public InspectionReportAttachment(AttachmentId inspectionReportAttachmentId) : base()
        {
            InspectionReportAttachmentId = inspectionReportAttachmentId;
        }

        private InspectionReportAttachment(string name, string description, byte[] content, int version, string edmsId, string pdfSize) : base()
        {
            AttachmentId.Create(Guid.NewGuid());
            Name = name;
            Description = description;
            Content = content;
            Version = version;
            edmsId = edmsId;
            PDFSize = pdfSize;
        }

        public InspectionReportAttachment()
        {
        }

    }
}
