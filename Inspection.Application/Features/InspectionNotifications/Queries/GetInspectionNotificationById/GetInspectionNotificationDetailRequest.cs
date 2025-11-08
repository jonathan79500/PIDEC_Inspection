using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using MediatR;
using System;


namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById;

public class GetInspectionNotificationByIdRequest : IRequest<InspectionNotificationDto> //IQuery<InspectionNotificationDto>
{
    
    public string InspectionNotificationId { get; set; }
}
