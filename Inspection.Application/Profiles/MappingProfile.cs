using AutoMapper;
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain;
using Inspection.Domain.Entities;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;


namespace Inspection.Application.Profiles
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<InspectionType, InspectionTypeDto>()
                .ForMember(intp => intp.Id, opt => opt.MapFrom(src => src.InspectionTypeId.Value)).ReverseMap()
                .ForMember(inpt => inpt.Name, opt => opt.MapFrom(src => src.Name)).ReverseMap();

            CreateMap<Vendor, VendorDto>()
            .ForMember(intp => intp.Id, opt => opt.MapFrom(src => src.Id)).ReverseMap()
            .ForMember(inpt => inpt.Name, opt => opt.MapFrom(src => src.Name)).ReverseMap()
            .ForMember(inpt => inpt.ContactsDto, opt => opt.MapFrom(src => src.Contacts)).ReverseMap();


            CreateMap<Order, OrderDto>()
            .ForMember(intp => intp.Id, opt => opt.MapFrom(src => src.Id)).ReverseMap()
            .ForMember(inpt => inpt.Name, opt => opt.MapFrom(src => src.Name)).ReverseMap()
            .ForMember(inpt => inpt.No, opt => opt.MapFrom(src => src.No)).ReverseMap()
            .ForMember(inpt => inpt.Title, opt => opt.MapFrom(src => src.Title)).ReverseMap()
            .ForMember(inpt => inpt.ProjectDto, opt => opt.MapFrom(src => src.Project)).ReverseMap();

            CreateMap<Contact, ContactDto>()
            .ForMember(intp => intp.Id, opt => opt.MapFrom(src => src.Id)).ReverseMap()
            .ForMember(inpt => inpt.Name, opt => opt.MapFrom(src => src.Name)).ReverseMap()
            .ForMember(inpt => inpt.AddressDto, opt => opt.MapFrom(src => src.Address)).ReverseMap()
            .ForMember(inpt => inpt.Email, opt => opt.MapFrom(src => src.Email)).ReverseMap()
            .ForMember(inpt => inpt.Fax, opt => opt.MapFrom(src => src.Fax)).ReverseMap()
            .ForMember(inpt => inpt.Phone, opt => opt.MapFrom(src => src.Phone)).ReverseMap()
            .ForMember(inpt => inpt.HomePage, opt => opt.MapFrom(src => src.HomePage)).ReverseMap();

            CreateMap<InspectionNotification, InspectionNotificationDto>().
                ForMember(infd => infd.Id, opt => opt.MapFrom(src => src.InspectionNotificationId.Value))                
                .ForMember(infd => infd.InspectionTypeDto, opt => opt.MapFrom(src => (src.InspectionType)))
                .ReverseMap();

            //CreateMap<Guid, InspectionTypeId>()
            //    .ForMember(d => d.Value, opt => opt.MapFrom(s => s)).ReverseMap();

            CreateMap<CreateInspectionNotificationDto, InspectionNotification>()
                .ForMember(d => d.InspectionNotificationId, opt => opt.MapFrom(s => Guid.NewGuid()));

            CreateMap<InspectionNotification, CreateInspectionNotificationDto>()                
                .ForMember(d => d.OrderId, opt => opt.MapFrom(s => s.Order!.Id))
                .ForMember(d=>d.OwnerId,opt=>opt.MapFrom(s=>s.Owner.Id))
                .ForMember(d=>d.InspectionTypeId,opt=>opt.MapFrom(s=>s.InspectionType!.InspectionTypeId))
                .ForMember(d=>d.LocationId,opt=>opt.MapFrom(s=>s.Location!.Id))
                .ForMember(d => d.NotificationNumber, opt => opt.MapFrom(s => s.NotificationNumber!.name))
                .ReverseMap();


            //CreateMap<CreateInspectionNotificationDto, InspectionNotification>()
            //    .ForMember(d=>d.Order,opt=>opt.MapFrom(s=>s.OrderId))
                //.ForMember(infd => infd.Order, src=>src.MapFrom(src=>Order.Create(src.OrderId,string.Empty,string.Empty,string.Empty,null))                   
                //)
                //.ForMember(infd => infd.Owner, src => src.MapFrom(src =>new Vendor(src.Id)))
                //.ForMember(infd => infd.InspectionType, src => src.MapFrom(src => InspectionType.Create(src.Id,string.Empty)))
                //.ForMember(infd=> infd.Contacts ,src=>src.MapFrom(src=>new List<ContactDto>()
                //{                     
                //}
                //))
                
                  
               //.ForMember(infd => infd.InspectionType, src => src.MapFrom(src => new InspectionTypeDto()
               //{
               //    Id = src.Id
               //}))

               //.ForMember(infd=>infd.Owner, opt => opt.MapFrom(src => new Vendor(src.Vendor_Id)))
               //.ReverseMap();


            //CreateMap<CreateInspectionNotificationDto ,InspectionNotification >()
            //    .ForMember(infd => infd.Order, opt => opt.MapFrom(src => new Order(src.OrderId)))
            //    //.ForMember(infd=>infd.Owner, opt => opt.MapFrom(src => new Vendor(src.Vendor_Id)))
            //   .ReverseMap();

        }
    }
}
