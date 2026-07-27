using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class DocumentTypeVisibility
    {
        public Guid DocumentTypeId { get; set; }
        public int ContactTypeCode { get; set; }
    }
}
