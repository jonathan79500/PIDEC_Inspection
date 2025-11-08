using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;


namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationFlow;

public class GetInspectionNotificationFlow : IRequest<InspectionNotificationFlowDto> //IQuery<InspectionNotificationDto>
{
    
    public string NotificationNumber { get; set; }
}
