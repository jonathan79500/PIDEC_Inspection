using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using MediatR;
using System.Collections.Generic;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationsByProjUsrIds;

public class GetInspectionNotificationsByProjUsrIdsRequest:IRequest<List<InspectionNotificationDataSummaryDto>>
{
    public string ProjectId { get; set; }
    public string UserId { get; set; }
}
