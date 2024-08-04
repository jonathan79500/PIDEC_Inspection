using AutoMapper;
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using System;


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
           
            CreateMap<InspectionNotification, CreateInspectionNotificationDto>().ReverseMap();

        }
    }
}
