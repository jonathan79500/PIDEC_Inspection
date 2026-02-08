using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class PersonAttachment
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public AttachmentId AttachmentId { get; set; }

        public Person Person { get; set; }
        public Attachment Attachment { get; set; }
    }
}
