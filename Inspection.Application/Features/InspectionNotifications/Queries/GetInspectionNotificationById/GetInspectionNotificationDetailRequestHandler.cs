using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Contracts.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System.Collections.Generic;
using Inspection.Application.DTOs.InspectionReport;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById
{
    public class GetInspectionNotificationRequestHandler : IRequestHandler<GetInspectionNotificationByIdRequest, (InspectionNotificationDto,List<InspectionReportDto>)>
    {
        private readonly IInspectionNotificationRepository _inspectionRequestRepository;
       //

        public GetInspectionNotificationRequestHandler(IInspectionNotificationRepository inspectionRequestRepository)
        {
           
            _inspectionRequestRepository = inspectionRequestRepository;
        }

        public async Task<(InspectionNotificationDto, List<InspectionReportDto>)> Handle(GetInspectionNotificationByIdRequest request, CancellationToken cancellationToken)
        {
            string strInspectionNotificationId = request.InspectionNotificationId;
            InspectionNotificationId newInspectionNotificationId = InspectionNotificationId.Create(new System.Guid(strInspectionNotificationId));
            var inspectionRequest = await _inspectionRequestRepository.GetInspectionNotificationById(newInspectionNotificationId);
            InspectionNotificationDto insNoteDto = inspectionRequest.Item1.Adapt<InspectionNotificationDto>();
            List<InspectionReportDto> relinsReportsDto = inspectionRequest.Item2.Adapt<List<InspectionReportDto>>();
            return (insNoteDto, relinsReportsDto);
        }
    }
}
