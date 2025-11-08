using Arch.EntityFrameworkCore.UnitOfWork;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.Features.InspectionNotifications.Validators;
using Inspection.Application.Models;
using Inspection.Domain.Entities.InspectionNotification;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Commands.CreateInspectionNotification;

public class CreateInspectionNotificationCommandHandler : IRequestHandler<CreateInspectionNotificationCommand, InspectionNotification>
{
    private readonly IInspectionNotificationRepository _inspectionNotificationRepository;
  
    private readonly IEmailSender _emailSender;
    private readonly IPublisher _publisher;


    public CreateInspectionNotificationCommandHandler(IInspectionNotificationRepository inspectionRequestRepository, IEmailSender emailSender,IPublisher publisher)//,IUnitOfWork unitOfWork)
    {
        _inspectionNotificationRepository = inspectionRequestRepository;     
        _emailSender = emailSender;
        _publisher = publisher;
      

    }


    public async Task<InspectionNotification> Handle(CreateInspectionNotificationCommand request, CancellationToken cancellationToken)
    {
        InspectionNotification resultinspectionNotification = null;

        var validator = new CreateInspectionNotificationValidator();
        var validationResult = validator.ValidateAsync(request.createInspectionNotificationDto).Result;
        if (validationResult.IsValid != true)
        {
            throw new Exception();
        }
        //  var InspectionNotification = _mapper.Map<InspectionNotification>(request.createInspectionNotificationDto);
        try
        {
            var InspectionNotification = request.createInspectionNotificationDto.Adapt<InspectionNotification>();
         
            //if (InspectionNotification != null && InspectionNotification.InspectionNotificationId != null && InspectionNotification.InspectionNotificationId.Value != Guid.Empty)
            //{
            //    resultinspectionNotification = await _inspectionNotificationRepository.UpdateInspectionNotification(InspectionNotification);
            //}
            //else
            //{

                resultinspectionNotification = await _inspectionNotificationRepository.CreateNewInspectionNotification(InspectionNotification);
            //}
        }
        catch (Exception exp)
        { 
        }
        //await _unitOfWork.SaveChangesAsync();


        //await _publisher.Publish(newinspectionNotification.Events.First(),cancellationToken);
       // await _publisher.Publish(newinspectionNotification.Events,cancellationToken);

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

        //await _unitOfWork.SaveChangesAsync();

       // return InspectionNotification;
       return await Task.FromResult(resultinspectionNotification);
    }
}

