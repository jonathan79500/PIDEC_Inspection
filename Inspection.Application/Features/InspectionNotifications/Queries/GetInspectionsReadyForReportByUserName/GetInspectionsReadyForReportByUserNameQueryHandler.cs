using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionsReadyForReportByUserName
{
    public class GetInspectionsReadyForReportByUserNameQueryHandler : IRequestHandler<GetInspectionsReadyForReportByUserNameQuery, List<InspectionNotificationDataSummaryDto>>
    {
        private IInspectionNotificationRepository _inspectionNotificationRepository;
        public GetInspectionsReadyForReportByUserNameQueryHandler(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _inspectionNotificationRepository = inspectionNotificationRepository;
        }
        public Task<List<InspectionNotificationDataSummaryDto>> Handle(GetInspectionsReadyForReportByUserNameQuery request, CancellationToken cancellationToken)
        {
            var result = _inspectionNotificationRepository.GetReadyNotificationsToAttachInspectionReportByProjIdUserId(request.projectId, request.userId).Result;
            List<InspectionNotificationDataSummaryDto> readyNotificationsForGenerateInspectionReport = result.ToList();
            return Task.FromResult(readyNotificationsForGenerateInspectionReport);
        }
    }
}
