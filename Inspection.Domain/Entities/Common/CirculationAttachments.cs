using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class CirculationAttachments
    {
        public CirculationAttachments() { }

        public AttachmentId AttachmentId { get; set; }

        public Circulation Circulation { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public byte[] Content { get; set; }
        public int Version { get; set; }
        public string edmsId { get; set; }
        public string PDFSize { get; set; }
    }
}
