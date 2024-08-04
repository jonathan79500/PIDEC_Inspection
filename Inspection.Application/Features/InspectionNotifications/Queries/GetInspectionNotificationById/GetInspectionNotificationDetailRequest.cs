using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using MediatR;
using System;


namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById;

public class GetInspectionNotificationDetailRequest : IRequest<InspectionNotificationDto> //IQuery<InspectionNotificationDto>
{
    
    public InspectionNotificationId Id { get; set; }
}
