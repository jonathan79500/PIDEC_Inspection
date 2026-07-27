using Arch.EntityFrameworkCore.UnitOfWork;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Features.InspectionNotifications.Validators;
using Inspection.Application.Models;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.EDMS;
using Inspection.Domain.Entities.InspectionNotification;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.User.Enums;
using Inspection.Domain.Entities.ValueObjects;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Commands.SendInspectionNotification;

public class SendInspectionNotificationCommandHandler : IRequestHandler<SendInspectionNotificationCommand, (InspectionNotificationDto, CirculationDto)>
{
    private readonly IInspectionNotificationRepository _inspectionNotificationRepository;
    private readonly IUserManagerRepository _userManagerRepository;
    private readonly IEDMSService _edmsService;

    private readonly IEmailSender _emailSender;
    private readonly IPublisher _publisher;


    public SendInspectionNotificationCommandHandler(IInspectionNotificationRepository inspectionRequestRepository, IUserManagerRepository userManagerRepository, IEDMSService eDMSService, IEmailSender emailSender, IPublisher publisher)//,IUnitOfWork unitOfWork)
    {
        _inspectionNotificationRepository = inspectionRequestRepository;
        _userManagerRepository = userManagerRepository;
        _emailSender = emailSender;
        _publisher = publisher;
        _edmsService = eDMSService;
    }


    public async Task<(InspectionNotificationDto, CirculationDto)> Handle(SendInspectionNotificationCommand request, CancellationToken cancellationToken)
    {

        try
        {

            var validator = new CreateInspectionNotificationValidator();
            var validationResult = await validator.ValidateAsync(request.CreateInspectionNotificationDto);
            if (validationResult.IsValid != true)
            {
                throw new Exception();
            }

            InspectionNotification InspectionNotification = request.CreateInspectionNotificationDto.Adapt<InspectionNotification>();

            Circulation returnedcirculation= new Circulation();
            Circulation currentCirculation = request.CurrentCirculationDto.Adapt<Circulation>();
            var project = request.ProjectDto.Adapt<Project>();
            var attachments = request.AttachmentDtos.Adapt<List<Attachment>>();

            InspectionNotification notificationForSend = null;
            try
            {

                NotificationNumber currentInspectionNumber = NotificationNumber.CreateFromString(InspectionNotification.NotificationNumber);

                //-------------This InspectionNotification is generated before this time and is sent to client because it needed an edit or a new Revision--------------------
                if (InspectionNotification.InspectionNotificationId.Value != null && InspectionNotification.InspectionNotificationId.Value != Guid.Empty && currentCirculation !=null)
                {
                    (notificationForSend, returnedcirculation) = await _inspectionNotificationRepository.ModifyInspectionNotification(currentCirculation, InspectionNotification, attachments);
                }
                else
                {

                    NotificationNumber newNotificationNumber = _inspectionNotificationRepository.GenerateNewNotificationNumber(project.Number, InspectionNotification.Order.Name).Result;
                    string newRevisedNumber = newNotificationNumber.ToString();

                    InspectionNotification.NotificationNumber = newRevisedNumber;
                    (notificationForSend, currentCirculation) = await _inspectionNotificationRepository.CreateNewInspectionNotification(InspectionNotification, newNotificationNumber, attachments);
                   
                }
                //var finalInspectionNotification1 = notificationForSend.Adapt<InspectionNotificationDto>();
                var finalInspectionNotification1 = notificationForSend.Adapt<CreateInspectionNotificationDto>();
                var result = CreateNextUsersCirculations(notificationForSend, project);
                await _inspectionNotificationRepository.UpdateNotificationNumber(NotificationNumber.CreateFromString(notificationForSend!.NotificationNumber!));
            }
            catch (Exception exp)
            {

            }          

            var finalInspectionNotification = notificationForSend.Adapt<InspectionNotificationDto>();
            CirculationDto currentCirculationDto = currentCirculation.Adapt<CirculationDto>();
            return await Task.FromResult((finalInspectionNotification, currentCirculationDto));

        }
        catch (Exception exp)
        {
            throw;
        }

   //-------------------------------------------------------------------------------------------------

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

        //    return newinspectionNotification;
    }

    public async void SendEmail(string link, string emailAddress)
    {
        var email = new Email
        {
            To = emailAddress,
            //  Body = $"Email From Inspection System at {request.createInspectionNotificationDto.ProposedDate: D}",
            Subject = "Inspection Notification New UserName and Password Reset Link",
            Body = link,
            From = "QCIMS@pidec.com"
        };

        try
        {
            await _emailSender.SendEmail(email);
        }
        catch (Exception ex) { }

    }

    public async Task<List<Circulation>> CreateNextUsersCirculations(InspectionNotification inspectionNotification, Project project)
    {
        string projectId = project.FSId;
        string orderDotId = inspectionNotification.Order.EdmsId;
        List<Circulation> circulations = new List<Circulation>();



        try
        {
            List<InspectionNotificationContact> PidecContacts = inspectionNotification.Contacts!.Where((contact) => contact.Contact.ContactType == ContactType.PIDEC_TO.GetHashCode() || contact.Contact.ContactType == ContactType.PIDEC_CC.GetHashCode()).ToList();
            List<InspectionNotificationContact> PidecInspectorContacts = inspectionNotification.Contacts!.Where((contact) => contact.Contact.ContactType == ContactType.PIDEC_Inspector.GetHashCode()).ToList();
            List<InspectionNotificationContact> ThirdPartInspectorContacts = inspectionNotification.Contacts!.Where((contact) => contact.Contact.ContactType == ContactType.ThirdPart_Inspector.GetHashCode()).ToList();

            Contact creator = inspectionNotification.Contacts!.Where((contact) => contact.Contact.ContactType == ContactType.Vendor.GetHashCode()).FirstOrDefault()!.Contact;


            string currentDatePersian = "";
            DateTime currentTime = DateTime.Now;

            if (PidecContacts.Count > 0)
            {


                PidecContacts.ForEach((pContact) =>
                {
                    Circulation nextCirculation = Circulation.CreateForNextContact(inspectionNotification, pContact.Contact);

                    circulations.Add(nextCirculation);
                 

                });

            }

            if (PidecInspectorContacts.Count > 0)
            {


                PidecInspectorContacts.ForEach((piContact) =>
                {
                    Circulation nextCirculation = Circulation.CreateForNextContact(inspectionNotification, piContact.Contact);

                    circulations.Add(nextCirculation);                  

                });

            }

            if (ThirdPartInspectorContacts.Count > 0)
            {


                ThirdPartInspectorContacts.ForEach((tpiContact) =>
                {
                    Circulation nextCirculation = Circulation.CreateForNextContact(inspectionNotification, tpiContact.Contact);

                    circulations.Add(nextCirculation);
                    //inspectionNotification.Circulations.Add(nextCirculation);

                });

            }


            var departmentLeads = _edmsService.GetOrderDepartmentLeads(orderDotId, projectId);
            Dictionary<Contact, string> departmentLeadsContacts = await GetDepartmentLeadContacts(departmentLeads);

            if (departmentLeadsContacts != null && departmentLeadsContacts.Count > 0)
            {

                for (int i = 0; i < departmentLeadsContacts.Count; i++)
                {
                    Circulation nextCirculation = Circulation.CreateForNextContact(inspectionNotification, departmentLeadsContacts.ElementAt(i).Key);
                    circulations.Add(nextCirculation);
                    // inspectionNotification.Circulations.Add(nextCirculation);

                    try
                    {
                        SendEmail(departmentLeadsContacts.ElementAt(i).Value, departmentLeadsContacts.ElementAt(i).Key.Email);
                    }
                    catch (Exception exp)
                    {

                    }
                }
            
            }
          
            _inspectionNotificationRepository?.SaveInspectionNotificationCirculations(circulations);


            return await Task.FromResult(circulations);
        }
        catch (Exception exp)
        {
            return null;
        }
    }
 
    private async Task<Dictionary<Contact, string>> GetDepartmentLeadContacts(Task<EDMSOrderDepartmentLeads> departmentLeads)
    {
        List<Contact> PidecDepartmentContacts = new List<Contact>();
        Dictionary<Contact, string> PidecDepartmentContacts_UserLink = new Dictionary<Contact, string>();
        SystematicUser originatorUser = new SystematicUser();
        SystematicUser DepartmentleadUser = new SystematicUser();
        EDMSOrderDepartmentLeads eDMSOrderDepartmentLeads = departmentLeads.Result;

        SystematicUser userLikeOriginator;
        SystematicUser userLikeDeptLead;


        string DeptLeadResetPasswordLink = "";
        string DeptOriginatorResetPasswordLink = "";

        SystematicRole LeadDepartmentRole;
        SystematicRole originatorRole;

        Contact LeadDepartmentContact;
        Contact OriginatorContact;

        List<string> RoleNames = new List<string>();
        try
        {
            userLikeOriginator = await _userManagerRepository.GetActiveUserByUserName_DomainId(eDMSOrderDepartmentLeads.originator, 0);
            userLikeDeptLead = await _userManagerRepository.GetActiveUserByUserName_DomainId(eDMSOrderDepartmentLeads.leadEngineer, 0);


            // LeadDepartmentRole = _userManagerRepository.AddNewRole(eDMSOrderDepartmentLeads.deptCode + "_Lead", UserType.PIDECDepartmentLead.GetHashCode().ToString()).Result;//_dbContext.Roles.Add(LeadDepartmentRole).Entity;

            //originatorRole = _userManagerRepository.AddNewRole(eDMSOrderDepartmentLeads.deptCode + "_Originator", UserType.PIDECDepartmentOriginator.GetHashCode().ToString()).Result;//_dbContext.Roles.Add(originatorRole).Entity;
            LeadDepartmentRole = _userManagerRepository.AddNewRole("PIDECDepartment_Lead", UserType.PIDECDepartmentLead.GetHashCode().ToString()).Result;//_dbContext.Roles.Add(LeadDepartmentRole).Entity;

            originatorRole = _userManagerRepository.AddNewRole("PIDECDepartment_Originator", UserType.PIDECDepartmentOriginator.GetHashCode().ToString()).Result;//_dbContext.Roles.Add(originatorRole).Entity;


            if (userLikeDeptLead == null)
            {
                DepartmentleadUser.UserName = eDMSOrderDepartmentLeads.leadEngineer;
                DepartmentleadUser.PasswordHash = "".GetHashCode().ToString();
                DepartmentleadUser.activationStartDate = DateTime.Now;
                DepartmentleadUser.DomainId = 0;
                DepartmentleadUser.IsActive = true;
                DepartmentleadUser.Email = eDMSOrderDepartmentLeads.leadEngineer.Trim() + "@pidec.com";
                DepartmentleadUser.Id = Guid.NewGuid().ToString();
                DepartmentleadUser.UserType = UserType.PIDECDepartmentLead.GetHashCode();
                // DepartmentleadUser.Roles.Add(LeadDepartmentRole);

                LeadDepartmentContact = new Contact();
                LeadDepartmentContact.Note = "";
                LeadDepartmentContact.Address = null;
                LeadDepartmentContact.Phone = "";
                LeadDepartmentContact.Email = eDMSOrderDepartmentLeads.leadEngineer.Trim() + "@pidec.com";
                LeadDepartmentContact.ContactId = Guid.NewGuid(); //ContactId.CreateUnique();
                LeadDepartmentContact.Cc = "";
                LeadDepartmentContact.ContactType = ContactType.PIDEC_LeadDepartment.GetHashCode();
                LeadDepartmentContact.UserName = eDMSOrderDepartmentLeads.leadEngineer;
                LeadDepartmentContact.CreatedDate = DateTime.Now;
                LeadDepartmentContact.ModifiedDate = DateTime.Now;
                LeadDepartmentContact.CreatedBy = "SysAdmin";
                LeadDepartmentContact.DomainId = 0;
                LeadDepartmentContact.Mobile = "";
                LeadDepartmentContact.Fax = "";
                LeadDepartmentContact.HomePage = "";
                LeadDepartmentContact.Name = eDMSOrderDepartmentLeads.leadEngineer;


                //DepartmentleadUser.contacts.Add(LeadDepartmentContact);
                RoleNames.Clear();
                RoleNames.Add("PIDECDepartment_Lead");
                // (DepartmentleadUser, DeptLeadResetPasswordLink) = _userManagerRepository.RegisterNewUser(DepartmentleadUser, RoleNames).Result;
                (DepartmentleadUser, DeptLeadResetPasswordLink) = _userManagerRepository.RegisterNewUserByRandomPass(DepartmentleadUser, RoleNames).Result;

                LeadDepartmentContact.UserId = DepartmentleadUser.Id;

                PidecDepartmentContacts.Add(LeadDepartmentContact);
                PidecDepartmentContacts_UserLink.Add(LeadDepartmentContact, DeptLeadResetPasswordLink);

                _inspectionNotificationRepository?.AddNewContact(LeadDepartmentContact);

                _userManagerRepository?.AddUserToRole(DepartmentleadUser, "PIDECDepartment_Lead");
            }
            else
            {
                var existedPidecLead = await _inspectionNotificationRepository.GetContactByUserId(userLikeDeptLead.Id);
                PidecDepartmentContacts.Add(existedPidecLead);

            }

            if (userLikeOriginator == null)
            {
                originatorUser.UserName = eDMSOrderDepartmentLeads.originator;
                originatorUser.PasswordHash = "".GetHashCode().ToString();
                originatorUser.activationStartDate = DateTime.Now;
                originatorUser.DomainId = 0;
                originatorUser.IsActive = true;
                originatorUser.Email = eDMSOrderDepartmentLeads.originator.Trim() + "@pidec.com";
                originatorUser.Id = Guid.NewGuid().ToString();
                originatorUser.UserType = UserType.PIDECDepartmentOriginator.GetHashCode();
                //originatorUser.Roles.Add(originatorRole);

                OriginatorContact = new Contact();
                OriginatorContact.Note = "";
                OriginatorContact.Address = null;
                OriginatorContact.Phone = "";
                OriginatorContact.Mobile = "";
                OriginatorContact.Email = eDMSOrderDepartmentLeads.originator.Trim() + "@pidec.com";
                OriginatorContact.ContactId = Guid.NewGuid();//ContactId.CreateUnique();
                OriginatorContact.Cc = "";
                OriginatorContact.ContactType = ContactType.PIDEC_Originator.GetHashCode();
                OriginatorContact.UserName = eDMSOrderDepartmentLeads.originator;
                OriginatorContact.CreatedDate = DateTime.Now;
                OriginatorContact.ModifiedDate = DateTime.Now;
                OriginatorContact.CreatedBy = "SysAdmin";
                OriginatorContact.DomainId = 0;
                OriginatorContact.Mobile = "";
                OriginatorContact.Fax = "";
                OriginatorContact.HomePage = "";
                OriginatorContact.Name = eDMSOrderDepartmentLeads.leadEngineer;

                RoleNames.Clear();
                RoleNames.Add("PIDECDepartment_Originator");

                (originatorUser, DeptOriginatorResetPasswordLink) = _userManagerRepository.RegisterNewUserByRandomPass(originatorUser, RoleNames).Result;

                OriginatorContact.UserId = originatorUser.Id;

                _inspectionNotificationRepository?.AddNewContact(OriginatorContact);
                PidecDepartmentContacts.Add(OriginatorContact);
                PidecDepartmentContacts_UserLink.Add(OriginatorContact, DeptOriginatorResetPasswordLink);

                _userManagerRepository?.AddUserToRole(originatorUser, "PIDECDepartment_Originator");
            }
            else
            {
                var existedPidecOriginator = await _inspectionNotificationRepository.GetContactByUserId(userLikeOriginator.Id);
                PidecDepartmentContacts.Add(existedPidecOriginator);
            }
        }
        catch (Exception exp)
        {
            return null;
        }

        if (PidecDepartmentContacts_UserLink.Count == 0)
            for (int i = 0; i < PidecDepartmentContacts.Count; i++)
            {
                PidecDepartmentContacts[i].ContactId = PidecDepartmentContacts[i].ContactId;
                PidecDepartmentContacts_UserLink.Add(PidecDepartmentContacts[i], "");
            }

        return PidecDepartmentContacts_UserLink;
    }
}

