using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetCompletedNotificationsByUserName
{
    public class GetCompletedNotificationsByUserNameRequestHandler : IRequestHandler<GetCompletedNotificationsByUserNameRequest, List<InspectionNotificationDataSummaryDto>>
    {
        private IInspectionNotificationRepository _inspectionNotificationRepository;
        public GetCompletedNotificationsByUserNameRequestHandler(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _inspectionNotificationRepository = inspectionNotificationRepository;
        }
        public Task<List<InspectionNotificationDataSummaryDto>> Handle(GetCompletedNotificationsByUserNameRequest request, CancellationToken cancellationToken)
        {
            try
            {
                List<InspectionNotificationDataSummaryDto> CompletedInspectionNotifications = _inspectionNotificationRepository.GetCompletedInspectionNotificationsByUserName(request.userName).Result;
            
                return Task.FromResult(CompletedInspectionNotifications);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
