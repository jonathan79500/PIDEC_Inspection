using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetNewNotificationNumber
{
    public class GetNewNotificationNumberQueryHandler : IRequestHandler<GetNewNotificationNumberQuery, string>
    {
        IInspectionNotificationRepository _repository;
        public GetNewNotificationNumberQueryHandler(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _repository = inspectionNotificationRepository;
        }
        public Task<string> Handle(GetNewNotificationNumberQuery request, CancellationToken cancellationToken)
        {
            NotificationNumber generatedNotificationNumber = _repository.GenerateNewNotificationNumber(request.ProjectNumber, request.OrderNumber).Result;

            return Task.FromResult<string>(generatedNotificationNumber.ToString());
        }
    }
}
