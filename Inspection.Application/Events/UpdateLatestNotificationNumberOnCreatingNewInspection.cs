using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.InspectionNotification;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Events
{
    public class UpdateLatestNotificationNumberOnCreatingNewInspection:INotificationHandler<InspectionNotificationCreatedEvent>
    {
        private readonly IInspectionNotificationRepository _inspectionNotificationRepository;

        public UpdateLatestNotificationNumberOnCreatingNewInspection(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _inspectionNotificationRepository = inspectionNotificationRepository;
        }

        public Task Handle(InspectionNotificationCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _inspectionNotificationRepository.UpdateNotificationNumber(notification.CreatedInspectionNotification.NotificationNumberParts);
        }
    }
}
