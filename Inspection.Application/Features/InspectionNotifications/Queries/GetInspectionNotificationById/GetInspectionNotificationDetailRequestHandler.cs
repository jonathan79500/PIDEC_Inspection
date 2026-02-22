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
using System;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById
{
    public class GetInspectionNotificationRequestHandler : IRequestHandler<GetInspectionNotificationByIdRequest, (List<InspectionNotificationDto>,List<InspectionReportDto>)>
    {
        private readonly IInspectionNotificationRepository _inspectionRequestRepository;
       //

        public GetInspectionNotificationRequestHandler(IInspectionNotificationRepository inspectionRequestRepository)
        {
           
            _inspectionRequestRepository = inspectionRequestRepository;
        }

        public async Task<(List<InspectionNotificationDto>, List<InspectionReportDto>)> Handle(GetInspectionNotificationByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string strInspectionNotificationId = request.InspectionNotificationId;
                InspectionNotificationId newInspectionNotificationId = InspectionNotificationId.Create(new System.Guid(strInspectionNotificationId));
                var inspectionRequest = await _inspectionRequestRepository.GetInspectionNotificationById(newInspectionNotificationId);
                List<InspectionNotificationDto> insNotesDto = inspectionRequest.Item1.Adapt<List<InspectionNotificationDto>>();
                List<InspectionReportDto> relinsReportsDto = inspectionRequest.Item2.Adapt<List<InspectionReportDto>>();
                return (insNotesDto, relinsReportsDto);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}
