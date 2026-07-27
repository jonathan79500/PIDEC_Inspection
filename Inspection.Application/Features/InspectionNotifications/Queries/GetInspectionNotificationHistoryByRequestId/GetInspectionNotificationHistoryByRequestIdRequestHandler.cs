using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationHistoryByRequestId
{
    public class GetInspectionNotificationHistoryByRequestIdRequestHandler : IRequestHandler<GetInspectionNotificationHistoryByRequestIdRequest, List<InspectionNotificationHistoryDto>>
    {
        private IInspectionNotificationRepository _inspectionNotificationrepository;
        public GetInspectionNotificationHistoryByRequestIdRequestHandler(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _inspectionNotificationrepository = inspectionNotificationRepository;
        }
        public Task<List<InspectionNotificationHistoryDto>> Handle(GetInspectionNotificationHistoryByRequestIdRequest request, CancellationToken cancellationToken)
        {
            List<InspectionNotificationHistoryDto> inspectionNotificationHistories = null;
            try
            {

                if (request.RequestId != null)
                {
                    inspectionNotificationHistories = _inspectionNotificationrepository.GetInspectionNotificationHistory(request.RequestId).Result;
                }
                return Task.FromResult(inspectionNotificationHistories);
            }
            catch (Exception ex) {
                return null;
            }
        }
    }
}
