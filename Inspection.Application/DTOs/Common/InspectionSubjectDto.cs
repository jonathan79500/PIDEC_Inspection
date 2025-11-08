using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectionSubjectDto
    {
        public Guid? Id { get; set; }=Guid.Empty;
        public string? Name { get; set; }
    }
}
