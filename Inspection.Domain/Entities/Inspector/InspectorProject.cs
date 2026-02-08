using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class InspectorProject
    {
        public int Id { get; set; }
        public Guid InspectorId { get; set; }
        public ProjectId ProjectId { get; set; }

        public Inspector Inspector { get; set; }
        public Project Project { get; set; }
    }
}
