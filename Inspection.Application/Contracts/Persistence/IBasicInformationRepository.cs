
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.Vendor;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Inspection.Application.Contracts.Persistence;

public interface IBasicInformationRepository
{
    Task<List<SystematicRole>> GetRoles(); 
    Task<List<VendorDocument>> GetVendorDocumentsByOrder(Contact vendor, Order order);
    Task<List<Order>> GetOrdersByProject(Project project);
    Task<List<Project>> GetProjects();
    Task<List<Project>> GetProjectsByVendor(Contact vendor);
    Task<BasicVendorInfo>  GetBasicVendorInfo(string vendorUserName,int vendorDomainId);
    Task<BasicQCMemberInfo> GetBasicQCMemberInfo(string QCMemberUserName, int QCMemberDomainId);
    Task<BasicInspectorInfo> GetBasicInspectorInfo(string InspectorUserName, int InspectorDomainId);
    Task<BasicPIDECLeadsInfo> GetPIDECLeadInfo(string LeadUserName, int LeadDomainId);
    Task<bool> ActivateEdmsOrderInInspectionSystem(Order order, Contact[] orderContacts, InspectionSubject[] subjects, Contact[] exceptionContacts);
    Task<Order> GetEdmsOrderDataInQCIMS(int EdmsOrderId,Guid projectId);
    Task<Project> GetProjectDataInQCIMS(Guid ProjectId);
    Task<Order> GetOrderByEdmsId(string edmsId);
    Task<Order> SaveOrder(Order order);
    Task<bool> RemoveOrderContacts(List<OrderContact> orderContacts);
    Task<bool> RemoveOrderInspectionSubjects(List<OrderInspectionSubject> orderInspectionSubjects);
    Task<bool> SaveOrderInspectionSubjects(List<InspectionSubject> InspectionSubjects,Order order);
    Task<bool> SaveOrderContacts(List<Contact> Contacts, Order order);
    Task<bool> SaveOrderExceptions(List<Contact> ExceptionContacts, Order order);

    //Task<bool> ActivateProjectInspectionSystem(Project project, List<Contact> mCContacts, List<Contact> ownerContacts, List<Contact> pidecInspectorsContacts, List<Contact> tPInspectorsContacts);
}
