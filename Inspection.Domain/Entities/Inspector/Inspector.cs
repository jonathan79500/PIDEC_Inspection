using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class Inspector:Person
    {
        public ContactId? ContactId { get; set; }
        public List<InspectorOrder> inspectedOrders { get; set; }
        public List<InspectorProject> inspectedProjects { get; set; }
        public List<CirculationInspector> Circulations { get; set; }
    }
}
