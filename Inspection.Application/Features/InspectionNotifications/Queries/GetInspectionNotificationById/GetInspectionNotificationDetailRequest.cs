using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionReport;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;


namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById;

public class GetInspectionNotificationByIdRequest : IRequest<(List<InspectionNotificationDto>, List<InspectionReportDto>)> //IQuery<InspectionNotificationDto>
{
    
    public string InspectionNotificationId { get; set; }
}
