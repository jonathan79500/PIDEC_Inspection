using Inspection.Application.DTOs.InspectionRequest;
using MediatR;
using System.Collections.Generic;


namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotifications;

public class GetInspectionNotificationListRequest : IRequest<List<InspectionNotificationDto>>
{
}
