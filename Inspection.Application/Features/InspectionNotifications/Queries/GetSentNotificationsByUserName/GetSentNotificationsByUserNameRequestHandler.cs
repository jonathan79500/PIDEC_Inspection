using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetSentNotificationsByUserName
{
    public class GetSentNotificationsByUserNameRequestHandler : IRequestHandler<GetSentNotificationsByUserNameRequest, List<InspectionNotificationDataSummaryDto>>
    {
        IInspectionNotificationRepository  _inspectionNotificationRepository;
        public GetSentNotificationsByUserNameRequestHandler(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _inspectionNotificationRepository = inspectionNotificationRepository;
        }

        public Task<List<InspectionNotificationDataSummaryDto>> Handle(GetSentNotificationsByUserNameRequest request, CancellationToken cancellationToken)
        {
            try
            {
                List<InspectionNotificationDataSummaryDto> SentInspectionNotifications = _inspectionNotificationRepository.GetSentInspectionNotificationsByUserName(request.userName).Result;
              
                return Task.FromResult(SentInspectionNotifications);
            }catch(Exception ex)
            {
                return null;
            }
        }
    }
}
