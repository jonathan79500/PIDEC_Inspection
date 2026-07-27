using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs
{
    public class DocumentTypeVisibilityDto
    {
        public Guid DocumentTypeId { get; set; }
        public int ContactTypeCode { get; set; }
    }
}
