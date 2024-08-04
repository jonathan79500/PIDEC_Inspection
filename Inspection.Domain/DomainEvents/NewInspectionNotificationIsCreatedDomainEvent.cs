using Inspection.Domain.Entities.Common;
using Inspection.Domain.Primitives;
using System;

namespace Inspection.Domain.DomainEvents;

public class NewInspectionNotificationIsCreatedDomainEvent(Guid InspectionNotificationId):DomainEvent
{
}
