using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.Entities
{
    public class InspectionNotificationLocation : BaseDomainEntity<Guid>
    {
        public InspectionNotificationLocation() { }

        public InspectionNotificationId InspectionNotificationId { get; set; }
        public Guid AddressId { get; set; }

        public InspectionNotification InspectionNotification { get;  set; }
        public Address Location { get;  set; }
    }
}
