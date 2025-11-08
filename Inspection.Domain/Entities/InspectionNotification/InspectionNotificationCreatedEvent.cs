using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification
{
    public class InspectionNotificationCreatedEvent :DomainEvent
    {
        public InspectionNotification.Entities.InspectionNotification CreatedInspectionNotification { get; set; }

        public InspectionNotificationCreatedEvent(InspectionNotificationId inspectionNotificationId) { }
    }
}
