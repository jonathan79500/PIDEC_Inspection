using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class DocumentLink
    {
        public Guid DocumentId { get; set; }
        public int OwnerType { get; set; }     // (int)DocumentOwnerType
        public Guid OwnerId { get; set; }
    }
}
