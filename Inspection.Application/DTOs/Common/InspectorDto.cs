using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectorDto
    {
        public InspectorDto()
        {
            
        }
        public Guid? ContactId { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Position { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;

        public List<Guid> InspectedOrders { get; set; }
        public List<Guid> InspectedProjects { get; set; }
    }
}
