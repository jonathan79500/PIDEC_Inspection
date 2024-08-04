using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using MediatR;

namespace Inspection.Application.Features.InspectionNotifications.Commands.CreateInspectionNotification;

public class CreateInspectionNotificationCommand : IRequest<InspectionNotification>
{
    public CreateInspectionNotificationDto createInspectionNotificationDto { get; set; }
}

