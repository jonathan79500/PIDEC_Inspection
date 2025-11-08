using Inspection.Application.DTOs.InspectionRequest;
using MediatR;
using System.Collections.Generic;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationsByProjUsrIds;

public class GetInspectionNotificationsByProjUsrIdsRequest:IRequest<List<InspectionNotificationDto>>
{
    public string ProjectId { get; set; }
    public string UserId { get; set; }
}
