using Inspection.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class VendorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ProjectDto Project { get; set; }
        public IEnumerable<ContactDto> ContactsDto { get; set; }
        public IEnumerable<ProjectVendorDto> ProjectsDto { get; set; }     
    }
}
