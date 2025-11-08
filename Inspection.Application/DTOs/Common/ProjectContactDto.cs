using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class ProjectContactDto
    {
        public Guid ProjectId { get; set; }
        public ContactDto Contact { get; set; }
    }
}
