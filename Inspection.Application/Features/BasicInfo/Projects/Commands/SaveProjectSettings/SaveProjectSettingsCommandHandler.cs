using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.ValueObjects;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Projects.Commands.SaveProjectSettings
{
    public class SaveProjectSettingsCommandHandler : IRequestHandler<SaveProjectSettingsCommand, bool>
    {
        private  IBasicInformationRepository _basicInformationRepository;
        private IUserManagerRepository _userManagerRepository;
        private IEmailSender _emailSender;

        public SaveProjectSettingsCommandHandler(IBasicInformationRepository basicInformationRepository,IUserManagerRepository userManagerRepository,IEmailSender emailSender)
        {
            _basicInformationRepository = basicInformationRepository;
            _userManagerRepository = userManagerRepository;
            _emailSender = emailSender;
        }
        

        public Task<bool> Handle(SaveProjectSettingsCommand request, CancellationToken cancellationToken)
        {
            Project project = request.projectDto.Adapt<Project>();
            List<Contact> pidecQCContacts = request.projectDto.PidecQCContacts.Adapt<List<Contact>>();
            List<Contact> mcContacts = request.projectDto.MCContacts.Adapt<List<Contact>>();
            List<Contact> ownerContacts = request.projectDto.OwnerContacts.Adapt<List<Contact>>();
            List<Contact> TPIContacts = request.projectDto.TPInspectors.Adapt<List<Contact>>();
            List<Contact> pidecInspectorContacts = request.projectDto.PidecInspectors.Adapt<List<Contact>>();

            //Project project = null;
            //List<Contact> mcContacts = null;
            //List<Contact> ownerContacts = null;
            //List<Contact> TPIContacts = null;
            //List<Contact> pidecInspectorContacts = null;

            //  Task<bool> result = _basicInformationRepository.ActivateProjectInspectionSystem(project,mcContacts,ownerContacts,pidecInspectorContacts,TPIContacts);
            Task<bool> result = ActivateProjectInspectionSystem(project, mcContacts, ownerContacts, pidecInspectorContacts, TPIContacts,pidecQCContacts);
            return Task.FromResult(result.Result);
        }

        public async Task<bool> ActivateProjectInspectionSystem(Project project, List<Contact> mCContacts, List<Contact> ownerContacts, List<Contact> pidecInspectorsContacts, List<Contact> tPInspectorsContacts, List<Contact> pidecQCContacts)
        {
            try
            {                
                
                string mcPassword;
                List<Contact> savedpidecQCContacts = new List<Contact>();
                List<Contact> savedmcContacts = new List<Contact>();
                List<Contact> savedownerContacts = new List<Contact>();
                List<Contact> savedpidecInspectorContacts = new List<Contact>();
                List<Contact> savedthirdPartInspectorContacts = new List<Contact>();


                SystematicUser pidecQCUser;
                string pidecQCPass;
                for (int i = 0; i < pidecQCContacts.Count; i++)
                {

                    List<string> PIDECQCRole = new List<string>();
                    PIDECQCRole.Add(_userManagerRepository.GetContactTypeName_Code(pidecQCContacts[i].ContactType.GetHashCode()));

                    (pidecQCUser, pidecQCPass) = _userManagerRepository.RegisterNewUserByRandomPassFromContactData(pidecQCContacts[i], PIDECQCRole).Result;


                    if (pidecQCUser != null)
                    {
                        savedpidecQCContacts.Add(_userManagerRepository.SaveContactWithUserId(pidecQCContacts[i], pidecQCUser.Id).Result);
                        if (pidecQCPass != null && pidecQCPass != "")
                            _emailSender.SendEmail("akbari.elahe@pidec.com", "", "New User And Password to login to QCMI System as PIDEC QC Member", "your pass:" + pidecQCPass + " and user is :" + pidecQCUser.Email);
                    }
                }
                _userManagerRepository.SaveProjectContacts(project, savedpidecQCContacts);

                SystematicUser mcUser;
                foreach (Contact mcContact in mCContacts)
                {
                    if (mcContact != null)
                    {  
                         List<string> MCRole= new List<string>();
                         MCRole.Add(_userManagerRepository.GetContactTypeName_Code(mcContact.ContactType.GetHashCode()));

                        (mcUser,mcPassword)= _userManagerRepository.RegisterNewUserByRandomPassFromContactData(mcContact, MCRole).Result;

                        Contact savedMCContact;
                        if (mcUser != null)
                        {
                           savedMCContact= _userManagerRepository.SaveContactWithUserId(mcContact, mcUser.Id).Result;
                           savedmcContacts.Add(savedMCContact);
                            if(mcPassword != null && mcPassword != "") 
                                _emailSender.SendEmail("akbari.elahe@pidec.com", "", "New User And Password to login to QCMI System as MC", "your pass:" + mcPassword + " and user is :"+ mcUser.Email);
                        }
                            
                        }
                    }
                    _userManagerRepository.SaveProjectContacts( project, savedmcContacts);


                SystematicUser ownerUser;
                string ownerPass;
                for (int i = 0; i < ownerContacts.Count; i++)
                {

                    List<string> OwnerRole = new List<string>();
                    OwnerRole.Add(_userManagerRepository.GetContactTypeName_Code(ownerContacts[i].ContactType.GetHashCode()));

                    (ownerUser, ownerPass) = _userManagerRepository.RegisterNewUserByRandomPassFromContactData(ownerContacts[i], OwnerRole).Result;


                    if (ownerUser != null)
                    {
                        savedownerContacts.Add(_userManagerRepository.SaveContactWithUserId(ownerContacts[i], ownerUser.Id).Result);
                        if(ownerPass !=null && ownerPass != "")
                            _emailSender.SendEmail("akbari.elahe@pidec.com", "", "New User And Password to login to QCMI System as Owner", "your pass:" + ownerPass + " and user is :" + ownerUser.Email);
                    }
                }
                _userManagerRepository.SaveProjectContacts(project, savedownerContacts);

                SystematicUser pidecInspectorUser;
                string pidecInspectorPass;
                for (int i = 0; i < pidecInspectorsContacts.Count; i++)
                {

                    List<string> PidecInspectorRole = new List<string>();
                    PidecInspectorRole.Add(_userManagerRepository.GetContactTypeName_Code(pidecInspectorsContacts[i].ContactType.GetHashCode()));

                    (pidecInspectorUser, pidecInspectorPass) = _userManagerRepository.RegisterNewUserByRandomPassFromContactData(pidecInspectorsContacts[i], PidecInspectorRole).Result;


                    if (pidecInspectorUser != null)
                    {
                        savedpidecInspectorContacts.Add(_userManagerRepository.SaveContactWithUserId(pidecInspectorsContacts[i], pidecInspectorUser.Id).Result);
                        if(pidecInspectorPass != null && pidecInspectorPass != "")
                            _emailSender.SendEmail("akbari.elahe@pidec.com", "", "New User And Password to login to QCMI System as PIDEC Inspector", "your pass:" + pidecInspectorPass + " and user is :" + pidecInspectorUser.Email);
                    }
                }
                _userManagerRepository.SaveProjectContacts(project, savedpidecInspectorContacts);

                SystematicUser thirdPartInspectorUser;
                string thirdPartInspectorPass;
                for (int i = 0; i < tPInspectorsContacts.Count; i++)
                {

                    List<string> TpInspectorRole = new List<string>();
                    TpInspectorRole.Add(_userManagerRepository.GetContactTypeName_Code(tPInspectorsContacts[i].ContactType.GetHashCode()));

                    (thirdPartInspectorUser, thirdPartInspectorPass) = _userManagerRepository.RegisterNewUserByRandomPassFromContactData(tPInspectorsContacts[i], TpInspectorRole).Result;
                    


                    if (thirdPartInspectorUser != null)
                    {
                        savedthirdPartInspectorContacts.Add(_userManagerRepository.SaveContactWithUserId(tPInspectorsContacts[i], thirdPartInspectorUser.Id).Result);
                        if (thirdPartInspectorPass != null && thirdPartInspectorPass != "")
                            _emailSender.SendEmail("akbari.elahe@pidec.com", "", "New User And Password to login to QCMI System as ThirdPart Inspector", "your pass:" + thirdPartInspectorPass + " and user is :" + thirdPartInspectorUser.Email);
                    }
                }
                _userManagerRepository.SaveProjectContacts(project, savedthirdPartInspectorContacts);
            }
            catch (Exception exp) { }

            return await Task.FromResult(true);
        }


    }
}
