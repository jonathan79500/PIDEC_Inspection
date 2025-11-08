using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Features.InspectionNotifications.Queries.GetSentNotificationsByUserName;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetNotStartedNotificationsByUserName
{
    public class GetNotStartedNotificationsByUserNameRequestHandler : IRequestHandler<GetNotStartedNotificationsByUserNameRequest, List<InspectionNotificationDataSummaryDto>>
    {
        IInspectionNotificationRepository  _inspectionNotificationRepository;
        public GetNotStartedNotificationsByUserNameRequestHandler(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _inspectionNotificationRepository = inspectionNotificationRepository;
        }

        public Task<List<InspectionNotificationDataSummaryDto>> Handle(GetNotStartedNotificationsByUserNameRequest request, CancellationToken cancellationToken)
        {
            try
            {
                List<InspectionNotificationDataSummaryDto> NotStartedInspectionNotifications = _inspectionNotificationRepository.GetNotStartedInspectionNotificationsByUserName(request.userName).Result;
              
                return Task.FromResult(NotStartedInspectionNotifications);
            }catch(Exception ex)
            {
                return null;
            }
        }
    }
}
