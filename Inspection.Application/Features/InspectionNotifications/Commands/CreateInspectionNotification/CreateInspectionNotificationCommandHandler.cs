

using AutoMapper;
using Inspection.Application.Features.InspectionNotifications.Validators;
using Inspection.Application.Contracts.Persistence;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Models;
using System;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification;
using System.Linq;
using Arch.EntityFrameworkCore.UnitOfWork;

namespace Inspection.Application.Features.InspectionNotifications.Commands.CreateInspectionNotification;

public class CreateInspectionNotificationCommandHandler : IRequestHandler<CreateInspectionNotificationCommand, InspectionNotification>
{
    private readonly IInspectionNotificationRepository _inspectionNotificationRepository;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;
    private readonly IPublisher _publisher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInspectionNotificationCommandHandler(IInspectionNotificationRepository inspectionRequestRepository, IMapper mapper, IEmailSender emailSender,IPublisher publisher)//,IUnitOfWork unitOfWork)
    {
        _inspectionNotificationRepository = inspectionRequestRepository;
        _mapper = mapper;
        _emailSender = emailSender;
        _publisher = publisher;
       // _unitOfWork = unitOfWork;
    }


    public async Task<InspectionNotification> Handle(CreateInspectionNotificationCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateInspectionNotificationValidator();
        var validationResult = validator.ValidateAsync(request.createInspectionNotificationDto).Result;
        if (validationResult.IsValid != true)
        {
            throw new Exception();
        }
        var InspectionNotification = _mapper.Map<InspectionNotification>(request.createInspectionNotificationDto);

        var ins = InspectionNotification.CreateByUniqueId(InspectionNotification);
        var newinspectionNotification = await _inspectionNotificationRepository.CreateNewInspectionNotification(ins);

       // await _unitOfWork.SaveChangesAsync();


        await _publisher.Publish(newinspectionNotification.Events.First(),cancellationToken);

        var email = new Email
        {
            To = "akbari.elahe@pidec.com",
          //  Body = $"Email From Inspection System at {request.createInspectionNotificationDto.ProposedDate: D}",
            Subject = "Inspection Notification"
        };

        try
        {
            await _emailSender.SendEmail(email);
        }
        catch (Exception ex) { }

        return InspectionNotification;
    }
}

