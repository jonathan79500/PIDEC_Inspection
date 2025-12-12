using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationsByProjUsrIds;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetApprovedInspectionNotificationsByProjIdUserId
{
    public class GetApprovedInspectionNotificationsByProjIdUserIdRequestHandler : IRequestHandler<GetApprovedInspectionNotificationsByProjIdUserIdRequest, List<InspectionNotificationDataSummaryDto>>
    {
        private readonly IInspectionNotificationRepository _inspectionRequestRepository;
        //

        public GetApprovedInspectionNotificationsByProjIdUserIdRequestHandler(IInspectionNotificationRepository inspectionRequestRepository)
        {
            _inspectionRequestRepository = inspectionRequestRepository;
        }

        public Task<List<InspectionNotificationDataSummaryDto>> Handle(GetApprovedInspectionNotificationsByProjIdUserIdRequest request, CancellationToken cancellationToken)
        {
            var result = _inspectionRequestRepository.GetReadyNotificationsToAttachInspectionReportByProjIdUserId(request.ProjectId, request.UserId).Result;
            return Task.FromResult(result);
        }
    }

}
