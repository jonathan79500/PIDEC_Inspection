using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.ValueObjects;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Orders.Commands.ActivateEdmsOrderInInspectionSystem
{
    public class ActivateEdmsOrderInInspectionSystemCommandHandler : IRequestHandler<ActivateEdmsOrderInInspectionSystemCommand, bool>
    {
        private readonly IBasicInformationRepository basicInformationRepository;
        private IUserManagerRepository userManagerRepository;

        public ActivateEdmsOrderInInspectionSystemCommandHandler(IBasicInformationRepository _basicInformationRepository, IUserManagerRepository _userManagerRepository)
        {
            basicInformationRepository = _basicInformationRepository;
            userManagerRepository = _userManagerRepository;
        }
        public Task<bool> Handle(ActivateEdmsOrderInInspectionSystemCommand request, CancellationToken cancellationToken)
        {
            ContactDto contactDto = new ContactDto();
            contactDto.Id = Guid.NewGuid();
            
            Contact adaptedContact = contactDto.Adapt<Contact>();
            Contact[] exceptionContacts =  request.OrderPropForActivation.ExceptionContacts?.Adapt<Contact[]>();
            Project prj = request.OrderPropForActivation.Project.Adapt<Project>();

            Order newOrderForActivation = request.OrderPropForActivation.Adapt<Order>();
            Contact[] OrderContacts = request.OrderPropForActivation.Contacts.Adapt<Contact[]>();            
            InspectionSubject[] subjects = request.OrderPropForActivation.InspectionSubjects.Adapt<InspectionSubject[]>();
            var saveResult = ActivateEdmsOrderInInspectionSystem(newOrderForActivation,OrderContacts,subjects,exceptionContacts);
            return saveResult;
        }

        public async Task<bool> ActivateEdmsOrderInInspectionSystem(Order newOrderForActivation, Contact[] orderContacts, InspectionSubject[] subjects, Contact[] exceptionContacts)
        {
            try
            {

                var vendorContact = orderContacts
                .Where(ord => ord.ContactType == ContactType.Vendor.GetHashCode())
                .FirstOrDefault();

                try
                {

                    SystematicUser vendoruser = null;

                    if (vendorContact != null)
                    {
                        userManagerRepository.RegisterNewUserByRandomPassFromContactData(vendorContact, ["Vendor"]);
                    }

                    Order newOrderInInspectionSystem;
                    List<OrderContact> existedOrderContacts;
                    List<OrderInspectionSubject> existedOrderInspections;

                    Order ExistedOrder = basicInformationRepository.GetOrderByEdmsId(newOrderForActivation.EdmsId).Result;
                    if (ExistedOrder == null)
                    {
                        newOrderInInspectionSystem = Order.Create(newOrderForActivation.Name, newOrderForActivation.Title, newOrderForActivation.No, newOrderForActivation.Project, newOrderForActivation.EdmsId);
                        newOrderInInspectionSystem.OrderId = OrderId.CreateUnique();
                        newOrderInInspectionSystem.Id = newOrderInInspectionSystem.OrderId.Value;
                        newOrderInInspectionSystem.Project = newOrderForActivation.Project;
                        // dbContext.Entry(newOrderInInspectionSystem.Project).State = EntityState.Unchanged;
                        newOrderInInspectionSystem.ITP_NO = newOrderForActivation.ITP_NO;
                        newOrderInInspectionSystem.EdmsId = newOrderForActivation.EdmsId;
                        newOrderInInspectionSystem.Version = 0;
                        newOrderInInspectionSystem.Title = newOrderForActivation.Title;
                        newOrderInInspectionSystem.No = newOrderForActivation.No;
                        newOrderInInspectionSystem.Name = newOrderForActivation.Name;

                        newOrderInInspectionSystem.EquipmentTagNo = newOrderForActivation.EquipmentTagNo;
                        newOrderInInspectionSystem.CreatedDate = DateTime.Now;
                        newOrderInInspectionSystem.CreatedBy = newOrderForActivation.CreatedBy;
                        newOrderInInspectionSystem.ModifiedBy = newOrderForActivation.ModifiedBy;
                        newOrderInInspectionSystem.ModifiedDate = DateTime.Now;

                        basicInformationRepository.SaveOrder(newOrderInInspectionSystem);
                    }
                    else
                    {
                        existedOrderContacts = ExistedOrder.Contacts!.ToList(); // Use ToList() to avoid modifying the collection while iterating.

                        //dbContext.OrderContacts.RemoveRange(ExistedOrder.Contacts);
                        //dbContext.OrderInspectionSubjects.RemoveRange(ExistedOrder.Subjects);
                        List<Contact> deletedContacts;
                        Guid orderContactId;
                        basicInformationRepository.RemoveOrderContacts(existedOrderContacts);

                        existedOrderInspections = ExistedOrder.Subjects!.ToList(); // Use ToList() to avoid modifying the collection while iterating.
                        List<InspectionSubject> deletedInspectionSubjects;

                        Guid orderInspectionSubjectId;
                        basicInformationRepository.RemoveOrderInspectionSubjects(existedOrderInspections);

                        newOrderInInspectionSystem = ExistedOrder;
                        newOrderInInspectionSystem.OrderId = ExistedOrder.OrderId;
                        newOrderInInspectionSystem.Id = ExistedOrder.OrderId;
                        newOrderInInspectionSystem.Project = newOrderForActivation.Project;
                        newOrderInInspectionSystem.ITP_NO = newOrderForActivation.ITP_NO;
                        newOrderInInspectionSystem.Version = 0;
                        newOrderInInspectionSystem.Title = newOrderForActivation.Title;
                        newOrderInInspectionSystem.No = newOrderForActivation.No;
                        newOrderInInspectionSystem.Name = newOrderForActivation.Name;
                        newOrderInInspectionSystem.EquipmentTagNo = newOrderForActivation.EquipmentTagNo;

                        newOrderInInspectionSystem.ModifiedBy = newOrderForActivation.ModifiedBy;
                        newOrderInInspectionSystem.ModifiedDate = DateTime.Now;                       

                        basicInformationRepository.SaveOrder(newOrderInInspectionSystem);
                    }



                    basicInformationRepository.SaveOrderInspectionSubjects(subjects.ToList<InspectionSubject>(), newOrderInInspectionSystem);
                    basicInformationRepository.SaveOrderContacts(orderContacts.ToList<Contact>(), newOrderInInspectionSystem);
                    basicInformationRepository.SaveOrderExceptions(exceptionContacts.ToList<Contact>(), newOrderInInspectionSystem);




                }
                catch (Exception ex)
                {

                }

                return await Task.FromResult(true);

            } catch (Exception ex) {
                return await Task.FromResult(false);
            }

            
        }

    }
}
