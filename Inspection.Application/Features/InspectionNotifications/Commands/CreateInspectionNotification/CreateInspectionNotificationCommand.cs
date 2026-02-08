using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using MediatR;
using System.Collections.Generic;

namespace Inspection.Application.Features.InspectionNotifications.Commands.CreateInspectionNotification;

public class CreateInspectionNotificationCommand : IRequest<(InspectionNotificationDto, CirculationDto)>
{
    public CreateInspectionNotificationDto createInspectionNotificationDto { get; set; }
    public ProjectDto ProjectDto { get; set; }
    public CirculationDto CurrentCirculationDto { get; set; }
    public List<AttachmentDto> AttachmentDtos { get; set; }
}

