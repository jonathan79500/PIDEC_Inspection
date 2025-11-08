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
            Task<bool> result = ActivateProjectInspectionSystem(project, mcContacts, ownerContacts, pidecInspectorContacts, TPIContacts);
            return Task.FromResult(result.Result);
        }

        public async Task<bool> ActivateProjectInspectionSystem(Project project, List<Contact> mCContacts, List<Contact> ownerContacts, List<Contact> pidecInspectorsContacts, List<Contact> tPInspectorsContacts)
        {
            try
            {                
                SystematicUser mcUser;
                string mcPassword;
                List<Contact> savedmcContacts = new List<Contact>();
                List<Contact> savedownerContacts = new List<Contact>();
                List<Contact> savedpidecInspectorContacts = new List<Contact>();
                List<Contact> savedthirdPartInspectorContacts = new List<Contact>();

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
                           savedMCContact= _userManagerRepository.SaveContact(mcContact).Result;
                           savedmcContacts.Add(savedMCContact);
                            _emailSender.SendEmail("akbari.elahe@pidec.com", "", "New User And Password to login to QCMI System", "your pass:" + mcPassword + " and user is :"+ mcUser.Email);
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
                        savedownerContacts.Add(_userManagerRepository.SaveContact(ownerContacts[i]).Result);
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
                        savedpidecInspectorContacts.Add(_userManagerRepository.SaveContact(pidecInspectorsContacts[i]).Result);
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
                        savedthirdPartInspectorContacts.Add(_userManagerRepository.SaveContact(tPInspectorsContacts[i]).Result);
                      
                    }
                }
                _userManagerRepository.SaveProjectContacts(project, savedthirdPartInspectorContacts);
            }
            catch (Exception exp) { }

            return await Task.FromResult(true);
        }


    }
}
