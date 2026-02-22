using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;

namespace Inspection.Domain.Entities.InspectionNotification.Entities
{
    public class InspectionNotificationRevision : BaseDomainEntity<Guid>
    {

        public InspectionNotificationRevision() { }

        public InspectionNotificationId InspectionNotificationId { get; set; }
        public Guid RevisionId { get; set; }

        public InspectionNotification InspectionNotification { get; set; }
        public Revision Revision { get; set; }

    }
}
