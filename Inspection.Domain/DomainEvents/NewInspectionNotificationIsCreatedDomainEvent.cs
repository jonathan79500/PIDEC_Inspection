using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Primitives;
using System;

namespace Inspection.Domain.DomainEvents;

public class NewInspectionNotificationIsCreatedDomainEvent(Guid InspectionNotificationId,NotificationNumber CreatedNotificationNumber):DomainEvent
{

}
