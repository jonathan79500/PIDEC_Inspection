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

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetDraftedNotificationsByUserName
{
    public class GetDraftedNotificationsByUserNameRequestHandler : IRequestHandler<GetDraftedNotificationsByUserNameRequest, List<InspectionNotificationDataSummaryDto>>
    {
        IInspectionNotificationRepository  _inspectionNotificationRepository;
        public GetDraftedNotificationsByUserNameRequestHandler(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _inspectionNotificationRepository = inspectionNotificationRepository;
        }

        public Task<List<InspectionNotificationDataSummaryDto>> Handle(GetDraftedNotificationsByUserNameRequest request, CancellationToken cancellationToken)
        {
            try
            {
                List<InspectionNotificationDataSummaryDto> DraftedInspectionNotifications = _inspectionNotificationRepository.GetDraftInspectionNotificationsByUserName(request.userName).Result;
              
                return Task.FromResult(DraftedInspectionNotifications);
            }catch(Exception ex)
            {
                return null;
            }
        }
    }
}
