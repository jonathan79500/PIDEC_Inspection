using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class CirculationInspector : BaseDomainEntity<Guid>
    {
        public CirculationInspector() { }

        public Guid CirculationId{ get; set; }
        public InspectorId? InspectorId { get; set; }

        public Circulation? Circulation{ get; set; }
        public Inspector.Inspector? Inspector { get; set; }
    }
}
