using Inspection.Domain.Entities.InspectionNotification.Entities;
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

        public Guid? Id { get; set; }

        public AttachmentId? AttachmentId { get; set; }
        public Attachment? Attachment { get; set; }

        public Circulation? Circulation { get; set; }
        public Guid? CirculationId { get; set; }

    }
}
