using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Contracts.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById
{
    public class GetInspectionNotificationRequestHandler : IRequestHandler<GetInspectionNotificationByIdRequest, InspectionNotificationDto>
    {
        private readonly IInspectionNotificationRepository _inspectionRequestRepository;
       //

        public GetInspectionNotificationRequestHandler(IInspectionNotificationRepository inspectionRequestRepository)
        {
           
            _inspectionRequestRepository = inspectionRequestRepository;
        }

        public async Task<InspectionNotificationDto> Handle(GetInspectionNotificationByIdRequest request, CancellationToken cancellationToken)
        {
            string strInspectionNotificationId = request.InspectionNotificationId;
            InspectionNotificationId newInspectionNotificationId = InspectionNotificationId.Create(new System.Guid(strInspectionNotificationId));
            var inspectionRequest = await _inspectionRequestRepository.GetInspectionNotificationById(newInspectionNotificationId);
            return inspectionRequest.Adapt<InspectionNotificationDto>();           

        }
    }
}
