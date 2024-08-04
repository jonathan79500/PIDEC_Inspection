

using FluentValidation;
using Inspection.Application.DTOs.InspectionRequest;
using System;

namespace Inspection.Application.Features.InspectionNotifications.Validators;

public class CreateInspectionNotificationValidator:AbstractValidator<CreateInspectionNotificationDto>
{
    public CreateInspectionNotificationValidator()
    {
        RuleFor(i => i.Name).NotEmpty().WithMessage("Name is required.")
            .NotNull();
        
        DateTime pd= DateTime.Now.AddDays(1);

        RuleFor(i => i.ProposedDate).GreaterThan(pd);

        RuleFor(i=>i.NotificationNumber).NotEmpty().WithMessage("NotificationNumber is required").NotNull();
    }
}
