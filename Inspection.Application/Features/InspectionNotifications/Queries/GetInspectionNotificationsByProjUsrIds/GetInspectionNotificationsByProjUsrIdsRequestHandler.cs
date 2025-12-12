
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Mapster;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationsByProjUsrIds;

public class GetInspectionNotificationsByProjUsrIdsRequestHandler : IRequestHandler<GetInspectionNotificationsByProjUsrIdsRequest, List<InspectionNotificationDataSummaryDto>>
{
    private readonly IInspectionNotificationRepository _inspectionRequestRepository;
    //

    public GetInspectionNotificationsByProjUsrIdsRequestHandler(IInspectionNotificationRepository inspectionRequestRepository)
    {
        _inspectionRequestRepository = inspectionRequestRepository;
    }

    public Task<List<InspectionNotificationDataSummaryDto>> Handle(GetInspectionNotificationsByProjUsrIdsRequest request, CancellationToken cancellationToken)
    {
        var result = _inspectionRequestRepository.GetInspectionNotificationsByProjUsrIds(request.ProjectId, request.UserId).Result;
        return Task.FromResult(result);
    }
}
