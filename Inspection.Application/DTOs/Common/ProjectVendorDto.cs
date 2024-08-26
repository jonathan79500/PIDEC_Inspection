using Inspection.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class ProjectVendorDto:BaseDto
    {
        public Guid ProjectId { get; set; }
        public Guid VendorId { get;  set; }
        public ProjectDto ProjectDto { get; set; }
        public VendorDto VendorDto { get; set; }
    }
}
