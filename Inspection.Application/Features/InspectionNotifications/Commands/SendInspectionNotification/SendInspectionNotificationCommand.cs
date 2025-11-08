using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using MediatR;

namespace Inspection.Application.Features.InspectionNotifications.Commands.SendInspectionNotification;

public class SendInspectionNotificationCommand : IRequest<InspectionNotification>
{
    public CreateInspectionNotificationDto createInspectionNotificationDto { get; set; }
    public ProjectDto ProjectDto { get; set; }
    public CirculationDto currentCirculationDto { get; set; }
}

