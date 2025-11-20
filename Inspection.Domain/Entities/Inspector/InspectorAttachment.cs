using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Inspector.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class InspectorAttachment:BaseDomainEntity<Guid>
    {
        public InspectorAttachment() { }       
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Content { get; set; }
        public string Size {  get; set; }
        public InspectorAttachmentTypes AttachmentTypes { get; set; }
        public Inspector Inspector { get; set; }
    }
}
