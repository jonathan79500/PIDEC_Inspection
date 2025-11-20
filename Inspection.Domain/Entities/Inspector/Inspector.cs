using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class Inspector
    {
        public InspectorId InspectorId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public List<InspectorAttachment> inspectorAttachments { get; set; }
        public List<CirculationInspector> Circulations { get; set; }
    }
}
