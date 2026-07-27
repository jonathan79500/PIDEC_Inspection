using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs
{
    public class DocumentLinkDto
    {
        public Guid DocumentId { get; set; }
        public int OwnerType { get; set; }
        public Guid OwnerId { get; set; }
    }
}
