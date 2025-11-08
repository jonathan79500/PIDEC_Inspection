using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Contracts.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using System.Collections.Generic;
using Inspection.Application.DTOs.Common;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationFlow
{
    public class GetInspectionNotificationFlowHandler : IRequestHandler<GetInspectionNotificationFlow,InspectionNotificationFlowDto>
    {
        private readonly IInspectionNotificationRepository _inspectionRequestRepository;
       //

        public GetInspectionNotificationFlowHandler(IInspectionNotificationRepository inspectionRequestRepository)
        {
           
            _inspectionRequestRepository = inspectionRequestRepository;
        }

        public async Task<InspectionNotificationFlowDto> Handle(GetInspectionNotificationFlow request, CancellationToken cancellationToken)
        {
            var inspectionFlow = await _inspectionRequestRepository.GetInspectionNotificationFlow(request.NotificationNumber);
            return inspectionFlow.Adapt<InspectionNotificationFlowDto>();           

        }
    }
}
