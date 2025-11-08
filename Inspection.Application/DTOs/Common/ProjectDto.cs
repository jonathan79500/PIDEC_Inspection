using Inspection.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class ProjectDto
    {
        public ProjectDto() { }

        public Guid? Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Number { get; set; } = string.Empty;
        public string? FSId { get; set; } = string.Empty;

        public List<ContactDto>? Contacts { get; set; }
        public List<ContactDto>? OwnerContacts { get; set; }
        public List<ContactDto>? MCContacts { get; set; }
        public List<ContactDto>? PidecInspectors { get; set; } //Second Part Inspectors
        public List<ContactDto>? TPInspectors { get; set; }//Third Part Inspectors

        public DateTime? CreatedDate { set; get; }
        public DateTime? ModifiedDate { set; get; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
