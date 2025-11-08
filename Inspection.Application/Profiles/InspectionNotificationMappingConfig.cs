using AutoMapper;
using AutoMapper.Configuration;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Features.InspectionNotifications.Commands.CreateInspectionNotification;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Mapster;
using System.Collections.Generic;
using System.Linq;


namespace Inspection.Application.Profiles;

public class InspectionNotificationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(CreateInspectionNotificationCommand command, InspectionNotificationDto ins), InspectionNotification>().PreserveReference(true)
            .Map(dest => dest.InspectionNotificationId.Value, src => src.ins.Id);

        config.NewConfig<InspectionNotification, CreateInspectionNotificationDto>()
            .Map(dest => dest.Id, src => src.InspectionNotificationId.Value)
            .Map(dest => dest.Order, src => src.Order!.Id)
            .Map(dest => dest.Owner, src => src.Owner.Id);
            
            

        config.NewConfig<(GetInspectionNotificationDetailRequest req,InspectionNotification res), InspectionNotificationDto>().PreserveReference(true)
            .Map(dest => dest.Id, src => src.res.InspectionNotificationId.Value)           
            .Map(dest=>dest.Order ,src=>src.res.Order);
            
    }
}
