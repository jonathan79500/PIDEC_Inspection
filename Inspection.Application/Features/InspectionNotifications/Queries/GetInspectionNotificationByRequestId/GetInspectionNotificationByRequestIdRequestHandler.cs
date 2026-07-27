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
    public class GetInspectionNotificationByRequestIdRequestHandler : IRequestHandler<GetInspectionNotificationByRequestIdRequest, (List<InspectionNotificationDto>,List<InspectionReportDto>)>
    {
        private readonly IInspectionNotificationRepository _inspectionRequestRepository;
       //

        public GetInspectionNotificationByRequestIdRequestHandler(IInspectionNotificationRepository inspectionRequestRepository)
        {
           
            _inspectionRequestRepository = inspectionRequestRepository;
        }

        public async Task<(List<InspectionNotificationDto>, List<InspectionReportDto>)> Handle(GetInspectionNotificationByRequestIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string strRequestId = request.RequestId;
                string strUserId = request.UserId;
                Guid requestId = new System.Guid(strRequestId);
                //InspectionNotificationId newInspectionNotificationId = InspectionNotificationId.Create(new System.Guid("7d4686b1-64c0-4223-b587-5e8787fe76f2"));
                //var inspectionRequest = await _inspectionRequestRepository.GetInspectionNotificationById(newInspectionNotificationId);

                 var inspectionRequest = await _inspectionRequestRepository.GetInspectionNotificationByRequestId(requestId,strUserId);
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
