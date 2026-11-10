

using Dapper;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.Models;
using Inspection.Domain.Constants;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.ValueObjects;
using Inspection.Domain.Entities.Vendor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Data;

using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;




namespace Inspection.Persistence.Repositories;

public class BasicInformationRepository : IBasicInformationRepository
{
    private readonly InspectionDbContext _dbcontext;
    private readonly IEDMSService eDMSService;
    private UserManager<SystematicUser> _userManager;
    private RoleManager<SystematicRole> _roleManager;
    private IServiceScopeFactory _serviceProvider;
    public BasicInformationRepository(InspectionDbContext dbContext, IEDMSService eDMSService, UserManager<SystematicUser> userManager,RoleManager<SystematicRole> roleManager, IServiceScopeFactory serviceProvider)
    {
        _dbcontext = dbContext;
        this.eDMSService = eDMSService;
        _userManager = userManager;
        _serviceProvider = serviceProvider;
        _roleManager = roleManager;
    }

    public Task<List<Order>> GetOrdersByProject(Project project)
    {
        List<Order> projectOrders = _dbcontext.Orders.Where(ord => ord.Project.ProjectId == project.ProjectId).ToList();
        return Task.FromResult(projectOrders);
    }

    public Task<List<Project>> GetProjects()
    {
        List<Project> projects = _dbcontext.Projects.ToList();
        return Task.FromResult(projects);
    }

    public Task<Attachment> GetAttachmentById(Guid id) {
        AttachmentId attachmentId = new AttachmentId(id);
        var result = _dbcontext.Attachments.FirstOrDefault(a => a.AttachmentId == attachmentId);
       return Task.FromResult(result);
    }

    public Task<List<VendorDocument>> GetVendorDocumentsByOrder(Contact vendor, Order order)
    {
        List<VendorDocument> vendorDocuments = _dbcontext.Contacts
                                                                .Where(v => v.ContactId == vendor.ContactId).FirstOrDefault()!.Orders
                                                                .Where(ord => ord.Id == order.Id).FirstOrDefault()!.Order.VendorDocuments.ToList();

     

        return Task.FromResult(vendorDocuments);
    }

    public Task<Order> GetOrderByEdmsId(string edmsId) {
        Order ExistedOrder = _dbcontext.Orders//.AsNoTracking()
                            .Include(o => o.Contacts)
                            .Include(o => o.Subjects)
                            .FirstOrDefault(o => o.EdmsId == edmsId);        
        return Task.FromResult(ExistedOrder);
    }

    public Task<Order> SaveOrder(Order order) {
        Order addedOrder;
        try
        {
            _dbcontext.Projects.Attach(order.Project);
            //_dbcontext.Entry(order.Project).State = EntityState.Unchanged;
            Guid currentOrderId = order.OrderId.Value;
            var existedOrder = _dbcontext.Orders.Where(ord => ord.OrderId == currentOrderId).FirstOrDefault();
            if (existedOrder == null)
            {
                addedOrder = _dbcontext.Orders.Add(order).Entity;
                _dbcontext.SaveChanges();
                return Task.FromResult(addedOrder);
            }
            else
            {
                return Task.FromResult(existedOrder);
            }
        }
        catch (Exception ex) {
            return null;
        }
    }
    public Task<BasicVendorInfo> GetBasicVendorInfo(string vendorUserName, int vendorDomainId)
    {

        try
        {

            Contact? vendorMemberData = new Contact();
            vendorMemberData = _dbcontext.Contacts.Where(predicate: v => v.UserName == vendorUserName).FirstOrDefault(); // && v.DomainId == vendorDomainId

            var connection = _dbcontext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();


            var adoTransaction =
               _dbcontext.Database.CurrentTransaction?.GetDbTransaction();

            List<Project> projects;
            List<Order> orders;
            List<InspectionType> inspectionTypes;
            List<InspectionSubject> inspectionSubjects;
            List<OrderInspectionSubject> orderInspectionSubjects;
            List<OrderContact> orderContacts;
            List<ProjectContact> projectContacts;
            List<Circulation> participatedCirculations;
            List<OrdersettingException> ordersettingExceptions;
            List<DocumentType> documentTypes;
            List<DocumentTypeVisibility> documentTypeVisibilities;

            using (var multi = connection.QueryMultiple(
            "GetBesicInfo_VendorContact",
            new { userName = vendorUserName },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {

                projects = multi.Read().Select(p => new Project()
                {
                    ProjectId = new ProjectId(new Guid(p.ProjectId.ToString())),
                    FSId = p.FSId,
                    Number = p.Number,
                    Title = p.Title,
                    Description = p.Description,
                    //ModifiedDate = p.ModifiedDate,
                    //CreatedBy = p.CreatedBy,
                    //CreatedDate = p.CreatedDate,                   
                    //ModifiedBy = p.ModifiedBy,

                }).ToList();

                //--------------Project Contacts--------------------------------
                projectContacts = multi.Read<ProjectContact, Contact, ProjectContact>(
                (projectContact, contact) =>
                {
                    projectContact.Contact = contact;
                    if (projectContact?.ProjectId! != null)
                    {
                        projectContact.ProjectId = ProjectId.Create(new Guid(projectContact.ProjectId.Value.ToString()));
                    }
                    projectContact.ContactId = contact.ContactId;
                    projectContact.Id = projectContact.Id;
                    return projectContact;
                },
                splitOn: "ContactId"
                ).ToList();

                //-------------------Order Settings Exceptions--------------------------

                ordersettingExceptions = multi.Read<OrdersettingException, ProjectContact, Contact, OrdersettingException>(
     (orderSettingException, projectContact, contact) =>
     {
         projectContact.Contact = contact;
         orderSettingException.ProjectContact = projectContact;

         if (orderSettingException?.OrderId != null)
         {
             orderSettingException.OrderId = OrderId.Create(new Guid(orderSettingException.OrderId.Value.ToString()));
         }

         orderSettingException.ProjectContactId = projectContact.Id;
         return orderSettingException;
     },
     splitOn: "ProjectContactId,ContactId"
 ).ToList();

                //ordersettingExceptions = multi.Read().Select(o => new OrdersettingException()
                //{
                //    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                //    ProjectContactId = new Guid(o.ProjectContactId.ToString())

                //}).ToList();

                //------------------Orders -----------------------------------------------

                orders = multi.Read().Select(o => new Order()
                {
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    Name = o.Name,
                    ModifiedDate = o.ModifiedDate,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedDate,
                    EdmsId = o.EdmsId,
                    EquipmentTagNo = o.EquipmentTagNo,
                    Id = o.Id,
                    ITP_NO = o.ITP_NO,
                    ModifiedBy = o.ModifiedBy,
                    No = o.No,
                    Version = o.Version,
                    Subjects = o.Subjects,
                    Project = o.Project,
                    Title = o.Title,

                }).ToList();




                //---------------InspectionTypes-----------------------------------------
                inspectionTypes = multi.Read().Select(it => new InspectionType()
                {
                    InspectionTypeId = new InspectionTypeId(new Guid(it.InspectionTypeId.ToString())),
                    Name = it.Name

                }).ToList();

                //---------------InspectionSubjects-----------------------------------------
                inspectionSubjects = multi.Read().Select(isu => new InspectionSubject()
                {
                    InspectionSubjectId = new InspectionSubjectId(new Guid(isu.InspectionSubjectId.ToString())),
                    Name = isu.Name

                }).ToList();

                // Fourth result set

                orderInspectionSubjects = multi.Read<OrderInspectionSubject, InspectionSubject, OrderInspectionSubject>(
         (orderInspectionSubject, inspectionSubject) =>
         {
             orderInspectionSubject.InspectionSubject = inspectionSubject;
             if (orderInspectionSubject?.OrderId! != null)
             {
                 orderInspectionSubject.OrderId = OrderId.Create(new Guid(orderInspectionSubject.OrderId.Value.ToString()));
             }
             orderInspectionSubject!.InspectionSubjectId = inspectionSubject.InspectionSubjectId;
             orderInspectionSubject.Id = orderInspectionSubject.Id;
             return orderInspectionSubject;
         },
         splitOn: "InspectionSubjectId"
     ).ToList();

                //orderInspectionSubjects = multi.Read().Select(o => new OrderInspectionSubject()
                //{
                //    InspectionSubjectId = new InspectionSubjectId(new Guid(o.InspectionSubjectId.ToString())),
                //    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                //    //ModifiedDate = o.ModifiedDate,
                //    //CreatedDate = o.CreatedDate,
                //    //CreatedBy = o.CreatedBy,
                //    //ModifiedBy = o.ModifiedBy,
                //    Id = o.Id
                //}).ToList();


                // Fourth result set
                orderContacts = multi.Read<OrderContact, Contact, OrderContact>(
           (orderContact, contact) =>
           {
               orderContact.Contact = contact;
               if (orderContact.Contact.ContactId == null)
                   orderContact.Contact.ContactId = contact.ContactId;
               if (orderContact?.OrderId! != null)
               {
                   orderContact.OrderId = OrderId.Create(new Guid(orderContact.OrderId.Value.ToString()));
               }
               orderContact.ContactId = contact.ContactId;
               orderContact.Id = orderContact.Id;
               return orderContact;
           },
           splitOn: "ContactId"
       ).ToList();

                //orderContacts = multi.Read().Select(o => new OrderContact()
                //{
                //    ContactId = new ContactId(new Guid(o.ContactId.ToString())),
                //    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                //    Id = o.Id
                //}).ToList();


                // Fifth result set
                //participatedCirculations = multi.Read().Select(cr => new Circulation()
                //{

                //    ActionType = GetActionType_ById(cr.ActionType.ToString()),
                //    CreatedBy = cr.CreatedBy,
                //    CreatedDate = cr.CreatedDate,
                //    ModifiedDate = cr.ModifiedDate,
                //    ModifiedBy = cr.ModifiedBy
                //}).ToList();
                participatedCirculations = multi.Read<Circulation, Contact, Revision, Circulation>(

                    (circulation, contact, revision) =>     
                    {
                        circulation.PerformerContactPerson = contact;
                        circulation.Revision = revision;
                        return circulation;    
                    },
                splitOn: "ContactId,RequestId"   // tells Dapper where each new object starts
).ToList();

                //---------------DocumentTypes-----------------------------------------
                documentTypes = multi.Read().Select(dt => new DocumentType()
                {
                    DocumentTypeId = dt.DocumentTypeId,
                    AllowedExtensions = dt.AllowedExtensions,
                    DocumentTypeCode= dt.DocumentTypeCode,
                    DocumentTypeName = dt.DocumentTypeName,               
                    MaxSizeMB = dt.MaxSizeMB,
                    MultipleAllowed= dt.MultipleAllowed,
                    RequiresApproval= dt.RequiresApproval,
                    RequiresName= dt.RequiresName,
                    RequiresNumber = dt.RequiresNumber,
                    RequiresReview = dt.RequiresReview

                }).ToList();

                //---------------DocumentTypeVisibility-----------------------------------------
                documentTypeVisibilities = multi.Read().Select(dtv => new DocumentTypeVisibility()
                {
                    ContactTypeCode = dtv.ContactTypeCode,
                     DocumentTypeId = dtv.DocumentTypeId

                }).ToList();


            }

            //  connection.Close();


            BasicVendorInfo? basicVendorMemberInfo = new BasicVendorInfo();

            //basicQCMemberInfo.SetOrders(qcMemberData?.Orders.ToList<OrderQCContactPerson>());
            //basicQCMemberInfo.MergeWithASet(qcMemberData);
            //string[] projectIds = qcMemberData?.Orders?.Select(o => o.Order.Project.Id.ToString()).ToArray<string>();
            //basicQCMemberInfo.SetEdmsOrders(eDMSService.GetProjectOrders(projectIds).Result);
            basicVendorMemberInfo.UserName = vendorUserName;
            basicVendorMemberInfo.DomainId = vendorDomainId;
            basicVendorMemberInfo.Name = vendorUserName;
            basicVendorMemberInfo.Orders = orders;
            basicVendorMemberInfo.Projects = projects;
            basicVendorMemberInfo.InspectionTypes = inspectionTypes;
            basicVendorMemberInfo.InspectionSubjects = inspectionSubjects;
            basicVendorMemberInfo.OrderInspectionSubjects = orderInspectionSubjects;
            basicVendorMemberInfo.OrderContacts = orderContacts;
            basicVendorMemberInfo.Participated_Circulations = participatedCirculations;
            basicVendorMemberInfo.OrderSettingsExceptions = ordersettingExceptions;
            basicVendorMemberInfo.ProjectContacts = projectContacts;

            return Task.FromResult(basicVendorMemberInfo);
        }
        catch (NullReferenceException aex)
        {
            var ttt = aex.Message;
            throw new Exception(ttt);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            //  return null;
        }

    }

    public Task<BasicInspectorInfo> GetBasicInspectorInfo(string inspectorUserName, int inspectorDomainId)
    {

        try
        {

            Contact? inspectorMemberData = new Contact();
            inspectorMemberData = _dbcontext.Contacts.Where(predicate: v => v.UserName == inspectorUserName).FirstOrDefault(); // && v.DomainId == vendorDomainId

            var connection = _dbcontext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();


            var adoTransaction =
               _dbcontext.Database.CurrentTransaction?.GetDbTransaction();

            List<Project> projects;
            List<Order> orders;
            List<InspectionType> inspectionTypes;
            List<InspectionSubject> inspectionSubjects;
            List<OrderInspectionSubject> orderInspectionSubjects;
            List<OrderContact> orderContacts;
            List<InspectionNotification> inspectionNotifications;
            List<Circulation> participatedCirculations;
            List<OrdersettingException> ordersettingExceptions;
            List<DocumentType> documentTypes;
            List<DocumentTypeVisibility> documentTypeVisibilities;

            using (var multi = connection.QueryMultiple(
            "GetBesicInfo_InspectorContact",
            new { userName = inspectorUserName },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {

                projects = multi.Read().Select(p => new Project()
                {
                    ProjectId = new ProjectId(new Guid(p.ProjectId.ToString())),
                    FSId = p.FSId,
                    Number = p.Number,
                    Title = p.Title,
                    Description = p.Description,
                    //ModifiedDate = p.ModifiedDate,
                    //CreatedBy = p.CreatedBy,
                    //CreatedDate = p.CreatedDate,                   
                    //ModifiedBy = p.ModifiedBy,

                }).ToList();

                inspectionNotifications = multi.Read().Select(ins => new InspectionNotification()
                {
                    InspectionNotificationId = new InspectionNotificationId(new Guid(ins.InspectionNotificationId.ToString())),
                    NotificationNumber = ins.NotificationNumber,
                    IssueDate = ins.IssueDate,
                    RequestInspectionDateStart = ins.RequestInspectionDateStart,
                    RequestInspectionDateEnd = ins.RequestInspectionDateEnd,
                    RequestInspectionDueDate = ins.RequestInspectionDueDate,
                   // RequestInspectionPeriodDays = ins.RequestInspectionPeriodDays,
                    RequestInspectionPersianDateEnd = ins.RequestInspectionPersianDateEnd,
                    RequestInspectionPersianDateStart = ins.RequestInspectionPersianDateStart,
                    //RequestInspectionTimesInPeriod = ins.RequestInspectionTimesInPeriod,                    
                    InspectionAction = ins.InspectionAction,
                    InspectionActionComment = ins.InspectionActionComment,
                    InspectionTimeComment= ins.InspectionTimeComment,
                    Description = ins.Description,
                    PersianIssueDate = ins.PersianIssueDate,
                    Name = ins.Name
               
                    //ModifiedDate = p.ModifiedDate,
                    //CreatedBy = p.CreatedBy,
                    //CreatedDate = p.CreatedDate,                   
                    //ModifiedBy = p.ModifiedBy,

                }).ToList();

                //------------------Orders -----------------------------------------------

                orders = multi.Read().Select(o => new Order()
                {
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    Name = o.Name,
                    ModifiedDate = o.ModifiedDate,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedDate,
                    EdmsId = o.EdmsId,
                    EquipmentTagNo = o.EquipmentTagNo,
                    Id = o.Id,
                    ITP_NO = o.ITP_NO,
                    ModifiedBy = o.ModifiedBy,
                    No = o.No,
                    Version = o.Version,
                    Subjects = o.Subjects,
                    Project = o.Project,
                    Title = o.Title,

                }).ToList();

                //---------------InspectionTypes-----------------------------------------
                inspectionTypes = multi.Read().Select(it => new InspectionType()
                {
                    InspectionTypeId = new InspectionTypeId(new Guid(it.InspectionTypeId.ToString())),
                    Name = it.Name

                }).ToList();

                //---------------InspectionSubjects-----------------------------------------
                inspectionSubjects = multi.Read().Select(isu => new InspectionSubject()
                {
                    InspectionSubjectId = new InspectionSubjectId(new Guid(isu.InspectionSubjectId.ToString())),
                    Name = isu.Name

                }).ToList();

                // Fourth result set

                orderInspectionSubjects = multi.Read<OrderInspectionSubject, InspectionSubject, OrderInspectionSubject>(
         (orderInspectionSubject, inspectionSubject) =>
         {
             orderInspectionSubject.InspectionSubject = inspectionSubject;
             if (orderInspectionSubject?.OrderId! != null)
             {
                 orderInspectionSubject.OrderId = OrderId.Create(new Guid(orderInspectionSubject.OrderId.Value.ToString()));
             }
             orderInspectionSubject!.InspectionSubjectId = inspectionSubject.InspectionSubjectId;
             orderInspectionSubject.Id = orderInspectionSubject.Id;
             return orderInspectionSubject;
         },
         splitOn: "InspectionSubjectId"
     ).ToList();

                // Fifth result set

                participatedCirculations = multi.Read<Circulation, Contact, Revision, Circulation>(

                (circulation, contact, revision) =>
                {
                    circulation.PerformerContactPerson = contact;
                    circulation.Revision = revision;
                    return circulation;
                },
            splitOn: "ContactId,RequestId"   // tells Dapper where each new object starts
).ToList();

                //---------------DocumentTypes-----------------------------------------
                documentTypes = multi.Read().Select(dt => new DocumentType()
                {
                    DocumentTypeId = dt.DocumentTypeId,
                    AllowedExtensions = dt.AllowedExtensions,
                    DocumentTypeCode = dt.DocumentTypeCode,
                    DocumentTypeName = dt.DocumentTypeName,
                 
                    MaxSizeMB = dt.MaxSizeMB,
                    MultipleAllowed = dt.MultipleAllowed,
                    RequiresApproval = dt.RequiresApproval,
                    RequiresName = dt.RequiresName,
                    RequiresNumber = dt.RequiresNumber,
                    RequiresReview = dt.RequiresReview

                }).ToList();

                //---------------DocumentTypeVisibility-----------------------------------------
                documentTypeVisibilities = multi.Read().Select(dtv => new DocumentTypeVisibility()
                {
                    ContactTypeCode = dtv.ContactTypeCode,
                    DocumentTypeId = dtv.DocumentTypeId

                }).ToList();

            }

            //  connection.Close();


            BasicInspectorInfo? basicInspectorInfo = new BasicInspectorInfo();

            //basicQCMemberInfo.SetOrders(qcMemberData?.Orders.ToList<OrderQCContactPerson>());
            //basicQCMemberInfo.MergeWithASet(qcMemberData);
            //string[] projectIds = qcMemberData?.Orders?.Select(o => o.Order.Project.Id.ToString()).ToArray<string>();
            //basicQCMemberInfo.SetEdmsOrders(eDMSService.GetProjectOrders(projectIds).Result);
            basicInspectorInfo.UserName = inspectorUserName;
            basicInspectorInfo.DomainId = inspectorDomainId;
            basicInspectorInfo.Name = inspectorUserName;
            basicInspectorInfo.Orders = orders;
            basicInspectorInfo.Projects = projects;
            basicInspectorInfo.InspectionTypes = inspectionTypes;
            basicInspectorInfo.InspectionSubjects = inspectionSubjects;
            basicInspectorInfo.OrderInspectionSubjects = orderInspectionSubjects;
          //  basicInspectorInfo.OrderContacts = orderContacts;
            basicInspectorInfo.Participated_Circulations = participatedCirculations;     

            return Task.FromResult(basicInspectorInfo);
        }
        catch (NullReferenceException aex)
        {
            var ttt = aex.Message;
            throw new Exception(ttt);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            //  return null;
        }

    }
    //public Task<BasicVendorInfo> GetBasicVendorInfo(string vendorUserName, int vendorDomainId)
    //{

    //    try
    //    {

    //        Contact? vendorMemberData = new Contact();
    //        vendorMemberData = _dbcontext.Contacts.Where(predicate: v => v.UserName == vendorUserName).FirstOrDefault(); // && v.DomainId == vendorDomainId

    //        var connection = _dbcontext.Database.GetDbConnection();
    //        // ensure the connection is open
    //        if (connection.State != ConnectionState.Open)
    //            connection.Open();


    //        var adoTransaction =
    //           _dbcontext.Database.CurrentTransaction?.GetDbTransaction();

    //        List<Project> projects;
    //        List<Order> orders;
    //        List<InspectionType> inspectionTypes;   
    //        List<InspectionSubject> inspectionSubjects;
    //        List<OrderInspectionSubject> orderInspectionSubjects;
    //        List<OrderContact> orderContacts;
    //        List<Circulation> participatedCirculations;

    //        using (var multi = connection.QueryMultiple(
    //        "GetBesicInfo_VendorContact",
    //        new { userName = vendorUserName },
    //        commandType: CommandType.StoredProcedure,
    //        transaction: adoTransaction))
    //        {

    //            projects = multi.Read().Select(p => new Project()
    //            {
    //                ProjectId = new ProjectId(new Guid(p.ProjectId.ToString())),
    //                FSId = p.FSId,
    //                Number = p.Number,
    //                Title = p.Title,
    //                Description = p.Description,
    //                //ModifiedDate = p.ModifiedDate,
    //                //CreatedBy = p.CreatedBy,
    //                //CreatedDate = p.CreatedDate,                   
    //                //ModifiedBy = p.ModifiedBy,

    //            }).ToList();


    //            // projects = multi.Read<Project>().ToList();

    //            orders = multi.Read().Select(o => new Order()
    //            {
    //                OrderId = new OrderId(new Guid(o.OrderId.ToString())),
    //                Name = o.Name,
    //                ModifiedDate = o.ModifiedDate,
    //                CreatedBy = o.CreatedBy,
    //                CreatedDate = o.CreatedDate,
    //                EdmsId = o.EdmsId,
    //                EquipmentTagNo = o.EquipmentTagNo,
    //                Id = o.Id,
    //                ITP_NO = o.ITP_NO,
    //                ModifiedBy = o.ModifiedBy,
    //                No = o.No,
    //                Version = o.Version,
    //                Subjects = o.Subjects,
    //                Project = o.Project,
    //                Title = o.Title,

    //            }).ToList();




    //            // Second result set
    //            inspectionTypes = multi.Read().Select(it => new InspectionType()
    //            {
    //                InspectionTypeId = new InspectionTypeId(new Guid(it.InspectionTypeId.ToString())),
    //                Name = it.Name

    //            }).ToList();

    //            // Second result set
    //            inspectionSubjects = multi.Read().Select(isu => new InspectionSubject()
    //            {
    //                InspectionSubjectId = new InspectionSubjectId(new Guid(isu.InspectionSubjectId.ToString())),
    //                Name = isu.Name

    //            }).ToList();

    //            // Fourth result set

    //            orderInspectionSubjects = multi.Read<OrderInspectionSubject, InspectionSubject, OrderInspectionSubject>(
    //     (orderInspectionSubject, inspectionSubject) =>
    //     {
    //         orderInspectionSubject.InspectionSubject = inspectionSubject;
    //         if (orderInspectionSubject?.OrderId! != null)
    //         {
    //             orderInspectionSubject.OrderId = OrderId.Create(new Guid(orderInspectionSubject.OrderId.Value.ToString()));
    //         }
    //         orderInspectionSubject!.InspectionSubjectId = inspectionSubject.InspectionSubjectId;
    //         orderInspectionSubject.Id = orderInspectionSubject.Id;
    //         return orderInspectionSubject;
    //     },
    //     splitOn: "InspectionSubjectId"
    // ).ToList();

    //            //orderInspectionSubjects = multi.Read().Select(o => new OrderInspectionSubject()
    //            //{
    //            //    InspectionSubjectId = new InspectionSubjectId(new Guid(o.InspectionSubjectId.ToString())),
    //            //    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
    //            //    //ModifiedDate = o.ModifiedDate,
    //            //    //CreatedDate = o.CreatedDate,
    //            //    //CreatedBy = o.CreatedBy,
    //            //    //ModifiedBy = o.ModifiedBy,
    //            //    Id = o.Id
    //            //}).ToList();


    //            // Fourth result set
    //            orderContacts = multi.Read<OrderContact, Contact, OrderContact>(
    //       (orderContact, contact) =>
    //       {
    //           orderContact.Contact = contact;
    //           if (orderContact?.OrderId! != null)
    //           {
    //               orderContact.OrderId = OrderId.Create(new Guid(orderContact.OrderId.Value.ToString()));
    //           }
    //           orderContact.ContactId = contact.Id;
    //           orderContact.Id = orderContact.Id;
    //           return orderContact;
    //       },
    //       splitOn: "ContactId"
    //   ).ToList();

    //            //orderContacts = multi.Read().Select(o => new OrderContact()
    //            //{
    //            //    ContactId = new ContactId(new Guid(o.ContactId.ToString())),
    //            //    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
    //            //    Id = o.Id
    //            //}).ToList();


    //            // Fifth result set
    //            participatedCirculations = multi.Read().Select(cr => new Circulation()
    //            {

    //                ActionType = GetActionType_ById(cr.ActionType.ToString()),
    //                CreatedBy = cr.CreatedBy,
    //                CreatedDate = cr.CreatedDate,
    //                ModifiedDate = cr.ModifiedDate,
    //                ModifiedBy = cr.ModifiedBy
    //            }).ToList();

    //        }

    //      //  connection.Close();


    //        BasicVendorInfo? basicVendorMemberInfo = new BasicVendorInfo();

    //        //basicQCMemberInfo.SetOrders(qcMemberData?.Orders.ToList<OrderQCContactPerson>());
    //        //basicQCMemberInfo.MergeWithASet(qcMemberData);
    //        //string[] projectIds = qcMemberData?.Orders?.Select(o => o.Order.Project.Id.ToString()).ToArray<string>();
    //        //basicQCMemberInfo.SetEdmsOrders(eDMSService.GetProjectOrders(projectIds).Result);
    //        basicVendorMemberInfo.UserName = vendorUserName;
    //        basicVendorMemberInfo.DomainId = vendorDomainId;
    //        basicVendorMemberInfo.Name = vendorUserName;
    //        basicVendorMemberInfo.Orders = orders;
    //        basicVendorMemberInfo.Projects = projects;
    //        basicVendorMemberInfo.InspectionTypes = inspectionTypes;
    //        basicVendorMemberInfo.InspectionSubjects = inspectionSubjects;
    //        basicVendorMemberInfo.OrderInspectionSubjects = orderInspectionSubjects;
    //        basicVendorMemberInfo.OrderContacts = orderContacts;
    //        basicVendorMemberInfo.Participated_Circulations = participatedCirculations;

    //        return Task.FromResult(basicVendorMemberInfo);
    //    }
    //    catch (NullReferenceException aex)
    //    {
    //        var ttt = aex.Message;
    //        throw new Exception(ttt);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //        //  return null;
    //    }

    //}

    public Task<List<Project>> GetProjectsByVendor(Contact vendor)
    {
        throw new NotImplementedException();
    }

    public Task<List<SystematicRole>> GetRoles()
    {
        return Task.FromResult(_dbcontext.Roles.ToList());
    }

    public ActionType GetActionType_ById(string ActionTypeId)
    {
       
       ActionType resultl=null;
        if(ActionTypeId != null)
            resultl = _dbcontext.ActionTypes.Where(at => at.ActionTypeId.ToString() == ActionTypeId).FirstOrDefault();

        return resultl;
    }

    public Task<BasicQCMemberInfo> GetBasicQCMemberInfo(string QCMemberUserName, int QCMemberDomainId)
    {

        try
        {

            Contact? qcMemberData = new Contact();
            qcMemberData = _dbcontext.Contacts.Where(predicate: v => v.UserName == QCMemberUserName && v.DomainId == QCMemberDomainId).FirstOrDefault();

            var connection = _dbcontext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();


            var adoTransaction =
               _dbcontext.Database.CurrentTransaction?.GetDbTransaction();

            List<Project> projects;
            List<Order> orders;
            List<InspectionType> inspectionTypes;
            List<InspectionSubject> inspectionSubjects;
            List<OrderInspectionSubject> orderInspectionSubjects;
            List<OrderContact> orderContacts;
            List<Circulation> participatedCirculations;
            List<Contact> usefulContacts;
            List<DocumentType> documentTypes;
            List<DocumentTypeVisibility> documentTypeVisibilities;

            using (var multi = connection.QueryMultiple(
            "GetBesicInfo_QCMembersContact",
            new { userName = QCMemberUserName },
            commandType: CommandType.StoredProcedure,
            transaction:adoTransaction
            ))
            {

                projects = multi.Read().Select(p => new Project()
                {
                    ProjectId = new ProjectId(new Guid(p.ProjectId.ToString())),
                    FSId = p.FSId,
                    Number = p.Number,
                    Title = p.Title,
                    Description = p.Description,
                    Id = new ProjectId(new Guid(p.ProjectId.ToString())),
                    ModifiedDate = p.ModifiedDate,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedDate,
                    ModifiedBy = p.ModifiedBy,

                }).ToList();


                // projects = multi.Read<Project>().ToList();

                orders = multi.Read().Select(o => new Order()
                {
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    Name = o.Name,
                    ModifiedDate = o.ModifiedDate,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedDate,
                    EdmsId = o.EdmsId,
                    EquipmentTagNo = o.EquipmentTagNo,
                    Id = o.Id,
                    ITP_NO = o.ITP_NO,
                    ModifiedBy = o.ModifiedBy,
                    No = o.No,
                    Version = o.Version,
                    Subjects = o.Subjects,
                    Project = o.Project,
                    Title = o.Title,

                }).ToList();




                // Second result set
                inspectionTypes = multi.Read().Select(it => new InspectionType()
                {
                    InspectionTypeId = new InspectionTypeId(new Guid(it.InspectionTypeId.ToString())),
                    Name = it.Name
                }).ToList();


                // Second result set
                inspectionSubjects = multi.Read().Select(isu => new InspectionSubject()
                {
                    InspectionSubjectId = new InspectionSubjectId(new Guid(isu.InspectionSubjectId.ToString())),
                    Name = isu.Name
                }).ToList();

                // Third result set
                orderInspectionSubjects = multi.Read().Select(o => new OrderInspectionSubject()
                {
                    InspectionSubjectId = new InspectionSubjectId(new Guid(o.InspectionSubjectId.ToString())),
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    //ModifiedDate = o.ModifiedDate,
                    //CreatedDate = o.CreatedDate,
                    //CreatedBy = o.CreatedBy,
                    //ModifiedBy = o.ModifiedBy,
                    Id = o.Id
                }).ToList();


                // Fourth result set
                orderContacts = multi.Read().Select(o => new OrderContact()
                {
                    ContactId = new ContactId(new Guid(o.ContactId.ToString())),
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    Id = o.Id
                }).ToList();


                // Fifth result set
                //participatedCirculations = multi.Read().Select(cr => new Circulation()
                //{
                //    PerformerContactPerson = cr.PerformerContactPerson,
                //   //  ActionType = GetActionType_ById(cr.ActionType),
                //    CreatedBy = cr.CreatedBy,
                //    CreatedDate = cr.CreatedDate==null ? null:cr.CreatedDate,
                //    ModifiedDate = cr.ModifiedDate,
                //    ModifiedBy = cr.ModifiedBy
                //}).ToList();

                participatedCirculations = multi.Read<Circulation, Contact, Revision, Circulation>(

                  (circulation, contact, revision) =>
                  {
                      circulation.PerformerContactPerson = contact;
                      circulation.Revision = revision;
                      return circulation;
                  },
              splitOn: "ContactId,RequestId"   // tells Dapper where each new object starts
).ToList();

                usefulContacts = multi.Read().Select(co=>new Contact()                 
                {                    
                    Name = co.Name,
                    Email = co.Email,
                    Phone = co.Phone == null ? "" : co.Phone,
                    Cc = co.Cc,
                    Address = co.Address,
                    Note = co.Note,
                    ContactType = co.ContactType,
                    CreatedBy = "",
                    CreatedDate = DateTime.Now,
                    ModifiedBy = "",
                    ModifiedDate = DateTime.Now,
                    HomePage = co.HomePage,
                    Fax = co.Fax,
                    DomainId = co.DomainId,
                    Mobile = co.Mobile,
                    IsVisible = co.IsVisible,
                    UserName = co.UserName != "" ? co.UserName : co.Name,
                    ContactId = co.ContactId                   
                }
                ).ToList();

                //---------------DocumentTypes-----------------------------------------
                documentTypes = multi.Read().Select(dt => new DocumentType()
                {
                    DocumentTypeId = dt.DocumentTypeId,
                    AllowedExtensions = dt.AllowedExtensions,
                    DocumentTypeCode = dt.DocumentTypeCode,
                    DocumentTypeName = dt.DocumentTypeName,
                   
                    MaxSizeMB = dt.MaxSizeMB,
                    MultipleAllowed = dt.MultipleAllowed,
                    RequiresApproval = dt.RequiresApproval,
                    RequiresName = dt.RequiresName,
                    RequiresNumber = dt.RequiresNumber,
                    RequiresReview = dt.RequiresReview

                }).ToList();

                //---------------DocumentTypeVisibility-----------------------------------------
                documentTypeVisibilities = multi.Read().Select(dtv => new DocumentTypeVisibility()
                {
                    ContactTypeCode = dtv.ContactTypeCode,
                    DocumentTypeId = dtv.DocumentTypeId

                }).ToList();

            }

          //  connection.Close();


            BasicQCMemberInfo? basicQCMemberInfo = new BasicQCMemberInfo();

            //basicQCMemberInfo.SetOrders(qcMemberData?.Orders.ToList<OrderQCContactPerson>());
            //basicQCMemberInfo.MergeWithASet(qcMemberData);
            //string[] projectIds = qcMemberData?.Orders?.Select(o => o.Order.Project.Id.ToString()).ToArray<string>();
            //basicQCMemberInfo.SetEdmsOrders(eDMSService.GetProjectOrders(projectIds).Result);

            basicQCMemberInfo.Orders = orders;
            basicQCMemberInfo.Projects = projects;
            basicQCMemberInfo.InspectionTypes = inspectionTypes;
            basicQCMemberInfo.OrderInspectionSubjects = orderInspectionSubjects;
            basicQCMemberInfo.OrderContacts = orderContacts;
            basicQCMemberInfo.Participated_Circulations = participatedCirculations;
            basicQCMemberInfo.UsefulContacts = usefulContacts;

            return Task.FromResult(basicQCMemberInfo);
        }
        catch (NullReferenceException aex)
        {
            var ttt = aex.Message;
            throw new Exception(ttt);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            //  return null;
        }
    }


    public Task<BasicPIDECLeadsInfo> GetPIDECLeadInfo(string LeadUserName, int LeadDomainId)
    {
        try
        {

            Contact? pidecLeadContactData = new Contact();
            pidecLeadContactData = _dbcontext.Contacts.Where(predicate: v => v.UserName == LeadUserName && v.DomainId == LeadDomainId).FirstOrDefault();

            var connection = _dbcontext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();


            var adoTransaction =
               _dbcontext.Database.CurrentTransaction?.GetDbTransaction();

            List<Project> projects;
            List<Order> orders;
            List<InspectionType> inspectionTypes;
            List<InspectionSubject> inspectionSubjects;
            List<OrderInspectionSubject> orderInspectionSubjects;
            List<OrderContact> orderContacts;
            List<Circulation> participatedCirculations;
            List<DocumentType> documentTypes;
            List<DocumentTypeVisibility> documentTypeVisibilities;

            using (var multi = connection.QueryMultiple(
            "GetBesicInfo_PIDECLeadContact",
            new { userName = LeadUserName },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction
            ))
            {

                projects = multi.Read().Select(p => new Project()
                {
                    ProjectId = new ProjectId(new Guid(p.ProjectId.ToString())),
                    FSId = p.FSId,
                    Number = p.Number,
                    Title = p.Title,
                    Description = p.Description,
                    Id = new ProjectId(new Guid(p.ProjectId.ToString())),
                    ModifiedDate = p.ModifiedDate,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedDate,
                    ModifiedBy = p.ModifiedBy,

                }).ToList();


                // projects = multi.Read<Project>().ToList();

                orders = multi.Read().Select(o => new Order()
                {
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    Name = o.Name,
                    ModifiedDate = o.ModifiedDate,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedDate,
                    EdmsId = o.EdmsId,
                    EquipmentTagNo = o.EquipmentTagNo,
                    Id = o.Id,
                    ITP_NO = o.ITP_NO,
                    ModifiedBy = o.ModifiedBy,
                    No = o.No,
                    Version = o.Version,
                    Subjects = o.Subjects,
                    Project = o.Project,
                    Title = o.Title,

                }).ToList();




                // Second result set
                inspectionTypes = multi.Read().Select(it => new InspectionType()
                {
                    InspectionTypeId = new InspectionTypeId(new Guid(it.InspectionTypeId.ToString())),
                    Name = it.Name
                }).ToList();


                // Second result set
                inspectionSubjects = multi.Read().Select(isu => new InspectionSubject()
                {
                    InspectionSubjectId = new InspectionSubjectId(new Guid(isu.InspectionSubjectId.ToString())),
                    Name = isu.Name
                }).ToList();

                // Third result set
                orderInspectionSubjects = multi.Read().Select(o => new OrderInspectionSubject()
                {
                    InspectionSubjectId = new InspectionSubjectId(new Guid(o.InspectionSubjectId.ToString())),
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    //ModifiedDate = o.ModifiedDate,
                    //CreatedDate = o.CreatedDate,
                    //CreatedBy = o.CreatedBy,
                    //ModifiedBy = o.ModifiedBy,
                    Id = o.Id
                }).ToList();


                // Fourth result set
                orderContacts = multi.Read().Select(o => new OrderContact()
                {
                    ContactId = new ContactId(new Guid(o.ContactId.ToString())),
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    Id = o.Id
                }).ToList();


                // Fifth result set
                participatedCirculations = multi.Read().Select(cr => new Circulation()
                {

                    ActionType = GetActionType_ById(cr.ActionType),
                    CreatedBy = cr.CreatedBy,
                    CreatedDate = cr.CreatedDate,
                    ModifiedDate = cr.ModifiedDate,
                    ModifiedBy = cr.ModifiedBy
                }).ToList();

                //---------------DocumentTypes-----------------------------------------
                documentTypes = multi.Read().Select(dt => new DocumentType()
                {
                    DocumentTypeId = dt.DocumentTypeId,
                    AllowedExtensions = dt.AllowedExtensions,
                    DocumentTypeCode = dt.DocumentTypeCode,
                    DocumentTypeName = dt.DocumentTypeName,                    
                    MaxSizeMB = dt.MaxSizeMB,
                    MultipleAllowed = dt.MultipleAllowed,
                    RequiresApproval = dt.RequiresApproval,
                    RequiresName = dt.RequiresName,
                    RequiresNumber = dt.RequiresNumber,
                    RequiresReview = dt.RequiresReview

                }).ToList();

                //---------------DocumentTypeVisibility-----------------------------------------
                documentTypeVisibilities = multi.Read().Select(dtv => new DocumentTypeVisibility()
                {
                    ContactTypeCode = dtv.ContactTypeCode,
                    DocumentTypeId = dtv.DocumentTypeId

                }).ToList();

            }

            //  connection.Close();


            BasicPIDECLeadsInfo? basicPIDECLeadsInfo = new BasicPIDECLeadsInfo();

            //basicQCMemberInfo.SetOrders(qcMemberData?.Orders.ToList<OrderQCContactPerson>());
            //basicQCMemberInfo.MergeWithASet(qcMemberData);
            //string[] projectIds = qcMemberData?.Orders?.Select(o => o.Order.Project.Id.ToString()).ToArray<string>();
            //basicQCMemberInfo.SetEdmsOrders(eDMSService.GetProjectOrders(projectIds).Result);

            basicPIDECLeadsInfo.Orders = orders;
            basicPIDECLeadsInfo.Projects = projects;
            basicPIDECLeadsInfo.InspectionTypes = inspectionTypes;
            basicPIDECLeadsInfo.OrderInspectionSubjects = orderInspectionSubjects;
            basicPIDECLeadsInfo.OrderContacts = orderContacts;
            basicPIDECLeadsInfo.Participated_Circulations = participatedCirculations;

            return Task.FromResult(basicPIDECLeadsInfo);
        }
        catch (NullReferenceException aex)
        {
            var ttt = aex.Message;
            throw new Exception(ttt);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            //  return null;
        }
    }

    

    //public async Task<bool> ActivateProjectInspectionSystem(Project project, List<Contact> mCContacts, List<Contact> ownerContacts, List<Contact> pidecInspectorsContacts, List<Contact> tPInspectorsContacts)
    //{
    //    try
    //    {
    //        SystematicUser projectuser = null;

    //        foreach (Contact prjContact in mCContacts)
    //        {
    //            if (prjContact != null)
    //            {

    //                var existedMCUser = userManager.Users.Where(usr => usr.Email == prjContact.Email && usr.UserName == prjContact.UserName).ToList();//&& usr.UserName == vendorContact.UserName).ToList();
    //                if (existedMCUser.Count == 0)
    //                {
    //                    projectuser = new SystematicUser
    //                    {
    //                        Id = Guid.NewGuid().ToString(),
    //                        AccessFailedCount = 0,
    //                        activationEndDate = DateTime.Now.AddYears(2),
    //                        activationStartDate = DateTime.Now,
    //                        ConcurrencyStamp = Guid.NewGuid().ToString(),
    //                        DomainId = 1,
    //                        EmailConfirmed = true,
    //                        IsActive = true,
    //                        IsAuthenticated = true,
    //                        LockoutEnabled = true,
    //                        LockoutEnd = DateTime.Now.AddSeconds(600),
    //                        NormalizedEmail = prjContact.Email,
    //                        NormalizedUserName = prjContact.UserName,
    //                        PhoneNumber = prjContact.Phone,
    //                        PhoneNumberConfirmed = true,
    //                        Token = "1",
    //                        TwoFactorEnabled = true,
    //                        UserType = 2,
    //                        Use_Id = 3,
    //                        UserName = "MC_11",//prjContact.Name,
    //                        Email = prjContact.Email
    //                    };



    //                    var result = await userManager.CreateAsync(projectuser);
    //                    if (result.Succeeded)
    //                    {
    //                        await userManager.AddToRoleAsync(projectuser, GetContactTypeName_Code(prjContact.ContactType.GetHashCode()));
    //                    }
    //                    else
    //                    {
    //                        // Optional: handle errors (e.g., logging them)
    //                        foreach (var error in result.Errors)
    //                        {
    //                            // Log each error (for example purposes, writing to console)
    //                            Console.WriteLine($"Error: {error.Description}");
    //                        }
    //                    }
    //                }
    //            }

    //            Contact newMCContact;
    //            for (int i = 0; i < ownerContacts.Count; i++)
    //            {
    //                var returnedContact = dbContext.Contacts.Where(contact => contact.ContactId == ownerContacts[i].Id).ToList();
    //                if (returnedContact.Count == 0)
    //                {
    //                    newMCContact = new Contact();
    //                    newMCContact.Name = ownerContacts[i].Name;
    //                    newMCContact.Email = ownerContacts[i].Email;
    //                    newMCContact.Phone = ownerContacts[i].Phone == null ? "" : ownerContacts[i].Phone;
    //                    newMCContact.Cc = ownerContacts[i].Cc;
    //                    // newMCContactct.Address = orderContacts[i].Address;
    //                    newMCContact.Attention = ownerContacts[i].Attention;
    //                    newMCContact.ContactType = ownerContacts[i].ContactType;
    //                    newMCContact.CreatedBy = "";
    //                    newMCContact.CreatedDate = DateTime.Now;
    //                    newMCContact.ModifiedBy = "";
    //                    newMCContact.ModifiedDate = DateTime.Now;
    //                    newMCContact.HomePage = ownerContacts[i].HomePage;
    //                    newMCContact.Fax = ownerContacts[i].Fax;
    //                    newMCContact.DomainId = ownerContacts[i].DomainId;
    //                    newMCContact.Mobile = ownerContacts[i].Mobile;
    //                    newMCContact.IsVisible = ownerContacts[i].IsVisible;
    //                    newMCContact.UserName = ownerContacts[i].UserName != "" ? ownerContacts[i].UserName : ownerContacts[i].Name;

    //                    newMCContact.ContactId = ContactId.CreateUnique();
    //                    dbContext.Contacts.Add(newMCContact);
    //                    dbContext.SaveChanges();
    //                }
    //                else
    //                    newMCContact = returnedContact[0];

    //                ProjectContact newMCProjectContact = new ProjectContact();
    //                newMCProjectContact.Contact = newMCContact;
    //                newMCProjectContact.ContactId = newMCContact.ContactId;
    //                newMCProjectContact.ProjectId = project.ProjectId;
    //                //newOrderContact.Order = newOrderInInspectionSystem;
    //                dbContext.ProjectContacts.Add(newMCProjectContact);
    //            }


    //            List<ProjectContact> existedProjectMCContacts;



    //            existedProjectMCContacts = project.MCContacts!.ToList(); // Use ToList() to avoid modifying the collection while iterating.

    //            //dbContext.OrderContacts.RemoveRange(ExistedOrder.Contacts);
    //            //dbContext.OrderInspectionSubjects.RemoveRange(ExistedOrder.Subjects);
    //            List<Contact> deletedMCContacts;
    //            Guid projectMCContactId;
    //            foreach (var projectMCContact in existedProjectMCContacts)
    //            {
    //                //orderContactId =orderContact.ContactId!;
    //                //deletedContacts = dbContext.Contacts.AsEnumerable().Where((contact) => contact.ContactId == orderContactId).ToList();
    //                //dbContext.Contacts.RemoveRange(deletedContacts);

    //                dbContext.ProjectContacts.Remove(projectMCContact);
    //            }

    //        }

    //        Contact newContact;
    //        for (int i = 0; i < ownerContacts.Count; i++)
    //        {
    //            var returnedContact = dbContext.Contacts.Where(contact => contact.ContactId == ownerContacts[i].Id).ToList();
    //            if (returnedContact.Count == 0)
    //            {
    //                newContact = new Contact();
    //                newContact.Name = ownerContacts[i].Name;
    //                newContact.Email = ownerContacts[i].Email;
    //                newContact.Phone = ownerContacts[i].Phone == null ? "" : ownerContacts[i].Phone;
    //                newContact.Cc = ownerContacts[i].Cc;
    //                //newContact.Address = orderContacts[i].Address;
    //                newContact.Attention = ownerContacts[i].Attention;
    //                newContact.ContactType = ownerContacts[i].ContactType;
    //                newContact.CreatedBy = "";
    //                newContact.CreatedDate = DateTime.Now;
    //                newContact.ModifiedBy = "";
    //                newContact.ModifiedDate = DateTime.Now;
    //                newContact.HomePage = ownerContacts[i].HomePage;
    //                newContact.Fax = ownerContacts[i].Fax;
    //                newContact.DomainId = ownerContacts[i].DomainId;
    //                newContact.Mobile = ownerContacts[i].Mobile;
    //                newContact.IsVisible = ownerContacts[i].IsVisible;
    //                newContact.UserName = ownerContacts[i].UserName != "" ? ownerContacts[i].UserName : ownerContacts[i].Name;

    //                newContact.ContactId = ContactId.CreateUnique();
    //                dbContext.Contacts.Add(newContact);
    //                dbContext.SaveChanges();
    //            }
    //            else
    //                newContact = returnedContact[0];

    //            ProjectContact newProjectContact = new ProjectContact();
    //            newProjectContact.Contact = newContact;
    //            newProjectContact.ContactId = newContact.ContactId;
    //            newProjectContact.ProjectId = project.ProjectId;
    //            //newOrderContact.Order = newOrderInInspectionSystem;
    //            dbContext.ProjectContacts.Add(newProjectContact);
    //        }
    //    }
    //    catch (Exception exp) { }

    //    return await Task.FromResult(true);
    //}




    //public async Task<bool> ActivateProjectInspectionSystem(Project project, List<Contact> mCContacts, List<Contact> ownerContacts, List<Contact> pidecInspectorsContacts, List<Contact> tPInspectorsContacts)
    //{
    //    try
    //    {


    //        using (var scope = _serviceProvider.CreateScope())
    //        {
    //            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SystematicUser>>();
    //            var dbContext = scope.ServiceProvider.GetRequiredService<InspectionDbContext>();

    //            using (var transaction = dbContext.Database.BeginTransaction())
    //            {
    //                try
    //                {

    //                    SystematicUser projectuser = null;

    //                    foreach (Contact prjContact in mCContacts)
    //                    {
    //                        if (prjContact != null)
    //                        {

    //                            var existedMCUser = userManager.Users.Where(usr => usr.Email == prjContact.Email && usr.UserName == prjContact.UserName).ToList();//&& usr.UserName == vendorContact.UserName).ToList();
    //                            if (existedMCUser.Count == 0)
    //                            {
    //                                projectuser = new SystematicUser
    //                                {
    //                                    Id = Guid.NewGuid().ToString(),
    //                                    AccessFailedCount = 0,
    //                                    activationEndDate = DateTime.Now.AddYears(2),
    //                                    activationStartDate = DateTime.Now,
    //                                    ConcurrencyStamp = Guid.NewGuid().ToString(),
    //                                    DomainId = 1,
    //                                    EmailConfirmed = true,
    //                                    IsActive = true,
    //                                    IsAuthenticated = true,
    //                                    LockoutEnabled = true,
    //                                    LockoutEnd = DateTime.Now.AddSeconds(600),
    //                                    NormalizedEmail = prjContact.Email,
    //                                    NormalizedUserName = prjContact.UserName,
    //                                    PhoneNumber = prjContact.Phone,
    //                                    PhoneNumberConfirmed = true,
    //                                    Token = "1",
    //                                    TwoFactorEnabled = true,
    //                                    UserType = 2,
    //                                    Use_Id = 3,
    //                                    UserName = "MC_11",//prjContact.Name,
    //                                    Email = prjContact.Email
    //                                };



    //                                var result = await userManager.CreateAsync(projectuser);
                                    
    //                                if (result.Succeeded)
    //                                {
    //                                    await userManager.AddToRoleAsync(projectuser, userManager.GetContactTypeName_Code(prjContact.ContactType.GetHashCode()));
    //                                }
    //                                else
    //                                {
    //                                    // Optional: handle errors (e.g., logging them)
    //                                    foreach (var error in result.Errors)
    //                                    {
    //                                        // Log each error (for example purposes, writing to console)
    //                                        Console.WriteLine($"Error: {error.Description}");
    //                                    }
    //                                }
    //                            }
    //                        }

    //                        Contact newMCContact;
    //                        for (int i = 0; i < ownerContacts.Count; i++)
    //                        {
    //                            var returnedContact = dbContext.Contacts.Where(contact => contact.ContactId == ownerContacts[i].Id).ToList();
    //                            if (returnedContact.Count == 0)
    //                            {
    //                                newMCContact = new Contact();
    //                                newMCContact.Name = ownerContacts[i].Name;
    //                                newMCContact.Email = ownerContacts[i].Email;
    //                                newMCContact.Phone = ownerContacts[i].Phone == null ? "" : ownerContacts[i].Phone;
    //                                newMCContact.Cc = ownerContacts[i].Cc;
    //                                // newMCContactct.Address = orderContacts[i].Address;
    //                                newMCContact.Attention = ownerContacts[i].Attention;
    //                                newMCContact.ContactType = ownerContacts[i].ContactType;
    //                                newMCContact.CreatedBy = "";
    //                                newMCContact.CreatedDate = DateTime.Now;
    //                                newMCContact.ModifiedBy = "";
    //                                newMCContact.ModifiedDate = DateTime.Now;
    //                                newMCContact.HomePage = ownerContacts[i].HomePage;
    //                                newMCContact.Fax = ownerContacts[i].Fax;
    //                                newMCContact.DomainId = ownerContacts[i].DomainId;
    //                                newMCContact.Mobile = ownerContacts[i].Mobile;
    //                                newMCContact.IsVisible = ownerContacts[i].IsVisible;
    //                                newMCContact.UserName = ownerContacts[i].UserName != "" ? ownerContacts[i].UserName : ownerContacts[i].Name;

    //                                newMCContact.ContactId = ContactId.CreateUnique();
    //                                dbContext.Contacts.Add(newMCContact);
    //                                dbContext.SaveChanges();
    //                            }
    //                            else
    //                                newMCContact = returnedContact[0];

    //                            ProjectContact newMCProjectContact = new ProjectContact();
    //                            newMCProjectContact.Contact = newMCContact;
    //                            newMCProjectContact.ContactId = newMCContact.ContactId;
    //                            newMCProjectContact.ProjectId = project.ProjectId;
    //                            //newOrderContact.Order = newOrderInInspectionSystem;
    //                            dbContext.ProjectContacts.Add(newMCProjectContact);
    //                        }


    //                        List<ProjectContact> existedProjectMCContacts;



    //                        existedProjectMCContacts = project.MCContacts!.ToList(); // Use ToList() to avoid modifying the collection while iterating.

    //                        //dbContext.OrderContacts.RemoveRange(ExistedOrder.Contacts);
    //                        //dbContext.OrderInspectionSubjects.RemoveRange(ExistedOrder.Subjects);
    //                        List<Contact> deletedMCContacts;
    //                        Guid projectMCContactId;
    //                        foreach (var projectMCContact in existedProjectMCContacts)
    //                        {
    //                            //orderContactId =orderContact.ContactId!;
    //                            //deletedContacts = dbContext.Contacts.AsEnumerable().Where((contact) => contact.ContactId == orderContactId).ToList();
    //                            //dbContext.Contacts.RemoveRange(deletedContacts);

    //                            dbContext.ProjectContacts.Remove(projectMCContact);
    //                        }

    //                    }

    //                    Contact newContact;
    //                    for (int i = 0; i < ownerContacts.Count; i++)
    //                    {
    //                        var returnedContact = dbContext.Contacts.Where(contact => contact.ContactId == ownerContacts[i].Id).ToList();
    //                        if (returnedContact.Count == 0)
    //                        {
    //                            newContact = new Contact();
    //                            newContact.Name = ownerContacts[i].Name;
    //                            newContact.Email = ownerContacts[i].Email;
    //                            newContact.Phone = ownerContacts[i].Phone == null ? "" : ownerContacts[i].Phone;
    //                            newContact.Cc = ownerContacts[i].Cc;
    //                            //newContact.Address = orderContacts[i].Address;
    //                            newContact.Attention = ownerContacts[i].Attention;
    //                            newContact.ContactType = ownerContacts[i].ContactType;
    //                            newContact.CreatedBy = "";
    //                            newContact.CreatedDate = DateTime.Now;
    //                            newContact.ModifiedBy = "";
    //                            newContact.ModifiedDate = DateTime.Now;
    //                            newContact.HomePage = ownerContacts[i].HomePage;
    //                            newContact.Fax = ownerContacts[i].Fax;
    //                            newContact.DomainId = ownerContacts[i].DomainId;
    //                            newContact.Mobile = ownerContacts[i].Mobile;
    //                            newContact.IsVisible = ownerContacts[i].IsVisible;
    //                            newContact.UserName = ownerContacts[i].UserName != "" ? ownerContacts[i].UserName : ownerContacts[i].Name;

    //                            newContact.ContactId = ContactId.CreateUnique();
    //                            dbContext.Contacts.Add(newContact);
    //                            dbContext.SaveChanges();
    //                        }
    //                        else
    //                            newContact = returnedContact[0];

    //                        ProjectContact newProjectContact = new ProjectContact();
    //                        newProjectContact.Contact = newContact;
    //                        newProjectContact.ContactId = newContact.ContactId;
    //                        newProjectContact.ProjectId = project.ProjectId;
    //                        //newOrderContact.Order = newOrderInInspectionSystem;
    //                        dbContext.ProjectContacts.Add(newProjectContact);
    //                    }

    //                    dbContext.SaveChanges();
    //                    transaction.Commit();
    //                }
    //                catch (Exception ex)
    //                {
    //                    transaction.Rollback();
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception exp) { }

    //    return await Task.FromResult(true);
    //}




    public async Task<bool> ActivateEdmsOrderInInspectionSystem(Order order, Contact[] orderContacts, InspectionSubject[] subjects, Contact[] exceptionContacts)
    {
        try
        {

            var vendorContact = orderContacts
            .Where(ord => ord.ContactType == ContactType.Vendor.GetHashCode())
            .FirstOrDefault();

            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SystematicUser>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<InspectionDbContext>();

                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {

                        SystematicUser vendoruser = null;

                        if (vendorContact != null)
                        {

                            var existedUser = userManager.Users.Where(usr => usr.Email == vendorContact.Email).ToList();//&& usr.UserName == vendorContact.UserName).ToList();
                            if (existedUser.Count == 0)
                            {
                                vendoruser = new SystematicUser
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    AccessFailedCount = 0,
                                    activationEndDate = DateTime.Now.AddYears(2),
                                    activationStartDate = DateTime.Now,
                                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                                    DomainId = 1,
                                    EmailConfirmed = true,
                                    IsActive = true,
                                    IsAuthenticated = true,
                                    LockoutEnabled = true,
                                    LockoutEnd = DateTime.Now.AddSeconds(600),
                                    NormalizedEmail = vendorContact.Email,
                                    NormalizedUserName = vendorContact.UserName,
                                    PhoneNumber = vendorContact.Phone,
                                    PhoneNumberConfirmed = true,
                                    Token = "1",
                                    TwoFactorEnabled = true,
                                    UserType = 2,
                                    Use_Id = 3,
                                    UserName = vendorContact.Name,
                                    Email = vendorContact.Email,

                                };



                                var result = await userManager.CreateAsync(vendoruser);
                                if (result.Succeeded)
                                {
                                    await userManager.AddToRoleAsync(vendoruser, "vendor");
                                }
                                else
                                {
                                    // Optional: handle errors (e.g., logging them)
                                    foreach (var error in result.Errors)
                                    {
                                        // Log each error (for example purposes, writing to console)
                                        Console.WriteLine($"Error: {error.Description}");
                                    }
                                }
                            }
                        }

                        Order newOrderInInspectionSystem;
                        List<OrderContact> existedOrderContacts;
                        List<OrderInspectionSubject> existedOrderInspections;

                        Order ExistedOrder = dbContext.Orders//.AsNoTracking()
                        .Include(o => o.Contacts)
                        .Include(o => o.Subjects)
                        .FirstOrDefault(o => o.EdmsId == order.EdmsId);
                        if (ExistedOrder == null)
                        {
                            newOrderInInspectionSystem = Order.Create(order.Name, order.Title, order.No, order.Project, order.EdmsId);
                            newOrderInInspectionSystem.OrderId = OrderId.CreateUnique();
                            newOrderInInspectionSystem.Project = order.Project;
                            dbContext.Entry(newOrderInInspectionSystem.Project).State = EntityState.Unchanged;
                            newOrderInInspectionSystem.ITP_NO = order.ITP_NO;
                            newOrderInInspectionSystem.EdmsId = order.EdmsId;
                            newOrderInInspectionSystem.Version = 0;
                            newOrderInInspectionSystem.Title = order.Title;
                            newOrderInInspectionSystem.No = order.No;
                            newOrderInInspectionSystem.Name = order.Name;

                            newOrderInInspectionSystem.EquipmentTagNo = order.EquipmentTagNo;
                            newOrderInInspectionSystem.CreatedDate = DateTime.Now;
                            newOrderInInspectionSystem.CreatedBy = order.CreatedBy;
                            newOrderInInspectionSystem.ModifiedBy = order.ModifiedBy;
                            newOrderInInspectionSystem.ModifiedDate = DateTime.Now;

                            dbContext.Orders.Add(newOrderInInspectionSystem);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            existedOrderContacts = ExistedOrder.Contacts!.ToList(); // Use ToList() to avoid modifying the collection while iterating.

                            //dbContext.OrderContacts.RemoveRange(ExistedOrder.Contacts);
                            //dbContext.OrderInspectionSubjects.RemoveRange(ExistedOrder.Subjects);
                            List<Contact> deletedContacts;
                            Guid orderContactId;
                            foreach (var orderContact in existedOrderContacts)
                            {
                                //orderContactId =orderContact.ContactId!;
                                //deletedContacts = dbContext.Contacts.AsEnumerable().Where((contact) => contact.ContactId == orderContactId).ToList();
                                //dbContext.Contacts.RemoveRange(deletedContacts);
                                
                                dbContext.OrderContacts.Remove(orderContact);
                            }

                            existedOrderInspections = ExistedOrder.Subjects!.ToList(); // Use ToList() to avoid modifying the collection while iterating.
                            List<InspectionSubject> deletedInspectionSubjects;

                            Guid orderInspectionSubjectId;
                            foreach (var orderSubject in existedOrderInspections)
                            {
                                //orderInspectionSubjectId = orderSubject.InspectionSubjectId!;
                                //deletedInspectionSubjects = dbContext.InspectionSubjects.Where((subject) => subject.InspectionSubjectId == orderInspectionSubjectId).ToList();
                                //dbContext.InspectionSubjects.RemoveRange(deletedInspectionSubjects);
                                
                                dbContext.OrderInspectionSubjects.Remove(orderSubject);
                            }

                            newOrderInInspectionSystem = ExistedOrder;
                            newOrderInInspectionSystem.OrderId = ExistedOrder.OrderId;
                            newOrderInInspectionSystem.Project = order.Project;
                            dbContext.Entry(newOrderInInspectionSystem.Project).State = EntityState.Unchanged;
                            newOrderInInspectionSystem.ITP_NO = order.ITP_NO;
                            newOrderInInspectionSystem.Version = 0;
                            newOrderInInspectionSystem.Title = order.Title;
                            newOrderInInspectionSystem.No = order.No;
                            newOrderInInspectionSystem.Name = order.Name;
                            newOrderInInspectionSystem.EquipmentTagNo = order.EquipmentTagNo;

                            newOrderInInspectionSystem.ModifiedBy = order.ModifiedBy;
                            newOrderInInspectionSystem.ModifiedDate = DateTime.Now;
                            dbContext.SaveChanges();
                        }


                        InspectionSubject newInspectionSubject ;
                        for (int i = 0; i < subjects.Length; i++)
                        {
                            newInspectionSubject = null;
                            var returnedInspectionSubject = dbContext.InspectionSubjects.Where(ins => ins.Name == subjects[i].Name).ToList();
                            if (returnedInspectionSubject.Count==0)
                            {
                                newInspectionSubject = new InspectionSubject();
                                newInspectionSubject.Name = subjects[i].Name;
                                newInspectionSubject.InspectionSubjectId = InspectionSubjectId.CreateUnique();
                                dbContext.InspectionSubjects.Add(newInspectionSubject);
                                dbContext.SaveChanges();
                            }
                            else
                                newInspectionSubject = returnedInspectionSubject[0];

                            OrderInspectionSubject newOrderInspectionSubject = new OrderInspectionSubject();
                            newOrderInspectionSubject.InspectionSubject = newInspectionSubject;
                            newOrderInspectionSubject.InspectionSubjectId = newInspectionSubject.InspectionSubjectId;
                            // newOrderInspectionSubject.Order = newOrderInInspectionSystem;
                            newOrderInspectionSubject.OrderId = newOrderInInspectionSystem.OrderId;
                            dbContext.OrderInspectionSubjects.Add(newOrderInspectionSubject);
                        }

                        Contact newContact;
                        for (int i = 0; i < orderContacts.Length; i++)
                        {
                            var returnedContact = dbContext.Contacts.Where(contact => contact.ContactId == orderContacts[i].ContactId).ToList();
                            if (returnedContact.Count==0) 
                                {
                                    newContact = new Contact();
                                    newContact.Name = orderContacts[i].Name;
                                    newContact.Email = orderContacts[i].Email;
                                    newContact.Phone = orderContacts[i].Phone == null? "" : orderContacts[i].Phone;
                                    newContact.Cc = orderContacts[i].Cc;
                                    //newContact.Address = orderContacts[i].Address;
                                    newContact.Note = orderContacts[i].Note;
                                    newContact.ContactType = orderContacts[i].ContactType;
                                    newContact.CreatedBy = newOrderInInspectionSystem.CreatedBy;
                                    newContact.CreatedDate = newOrderInInspectionSystem.CreatedDate;
                                    newContact.ModifiedBy = newOrderInInspectionSystem.ModifiedBy;
                                    newContact.ModifiedDate = newOrderInInspectionSystem.ModifiedDate;
                                    newContact.HomePage = orderContacts[i].HomePage;
                                    newContact.Fax = orderContacts[i].Fax;
                                    newContact.DomainId = orderContacts[i].DomainId;
                                    newContact.Mobile = orderContacts[i].Mobile;
                                    newContact.IsVisible = orderContacts[i].IsVisible;
                                    newContact.UserName = orderContacts[i].UserName != "" ? orderContacts[i].UserName : orderContacts[i].Name;

                                    newContact.ContactId = ContactId.CreateUnique();
                                    dbContext.Contacts.Add(newContact);
                                    dbContext.SaveChanges();
                                }
                                else
                                    newContact = returnedContact[0];

                            OrderContact newOrderContact = new OrderContact();
                            newOrderContact.Contact = newContact;
                            newOrderContact.ContactId = newContact.ContactId;
                            newOrderContact.OrderId = newOrderInInspectionSystem.OrderId;
                            //newOrderContact.Order = newOrderInInspectionSystem;
                            dbContext.OrderContacts.Add(newOrderContact);
                        }

                        //Find ProjectContacts that are expected for this order
                        //at first should delete all order Exception settings from database
                        dbContext.OrdersettingExceptions.Where(ords => ords.OrderId == order.Id).ExecuteDelete();

                        //Then should add updated exceptions to orderExceptionSettings Table

                        ProjectContact prjContact = new ProjectContact();
                        Guid exceptionContactId = Guid.Empty;
                        for (int i = 0; i < exceptionContacts.Length; i++)
                        {
                            exceptionContactId = exceptionContacts[i].ContactId;
                            prjContact = dbContext.ProjectContacts.Where(pc => pc.ContactId == exceptionContactId && pc.ProjectId == order.Project.Id).FirstOrDefault();
                            if (prjContact != null)
                            {
                                OrdersettingException ordersettingException = new OrdersettingException();
                                ordersettingException.OrderId = newOrderInInspectionSystem.OrderId;
                                ordersettingException.ProjectContactId = prjContact.Id;
                                dbContext.OrdersettingExceptions.Add(ordersettingException);
                            }
                        }

                        dbContext.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
        catch (Exception exp) { }

        return await Task.FromResult(true);
    }

    public  Task<bool> RemoveOrderContacts(List<OrderContact> orderContacts)
    {
        foreach (var orderContact in orderContacts)
        {
            //orderContactId =orderContact.ContactId!;
            //deletedContacts = dbContext.Contacts.AsEnumerable().Where((contact) => contact.ContactId == orderContactId).ToList();
            //dbContext.Contacts.RemoveRange(deletedContacts);

            _dbcontext.OrderContacts.Remove(orderContact);
        }
        try
        {
            _dbcontext.SaveChanges();
            return Task.FromResult(true);
        }
        catch (Exception exp) { 
            return Task.FromResult(false);
        }
    }

    public Task<bool> RemoveOrderInspectionSubjects(List<OrderInspectionSubject> orderInspectionSubjects)
    {
        foreach (var orderInspectionSubject in orderInspectionSubjects)
        {
            //orderContactId =orderContact.ContactId!;
            //deletedContacts = dbContext.Contacts.AsEnumerable().Where((contact) => contact.ContactId == orderContactId).ToList();
            //dbContext.Contacts.RemoveRange(deletedContacts);

            _dbcontext.OrderInspectionSubjects.Remove(orderInspectionSubject);
        }
        try
        {
            _dbcontext.SaveChanges();

            return Task.FromResult(true);
        }
        catch (Exception exp) { 
            return Task.FromResult(false);
        }
    }

    public Task<bool> SaveOrderInspectionSubjects(List<InspectionSubject> InspectionSubjects, Order order) {

        var trackedOrder = _dbcontext.Orders.Find(order.OrderId);

        InspectionSubject newInspectionSubject;
        for (int i = 0; i < InspectionSubjects.Count; i++)
        {
            newInspectionSubject = null;
            var returnedInspectionSubject = _dbcontext.InspectionSubjects.Where(ins => ins.Name == InspectionSubjects[i].Name).ToList();
            if (returnedInspectionSubject.Count == 0)
            {
                newInspectionSubject = new InspectionSubject();
                newInspectionSubject.Name = InspectionSubjects[i].Name;
                newInspectionSubject.InspectionSubjectId = InspectionSubjectId.CreateUnique();
                _dbcontext.InspectionSubjects.Add(newInspectionSubject);
                _dbcontext.SaveChanges();
            }
            else
                newInspectionSubject = returnedInspectionSubject[0];

            OrderInspectionSubject newOrderInspectionSubject = new OrderInspectionSubject();
            newOrderInspectionSubject.InspectionSubject = newInspectionSubject;
            newOrderInspectionSubject.InspectionSubjectId = newInspectionSubject.InspectionSubjectId;
            // newOrderInspectionSubject.Order = newOrderInInspectionSystem;
            newOrderInspectionSubject.OrderId = order.OrderId;
            _dbcontext.Entry(order).State = EntityState.Unchanged;
            _dbcontext.OrderInspectionSubjects.Add(newOrderInspectionSubject);
        }
        try
        {
            _dbcontext.SaveChanges();
            return Task.FromResult(true);
        }
        catch (Exception exp)
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> SaveOrderContacts(List<Contact> contacts, Order order)
    {
        Contact newContact;
        for (int i = 0; i < contacts.Count; i++)
        {
            var returnedContact = _dbcontext.Contacts.Where(contact => (contact.ContactId == contacts[i].ContactId || contact.UserName == contacts[i].UserName)).ToList();
            if (returnedContact.Count == 0)
            {
                newContact = new Contact();
                newContact.Name = contacts[i].Name;
                newContact.Email = contacts[i].Email;
                newContact.Phone = contacts[i].Phone == null ? "" : contacts[i].Phone;
                newContact.Cc = contacts[i].Cc;
                //newContact.Address = orderContacts[i].Address;
                newContact.Note = contacts[i].Note;
                newContact.ContactType = contacts[i].ContactType;
                newContact.CreatedBy = order.CreatedBy;
                newContact.CreatedDate = order.CreatedDate;
                newContact.ModifiedBy = order.ModifiedBy;
                newContact.ModifiedDate = order.ModifiedDate;
                newContact.HomePage = contacts[i].HomePage;
                newContact.Fax = contacts[i].Fax;
                newContact.DomainId = contacts[i].DomainId;
                newContact.Mobile = contacts[i].Mobile;
                newContact.IsVisible = contacts[i].IsVisible;
                newContact.UserName = contacts[i].UserName != "" ? contacts[i].UserName : contacts[i].Name;

                newContact.ContactId = ContactId.CreateUnique();
                _dbcontext.Contacts.Add(newContact);
                _dbcontext.SaveChanges();
            }
            else
                newContact = returnedContact[0];

            OrderContact newOrderContact = new OrderContact();
            newOrderContact.Contact = newContact;
            newOrderContact.ContactId = newContact.ContactId;
            newOrderContact.OrderId = order.OrderId;
            //newOrderContact.Order = newOrderInInspectionSystem;
            _dbcontext.Entry(order).State = EntityState.Unchanged;
            _dbcontext.OrderContacts.Add(newOrderContact);
        }
        try
        {
            _dbcontext.SaveChanges();
            return Task.FromResult(true);
        }
        catch (Exception exp)
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> SaveOrderExceptions(List<Contact> exceptionContacts, Order order)
    {

        //Find ProjectContacts that are expected for this order
        //at first should delete all order Exception settings from database
        _dbcontext.OrdersettingExceptions.Where(ords => ords.OrderId == order.Id).ExecuteDelete();

        //Then should add updated exceptions to orderExceptionSettings Table

        ProjectContact prjContact = new ProjectContact();
        Guid exceptionContactId = Guid.Empty;
        for (int i = 0; i < exceptionContacts.Count; i++)
        {
            exceptionContactId = exceptionContacts[i].ContactId;
            prjContact = _dbcontext.ProjectContacts.Where(pc => pc.ContactId == exceptionContactId && pc.ProjectId == order.Project.Id).FirstOrDefault();
            if (prjContact != null)
            {
                OrdersettingException ordersettingException = new OrdersettingException();
                ordersettingException.OrderId = order.OrderId;
                _dbcontext.Entry(order).State = EntityState.Unchanged;
                ordersettingException.ProjectContactId = prjContact.Id;
                _dbcontext.OrdersettingExceptions.Add(ordersettingException);
            }
        }
        try
        {
            _dbcontext.SaveChanges();
            return Task.FromResult(true);
        }
        catch (Exception exp)
        {
            return Task.FromResult(false);
        }
    }


    //public async Task<bool> ActivateEdmsOrderInInspectionSystem(Order order, Contact[] orderContacts, InspectionSubject[] subjects)
    //{
    //    try
    //    {

    //        var vendorContact = orderContacts
    //        .Where(ord => ord.ContactType == ContactType.Vendor.GetHashCode())
    //        .FirstOrDefault();

    //        using (var scope1 = _serviceProvider.CreateScope())
    //        {
    //            // Resolve the UserManager from the new scope.
    //            var userManager = scope1.ServiceProvider.GetRequiredService<UserManager<SystematicUser>>();
    //            var dbContext1_Scope1 = scope1.ServiceProvider.GetRequiredService<InspectionDbContext>();
    //            SystematicUser vendoruser = null;
    //            var connection = dbContext1_Scope1.Database.GetDbConnection();
    //            if (connection.State != ConnectionState.Open)
    //                connection.Open();

    //            //if (!await _roleManager.RoleExistsAsync("vendor"))
    //            //{
    //            //    await _roleManager.CreateAsync(new IdentityRole("vendor"));
    //            //}

    //            // Create the vendor user.

    //            if (vendorContact != null)
    //            {

    //                var existedUser = userManager.Users.Where(usr => usr.Email == vendorContact.Email).ToList();//&& usr.UserName == vendorContact.UserName).ToList();
    //                if (existedUser.Count == 0)
    //                {
    //                    vendoruser = new SystematicUser
    //                    {
    //                        Id = Guid.NewGuid().ToString(),
    //                        AccessFailedCount = 0,
    //                        activationEndDate = DateTime.Now.AddYears(2),
    //                        activationStartDate = DateTime.Now,
    //                        ConcurrencyStamp = Guid.NewGuid().ToString(),
    //                        DomainId = 1,
    //                        EmailConfirmed = true,
    //                        IsActive = true,
    //                        IsAuthenticated = true,
    //                        LockoutEnabled = true,
    //                        LockoutEnd = DateTime.Now.AddSeconds(600),
    //                        NormalizedEmail = vendorContact.Email,
    //                        NormalizedUserName = vendorContact.UserName,
    //                        PhoneNumber = vendorContact.Phone,
    //                        PhoneNumberConfirmed = true,
    //                        Token = "1",
    //                        TwoFactorEnabled = true,
    //                        UserType = 2,
    //                        Use_Id = 3,
    //                        UserName = vendorContact.Name,
    //                        Email = vendorContact.Email,

    //                    };



    //                    var result = await userManager.CreateAsync(vendoruser);
    //                    if (result.Succeeded)
    //                    {
    //                        await userManager.AddToRoleAsync(vendoruser, "vendor");
    //                    }
    //                    else
    //                    {
    //                        // Optional: handle errors (e.g., logging them)
    //                        foreach (var error in result.Errors)
    //                        {
    //                            // Log each error (for example purposes, writing to console)
    //                            Console.WriteLine($"Error: {error.Description}");
    //                        }
    //                    }
    //                }
    //        }
    //    }



    //        using (var scope2 = _serviceProvider.CreateScope())
    //        {


    //            var _dbcontext2_Scope2 = scope2.ServiceProvider.GetRequiredService<InspectionDbContext>();
    //            var connection = _dbcontext2_Scope2.Database.GetDbConnection();
    //            if (connection.State != ConnectionState.Open)
    //                connection.Open();



    //            try
    //            {
    //                Order newOrderInInspectionSystem;
    //                List<OrderContact> existedOrderContacts;
    //                List<OrderInspectionSubject> existedOrderInspections;

    //                Order ExistedOrder = _dbcontext2_Scope2.Orders.AsNoTracking()
    //                .Include(o => o.Contacts)
    //                .Include(o => o.Subjects)
    //                .FirstOrDefault(o => o.EdmsId == order.EdmsId);
    //                if (ExistedOrder == null)
    //                {
    //                    newOrderInInspectionSystem = Order.Create(order.Name, order.Title, order.No, order.Project, order.EdmsId);
    //                    newOrderInInspectionSystem.OrderId = OrderId.CreateUnique();
    //                    newOrderInInspectionSystem.Project = order.Project;
    //                    newOrderInInspectionSystem.ITP_NO = order.ITP_NO;
    //                    newOrderInInspectionSystem.EdmsId = order.EdmsId;
    //                    newOrderInInspectionSystem.Version = 0;
    //                    newOrderInInspectionSystem.Title = order.Title;
    //                    newOrderInInspectionSystem.No = order.No;
    //                    newOrderInInspectionSystem.Name = order.Name;

    //                    newOrderInInspectionSystem.EquipmentTagNo = order.EquipmentTagNo;
    //                    newOrderInInspectionSystem.CreatedDate = DateTime.Now;
    //                    newOrderInInspectionSystem.CreatedBy = order.CreatedBy;
    //                    newOrderInInspectionSystem.ModifiedBy = order.ModifiedBy;
    //                    newOrderInInspectionSystem.ModifiedDate = DateTime.Now;

    //                    _dbcontext2_Scope2.Orders.Add(newOrderInInspectionSystem);
    //                    _dbcontext2_Scope2.SaveChanges();
    //                }
    //                else
    //                {  
    //                    existedOrderContacts = ExistedOrder.Contacts.ToList(); // Use ToList() to avoid modifying the collection while iterating.
    //                    foreach (var orderContact in existedOrderContacts)
    //                    {
    //                        _dbcontext2_Scope2.OrderContacts.Remove(orderContact);
    //                    }

    //                    existedOrderInspections = ExistedOrder.Subjects.ToList(); // Use ToList() to avoid modifying the collection while iterating.
    //                    foreach (var orderSubject in existedOrderInspections)
    //                    {
    //                        _dbcontext2_Scope2.OrderInspectionSubjects.Remove(orderSubject);
    //                    }

    //                    newOrderInInspectionSystem = ExistedOrder;
    //                    newOrderInInspectionSystem.OrderId = ExistedOrder.OrderId;
    //                    newOrderInInspectionSystem.Project = order.Project;
    //                    newOrderInInspectionSystem.ITP_NO = order.ITP_NO;                
    //                    newOrderInInspectionSystem.Version = 0;
    //                    newOrderInInspectionSystem.Title = order.Title;
    //                    newOrderInInspectionSystem.No = order.No;
    //                    newOrderInInspectionSystem.Name = order.Name;
    //                    newOrderInInspectionSystem.EquipmentTagNo = order.EquipmentTagNo;

    //                    newOrderInInspectionSystem.ModifiedBy = order.ModifiedBy;
    //                    newOrderInInspectionSystem.ModifiedDate = DateTime.Now;
    //                    _dbcontext2_Scope2.SaveChanges();
    //                }


    //                InspectionSubject newInspectionSubject;
    //                for (int i = 0; i < subjects.Length; i++)
    //                {
    //                    newInspectionSubject = new InspectionSubject();
    //                    newInspectionSubject.Name = subjects[i].Name;
    //                    newInspectionSubject.InspectionSubjectId = InspectionSubjectId.CreateUnique();
    //                    _dbcontext2_Scope2.InspectionSubjects.Add(newInspectionSubject);
    //                    _dbcontext2_Scope2.SaveChanges();

    //                    OrderInspectionSubject newOrderInspectionSubject = new OrderInspectionSubject();
    //                    newOrderInspectionSubject.InspectionSubject = newInspectionSubject;
    //                    newOrderInspectionSubject.InspectionSubjectId = newInspectionSubject.InspectionSubjectId;
    //                   // newOrderInspectionSubject.Order = newOrderInInspectionSystem;
    //                    newOrderInspectionSubject.OrderId = newOrderInInspectionSystem.OrderId;
    //                    _dbcontext2_Scope2.OrderInspectionSubjects.Add(newOrderInspectionSubject);
    //                }

    //                Contact newContact;
    //                for (int i = 0; i < orderContacts.Length; i++)
    //                {
    //                    newContact = new Contact();
    //                    newContact.Name = orderContacts[i].Name;
    //                    newContact.Email = orderContacts[i].Email;
    //                    newContact.Phone = orderContacts[i].Phone;
    //                    newContact.Cc = orderContacts[i].Cc;
    //                    //newContact.Address = orderContacts[i].Address;
    //                    newContact.Attention = orderContacts[i].Attention;
    //                    newContact.ContactType = orderContacts[i].ContactType;
    //                    newContact.CreatedBy = newOrderInInspectionSystem.CreatedBy;
    //                    newContact.CreatedDate = newOrderInInspectionSystem.CreatedDate;
    //                    newContact.ModifiedBy = newOrderInInspectionSystem.ModifiedBy;
    //                    newContact.ModifiedDate = newOrderInInspectionSystem.ModifiedDate;
    //                    newContact.HomePage = orderContacts[i].HomePage;
    //                    newContact.Fax = orderContacts[i].Fax;
    //                    newContact.DomainId = orderContacts[i].DomainId;
    //                    newContact.Mobile = orderContacts[i].Mobile;
    //                    newContact.IsVisible = orderContacts[i].IsVisible;
    //                    newContact.UserName = orderContacts[i].UserName != "" ? orderContacts[i].UserName : orderContacts[i].Name;

    //                    newContact.ContactId = ContactId.CreateUnique();
    //                    _dbcontext2_Scope2.Contacts.Add(newContact);
    //                    _dbcontext2_Scope2.SaveChanges();

    //                    OrderContact newOrderContact = new OrderContact();
    //                    newOrderContact.Contact = newContact;
    //                    newOrderContact.ContactId = newContact.ContactId;
    //                    newOrderContact.OrderId = newOrderInInspectionSystem.OrderId;
    //                    //newOrderContact.Order = newOrderInInspectionSystem;
    //                    _dbcontext2_Scope2.OrderContacts.Add(newOrderContact);
    //                }

    //                _dbcontext2_Scope2.SaveChanges();
    //                //transaction.Commit();
    //            }
    //            catch (Exception ex)
    //            {
    //                //transaction.Rollback();
    //            }
    //        }
    //        //}
    //    }
    //    catch (Exception exp)
    //    {
    //    }

    //    return await Task.FromResult(true);
    //}


    public Task<Order> GetEdmsOrderDataInQCIMS(int EdmsOrderId,Guid projectId)
    {
        try
        {

            var connection = _dbcontext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();


            var adoTransaction =
               _dbcontext.Database.CurrentTransaction?.GetDbTransaction();

            List<OrderInspectionSubject> orderInspectionSubjects;
            List<OrderContact> orderContacts;
            List<ProjectContact> projectContacts;
            List<OrdersettingException> ordersettingExceptions;
            Order? orderData;
            Project project1 = new Project();

            using (var multi = connection.QueryMultiple(
          "GetOrder_ByEdmsOrderId",
          new { EdmsOrderId = EdmsOrderId.ToString(),ProjectId=  projectId},
          commandType: CommandType.StoredProcedure,
          transaction: adoTransaction))
            {
                // 1. Read the main Order record.

                orderData = multi.Read().Select(o => new Order()
                {
                    OrderId = new OrderId(new Guid(o.OrderId.ToString())),
                    Id = new OrderId(new Guid(o.OrderId.ToString())),
                    Name = o.Name,
                    ModifiedDate = o.ModifiedDate,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedDate,
                    EdmsId = o.EdmsId,
                    EquipmentTagNo = o.EquipmentTagNo,
                    ITP_NO = o.ITP_NO,
                    ModifiedBy = o.ModifiedBy,
                    No = o.No,
                    Version = o.Version,
                    Subjects = o.Subjects,
                    Project = o.Project,
                    Title = o.Title
                }).FirstOrDefault();
                //orderData = multi.Read<Order>().SingleOrDefault();
                if (orderData == null)
                {
                    orderData = new Order();
                    orderData.OrderId = new OrderId(Guid.Empty);
                    orderData.EdmsId = EdmsOrderId.ToString();                    
                 }
                    //return null; // Or handle a missing record accordingly.

                // 2. Read the Project.

                // var st = multi.Read().FirstOrDefault();
                var project = multi.Read().Select(p => new Project()
                {
                    ProjectId = ProjectId.Create(new Guid(p.ProjectId.ToString())),
                    //Id = new Guid(p.ProjectId.Value!.ToString()),
                    FSId = p?.FSId!,
                    //CreatedBy = p.CreatedBy! ,
                    //CreatedDate = p.CreatedDate! ,
                    //ModifiedBy = p?.ModifiedBy!,
                    //ModifiedDate = p.ModifiedDate!,
                    // Contacts = p?.Contacts!,
                    Description = p?.Description,
                    Title = p?.Title!,
                    Number = p?.Number!,
                }).FirstOrDefault();

                orderData.Project = project!;

                // 3. Read OrderContacts and map the joined Contact.
                // The 'splitOn' parameter tells Dapper where the Contact columns start (using the alias "Id").
                //orderContacts = multi.Read<OrderContact, Contact, OrderContact>(
                //    (oc, contact) =>
                //    {
                //        oc.Contact = new Contact()
                //        {
                //            Phone = contact.Phone,
                //            Name = contact.Name,
                //            IsVisible = contact.IsVisible,
                //            UserName = contact.UserName,
                //            HomePage = contact.HomePage,
                //            Fax = contact.Fax,
                //            Attention = contact.Attention,
                //            Cc = contact.Cc,
                //            ContactType = contact.ContactType,
                //            Address = contact.Address,
                //            ContactId = contact.ContactId,
                //            CreatedBy = contact.CreatedBy,
                //            CreatedDate = contact.CreatedDate,
                //            ModifiedBy = contact.ModifiedBy,
                //            ModifiedDate = contact.ModifiedDate,
                //            DomainId = contact.DomainId,
                //            Email = contact.Email,
                //            Id = contact.Id,
                //            Mobile = contact.Mobile
                //        };
                //        return oc;
                //    },
                //    splitOn: "Id").ToList();
                //orderData.Contacts = orderContacts;


                var contactsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                orderContacts = new List<OrderContact>();

                foreach (var row in contactsDynamic)
                {
                    // Create the OrderContact instance.
                    var orderContact = new OrderContact
                    {
                        // Assuming your OrderContacts table has columns like OrderContactId, OrderId, ContactId, etc.
                        Id = row.Id,
                        OrderId = row.OrderId,
                        ContactId = row.ContactId,
                        // Map other OrderContact properties as needed.
                    };

                    // Now manually create the related Contact.
                    var contact = new Contact
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        ContactId = row.ContactId,      // the alias for c.ContactId in your query
                        Name = row.Name,
                        Email = row.Email,
                        DomainId = row.DomainId,
                        Fax = row.Fax,
                        HomePage = row.HomePage,
                        ContactType = row.ContactType,
                        IsVisible = row.IsVisible,
                        CreatedBy = row.CreatedBy,
                        CreatedDate = row.CreatedDate,
                        Note = row.Note,
                        Cc = row.Cc,
                        Mobile = row.Mobile,
                        Address = row.Address,
                        ModifiedBy = row.ModifiedBy,
                        ModifiedDate = row.ModifiedDate,
                        Phone = row.Phone,
                        UserName = row.UserName
                        // Map additional columns (e.g., row.OtherColumns)
                    };

                    orderContact.Contact = contact;
                    orderContacts.Add(orderContact);
                }

                orderData.Contacts = orderContacts;

                var pcontactsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                projectContacts = new List<ProjectContact>();

                foreach (var row in pcontactsDynamic)
                {
                    // Create the OrderContact instance.
                    var projectContact = new ProjectContact
                    {
                        // Assuming your OrderContacts table has columns like OrderContactId, OrderId, ContactId, etc.
                        Id = row.Id,
                        ProjectId = row.ProjectId,
                        ContactId = row.ContactId,
                        // Map other OrderContact properties as needed.
                    };

                    // Now manually create the related Contact.
                    var contact = new Contact
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        ContactId = row.ContactId,      // the alias for c.ContactId in your query
                        Name = row.Name,
                        Email = row.Email,
                        DomainId = row.DomainId,
                        Fax = row.Fax,
                        HomePage = row.HomePage,
                        ContactType = row.ContactType,
                        IsVisible = row.IsVisible,
                        CreatedBy = row.CreatedBy,
                        CreatedDate = row.CreatedDate,
                        Note = row.Note,
                        Cc = row.Cc,
                        Mobile = row.Mobile,
                        Address = row.Address,
                        ModifiedBy = row.ModifiedBy,
                        ModifiedDate = row.ModifiedDate,
                        Phone = row.Phone,
                        UserName = row.UserName
                        // Map additional columns (e.g., row.OtherColumns)
                    };

                    projectContact.Contact = contact;
                    projectContacts.Add(projectContact);
                }

                orderData.Project.MCContacts = new List<ProjectContact>();
                orderData.Project.OwnerContacts = new List<ProjectContact>();
                orderData.Project.PidecInspectors = new List<ProjectContact>();
                orderData.Project.TPInspectors = new List<ProjectContact>();

                for(int i = 0;i< projectContacts.Count;i++)
                {
                    switch (projectContacts[i].Contact.ContactType)
                    {
                        case 1://Owner Code == 1
                            orderData.Project.OwnerContacts.Add(projectContacts[i]);
                            break;
                        case 8: //PIdec Inspector
                            orderData.Project.PidecInspectors.Add(projectContacts[i]);
                            break;
                        case 9://Third-part Inspector
                            orderData.Project.TPInspectors.Add(projectContacts[i]);
                            break;
                        case 3://MC
                            orderData.Project.MCContacts.Add(projectContacts[i]);
                            break;

                        case null:
                            break;
                    } }
                orderData.Project.Contacts = projectContacts;




                var pexceptioncontactsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                ordersettingExceptions = new List<OrdersettingException>();
                OrdersettingException newOrderSettingException;

                foreach (var row in pexceptioncontactsDynamic)
                {
                    newOrderSettingException = new OrdersettingException();
                    // Create the OrderContact instance.
                    var projectContact = new ProjectContact
                    {
                        // Assuming your OrderContacts table has columns like OrderContactId, OrderId, ContactId, etc.
                        Id = row.Id,
                        ProjectId = row.ProjectId,
                        ContactId = row.ContactId,
                        // Map other OrderContact properties as needed.
                    };

                    // Now manually create the related Contact.
                    var contact = new Contact
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        ContactId = row.ContactId,      // the alias for c.ContactId in your query
                        Name = row.Name,
                        Email = row.Email,
                        DomainId = row.DomainId,
                        Fax = row.Fax,
                        HomePage = row.HomePage,
                        ContactType = row.ContactType,
                        IsVisible = row.IsVisible,
                        CreatedBy = row.CreatedBy,
                        CreatedDate = row.CreatedDate,
                        Note = row.Note,
                        Cc = row.Cc,
                        Mobile = row.Mobile,
                        Address = row.Address,
                        ModifiedBy = row.ModifiedBy,
                        ModifiedDate = row.ModifiedDate,
                        Phone = row.Phone,
                        UserName = row.UserName
                        // Map additional columns (e.g., row.OtherColumns)
                    };

                    projectContact.Contact = contact;

                    newOrderSettingException.OrderId = orderData.OrderId;
                    newOrderSettingException.ProjectContactId = projectContact.ProjectId;
                    newOrderSettingException.ProjectContact = projectContact;

                    ordersettingExceptions.Add(newOrderSettingException);
                }

                orderData.ExceptionContacts = ordersettingExceptions;

                var subjectsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                orderInspectionSubjects = new List<OrderInspectionSubject>();

                foreach (var row in subjectsDynamic)
                {
                    // Create the OrderInspectionSubject instance.
                    var orderInspectionSubject = new OrderInspectionSubject
                    {
                        // Assuming your OrderInspectionSubject table has columns like OrderInspectionSubjectId, OrderId, InspectionSubjectId, etc.
                        Id = row.Id,
                        OrderId = row.OrderId,
                        InspectionSubjectId = row.InspectionSubjectId,
                        // Map other OrderInspectionSubject properties as needed.
                    };

                    // Now manually create the related InspectionSubject.
                    var inspectionSubject = new InspectionSubject
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        InspectionSubjectId = row.InspectionSubjectId,      // the alias for c.InspectionSubjectId in your query
                        Name = row.Name
                    };

                    orderInspectionSubject.InspectionSubject = inspectionSubject;
                    orderInspectionSubjects.Add(orderInspectionSubject);
                }

                orderData.Subjects = orderInspectionSubjects;
                // 4. Read OrderInspectionSubjects and map the joined InspectionSubject.
                //var orderSubjects = multi.Read<OrderInspectionSubject, InspectionSubject, OrderInspectionSubject>(
                //    (ois, subject) =>
                //    {
                //        ois.InspectionSubject = new InspectionSubject()
                //        {
                //            InspectionSubjectId = InspectionSubjectId.Create(new Guid(subject.InspectionSubjectId.Value.ToString())),
                //            Name = subject.Name
                //        };

                //        return ois;
                //    },
                //    splitOn: "Id").ToList();
                //orderData.Subjects = orderSubjects;

                // 5. Read VendorDocuments.
                var vendorDocs = multi.Read<VendorDocument>().ToList();
                foreach (var doc in vendorDocs)
                {
                    var vendorDocument = new VendorDocument()
                    {
                        VendorDocumentId = VendorDocumentId.Create(new Guid(doc.VendorDocumentId.Value.ToString())),
                        Name = doc.Name,
                        Content = doc.Content,
                        CreatedBy = doc.CreatedBy,
                        CreatedDate = doc.CreatedDate,
                        Description = doc.Description,
                        edmsId = doc.edmsId,
                        ModifiedBy = doc.ModifiedBy,
                        ModifiedDate = doc.ModifiedDate,
                        PDFSize = doc.PDFSize,
                        Version = doc.Version,
                        Id = new Guid(doc.VendorDocumentId.Value.ToString())

                    };
                    orderData.AddVendorDocument(doc);
                }

                return Task.FromResult(orderData);
            }


            //    using (var multi = connection.QueryMultiple(
            //"GetOrder_ByEdmsOrderId",
            //new { EdmsOrderId = EdmsOrderId.ToString() },
            //commandType: CommandType.StoredProcedure))
            //{

            //    orderData = multi.Read().Select(o => new Order()
            //    {
            //        OrderId = new OrderId(new Guid(o.OrderId.ToString())),
            //        Id= new OrderId(new Guid(o.OrderId.ToString())),
            //        Name = o.Name,
            //        ModifiedDate = o.ModifiedDate,
            //        CreatedBy = o.CreatedBy,
            //        CreatedDate = o.CreatedDate,
            //        EdmsId = o.EdmsId,
            //        EquipmentTagNo = o.EquipmentTagNo,                    
            //        ITP_NO = o.ITP_NO,
            //        ModifiedBy = o.ModifiedBy,
            //        No = o.No,
            //        Version = o.Version,
            //        Subjects = o.Subjects,
            //        Project = o.Project,
            //        Title = o.Title
            //    }).FirstOrDefault();

            //    if (orderData != null && orderData!.OrderId!.Value != Guid.Empty )
            //    {


            //        orderContacts = multi.Read().Select(c => new Contact()
            //        {
            //            ContactId = new ContactId(new Guid(c.ContactId.ToString())),
            //            Attention = c.Attention,
            //            Cc = c.Cc,
            //            ContactType = c.ContactType,
            //            DomainId = c.DomainId,
            //            Fax = c.Fax,
            //            Email = c.Email,
            //            HomePage = c.HomePage,
            //            IsVisible = c.IsVisible,
            //            Mobile = c.Mobile,
            //            Name = c.Name,
            //            Phone = c.Phone,
            //            UserName = c.UserName,
            //            ModifiedDate = c.ModifiedDate,
            //            CreatedBy = c.CreatedBy,
            //            CreatedDate = c.CreatedDate,
            //            ModifiedBy = c.ModifiedBy,

            //        }).ToList();

            //        OrderContact newOrderContact;
            //        foreach (Contact contact in orderContacts) {
            //            newOrderContact = new OrderContact();
            //        }

            //        // Third result set
            //        orderInspectionSubjects = multi.Read().Select(o => new InspectionSubject()
            //        {
            //            InspectionSubjectId = new InspectionSubjectId(new Guid(o.InspectionSubjectId.ToString())),
            //            Name = o.Name
            //        }).ToList();

            //        connection.Close();
            //        return Task.FromResult(orderData);
            //    }
            //    else
            //    {
            //        connection.Close();
            //        return null;
            //    }

            //} 


        }
        catch (RuntimeBinderException exp)
        {
            string msg = exp.Message;
            return null;
        }
        catch (ValidationException exp)
        {
            return null;
        }
        catch (DivideByZeroException ex)
        {
            return null;
        }

        catch (Exception ex)
        {
            return null;
        }
    }

    public Task<Project> GetProjectDataInQCIMS(Guid ProjectId)
    {
        try
        {

            var connection = _dbcontext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();


            var adoTransaction =
            _dbcontext.Database.CurrentTransaction?.GetDbTransaction();

            
            List<ProjectContact> projectContacts;
            Project? projectData = null;
            Project project1 = new Project();

            using (var multi = connection.QueryMultiple(
          "GetProjectSettings_ProjectId",
          new { projectId = ProjectId },
          commandType: CommandType.StoredProcedure,
          transaction:adoTransaction))
            {
                // 1. Read the main Order record.

                projectData = multi.Read().Select(o => new Project()
                {
                    ProjectId = new ProjectId(new Guid(o.ProjectId.ToString())),
                    Id = new ProjectId(new Guid(o.ProjectId.ToString())),
                    FSId = o.FSId,
                    Number = o.Number,
                    Description = o.Description,                    
                    ModifiedDate = o.ModifiedDate,
                    CreatedBy = o.CreatedBy,
                    CreatedDate = o.CreatedDate,                    
                    ModifiedBy = o.ModifiedBy,                    
                    Title = o.Title
                }).FirstOrDefault();
                //projectData = multi.Read<Project>().SingleOrDefault();
                if (projectData == null)
                    return null; // Or handle a missing record accordingly.

                var PidecQCContactsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                projectContacts = new List<ProjectContact>();

                foreach (var row in PidecQCContactsDynamic)
                {
                    // Create the ProjectContact instance.
                    var projectContact = new ProjectContact
                    {
                        // Assuming your ProjectContact table has columns like ProjectContactId, ProjectId, ContactId, etc.
                        Id = row.Id,
                        ProjectId = row.ProjectId,
                        ContactId = row.ContactId,
                        // Map other ProjectContact properties as needed.
                    };

                    // Now manually create the related Contact.
                    var contact = new Contact
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        ContactId = row.ContactId,      // the alias for c.ContactId in your query
                        Name = row.Name,
                        Email = row.Email,
                        DomainId = row.DomainId,
                        Fax = row.Fax,
                        HomePage = row.HomePage,
                        ContactType = row.ContactType,
                        IsVisible = row.IsVisible,
                        CreatedBy = row.CreatedBy,
                        CreatedDate = row.CreatedDate,
                        Note = row.Note,
                        Cc = row.Cc,
                        Mobile = row.Mobile,
                        Address = row.Address,
                        ModifiedBy = row.ModifiedBy,
                        ModifiedDate = row.ModifiedDate,
                        Phone = row.Phone,
                        UserName = row.UserName
                        // Map additional columns (e.g., row.OtherColumns)
                    };


                    projectContact.Contact = contact;
                    projectContacts.Add(projectContact);
                }

                projectData.PidecQCContacts = projectContacts;

                var PidecInspectorContactsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                projectContacts = new List<ProjectContact>();

                foreach (var row in PidecInspectorContactsDynamic)
                {
                    // Create the ProjectContact instance.
                    var projectContact = new ProjectContact
                    {
                        // Assuming your ProjectContact table has columns like ProjectContactId, ProjectId, ContactId, etc.
                        Id = row.Id,
                        ProjectId = row.ProjectId,
                        ContactId = row.ContactId,
                        // Map other ProjectContact properties as needed.
                    };

                    // Now manually create the related Contact.
                    var contact = new Contact
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        ContactId = row.ContactId,      // the alias for c.ContactId in your query
                        Name = row.Name,
                        Email = row.Email,
                        DomainId = row.DomainId,
                        Fax = row.Fax,
                        HomePage = row.HomePage,
                        ContactType = row.ContactType,
                        IsVisible = row.IsVisible,
                        CreatedBy = row.CreatedBy,
                        CreatedDate = row.CreatedDate,
                        Note = row.Note,
                        Cc = row.Cc,
                        Mobile = row.Mobile,
                        Address = row.Address,
                        ModifiedBy = row.ModifiedBy,
                        ModifiedDate = row.ModifiedDate,
                        Phone = row.Phone,
                        UserName = row.UserName
                        // Map additional columns (e.g., row.OtherColumns)
                    };

                   
                    projectContact.Contact = contact;
                    projectContacts.Add(projectContact);
                }

                projectData.PidecInspectors = projectContacts;

                var TPInspectorContactsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                projectContacts = new List<ProjectContact>();

                foreach (var row in TPInspectorContactsDynamic)
                {
                    // Create the ProjectContact instance.
                    var projectContact = new ProjectContact
                    {
                        // Assuming your ProjectContact table has columns like ProjectContactId, ProjectId, ContactId, etc.
                        Id = row.Id,
                        ProjectId = row.ProjectId,
                        ContactId = row.ContactId,
                        // Map other ProjectContact properties as needed.
                    };

                    // Now manually create the related Contact.
                    var contact = new Contact
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        ContactId = row.ContactId,      // the alias for c.ContactId in your query
                        Name = row.Name,
                        Email = row.Email,
                        DomainId = row.DomainId,
                        Fax = row.Fax,
                        HomePage = row.HomePage,
                        ContactType = row.ContactType,
                        IsVisible = row.IsVisible,
                        CreatedBy = row.CreatedBy,
                        CreatedDate = row.CreatedDate,
                        Note = row.Note,
                        Cc = row.Cc,
                        Mobile = row.Mobile,
                        Address = row.Address,
                        ModifiedBy = row.ModifiedBy,
                        ModifiedDate = row.ModifiedDate,
                        Phone = row.Phone,
                        UserName = row.UserName
                        // Map additional columns (e.g., row.OtherColumns)
                    };

                    projectContact.Contact = contact;
                    projectContacts.Add(projectContact);
                }

                projectData.TPInspectors = projectContacts;

                var OwnerContactsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                projectContacts = new List<ProjectContact>();

                foreach (var row in OwnerContactsDynamic)
                {
                    // Create the ProjectContact instance.
                    var projectContact = new ProjectContact
                    {
                        // Assuming your ProjectContact table has columns like ProjectContactId, ProjectId, ContactId, etc.
                        Id = row.Id,
                        ProjectId = row.ProjectId,
                        ContactId = row.ContactId,
                        // Map other ProjectContact properties as needed.
                    };

                    // Now manually create the related Contact.
                    var contact = new Contact
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        ContactId = row.ContactId,      // the alias for c.ContactId in your query
                        Name = row.Name,
                        Email = row.Email,
                        DomainId = row.DomainId,
                        Fax = row.Fax,
                        HomePage = row.HomePage,
                        ContactType = row.ContactType,
                        IsVisible = row.IsVisible,
                        CreatedBy = row.CreatedBy,
                        CreatedDate = row.CreatedDate,
                        Note = row.Note,
                        Cc = row.Cc,
                        Mobile = row.Mobile,
                        Address = row.Address,
                        ModifiedBy = row.ModifiedBy,
                        ModifiedDate = row.ModifiedDate,
                        Phone = row.Phone,
                        UserName = row.UserName
                        // Map additional columns (e.g., row.OtherColumns)
                    };

                    projectContact.Contact = contact;
                    projectContacts.Add(projectContact);
                }

                projectData.OwnerContacts = projectContacts;

                var MCContactsDynamic = multi.Read(); // returns IEnumerable<dynamic>
                projectContacts = new List<ProjectContact>();

                foreach (var row in MCContactsDynamic)
                {
                    // Create the ProjectContact instance.
                    var projectContact = new ProjectContact
                    {
                        // Assuming your ProjectContact table has columns like ProjectContactId, ProjectId, ContactId, etc.
                        Id = row.Id,
                        ProjectId = row.ProjectId,
                        ContactId = row.ContactId,
                        // Map other ProjectContact properties as needed.
                    };

                    // Now manually create the related Contact.
                    var contact = new Contact
                    {
                        // Notice here you’re reading the alias you defined in your SQL (for the joined Contact columns).
                        ContactId = row.ContactId,      // the alias for c.ContactId in your query
                        Name = row.Name,
                        Email = row.Email,
                        DomainId = row.DomainId,
                        Fax = row.Fax,
                        HomePage = row.HomePage,
                        ContactType = row.ContactType,
                        IsVisible = row.IsVisible,
                        CreatedBy = row.CreatedBy,
                        CreatedDate = row.CreatedDate,
                        Note = row.Note,
                        Cc = row.Cc,
                        Mobile = row.Mobile,
                        Address = row.Address,
                        ModifiedBy = row.ModifiedBy,
                        ModifiedDate = row.ModifiedDate,
                        Phone = row.Phone,
                        UserName = row.UserName
                        // Map additional columns (e.g., row.OtherColumns)
                    };

                    projectContact.Contact = contact;
                    projectContacts.Add(projectContact);
                }

                projectData.MCContacts = projectContacts;






                return Task.FromResult(projectData);
            }


            //    using (var multi = connection.QueryMultiple(
            //"GetOrder_ByEdmsOrderId",
            //new { EdmsOrderId = EdmsOrderId.ToString() },
            //commandType: CommandType.StoredProcedure))
            //{

            //    orderData = multi.Read().Select(o => new Order()
            //    {
            //        OrderId = new OrderId(new Guid(o.OrderId.ToString())),
            //        Id= new OrderId(new Guid(o.OrderId.ToString())),
            //        Name = o.Name,
            //        ModifiedDate = o.ModifiedDate,
            //        CreatedBy = o.CreatedBy,
            //        CreatedDate = o.CreatedDate,
            //        EdmsId = o.EdmsId,
            //        EquipmentTagNo = o.EquipmentTagNo,                    
            //        ITP_NO = o.ITP_NO,
            //        ModifiedBy = o.ModifiedBy,
            //        No = o.No,
            //        Version = o.Version,
            //        Subjects = o.Subjects,
            //        Project = o.Project,
            //        Title = o.Title
            //    }).FirstOrDefault();

            //    if (orderData != null && orderData!.OrderId!.Value != Guid.Empty )
            //    {


            //        orderContacts = multi.Read().Select(c => new Contact()
            //        {
            //            ContactId = new ContactId(new Guid(c.ContactId.ToString())),
            //            Attention = c.Attention,
            //            Cc = c.Cc,
            //            ContactType = c.ContactType,
            //            DomainId = c.DomainId,
            //            Fax = c.Fax,
            //            Email = c.Email,
            //            HomePage = c.HomePage,
            //            IsVisible = c.IsVisible,
            //            Mobile = c.Mobile,
            //            Name = c.Name,
            //            Phone = c.Phone,
            //            UserName = c.UserName,
            //            ModifiedDate = c.ModifiedDate,
            //            CreatedBy = c.CreatedBy,
            //            CreatedDate = c.CreatedDate,
            //            ModifiedBy = c.ModifiedBy,

            //        }).ToList();

            //        OrderContact newOrderContact;
            //        foreach (Contact contact in orderContacts) {
            //            newOrderContact = new OrderContact();
            //        }

            //        // Third result set
            //        orderInspectionSubjects = multi.Read().Select(o => new InspectionSubject()
            //        {
            //            InspectionSubjectId = new InspectionSubjectId(new Guid(o.InspectionSubjectId.ToString())),
            //            Name = o.Name
            //        }).ToList();

            //        connection.Close();
            //        return Task.FromResult(orderData);
            //    }
            //    else
            //    {
            //        connection.Close();
            //        return null;
            //    }

            //} 


        }
        catch (RuntimeBinderException exp)
        {
            string msg = exp.Message;
            return null;
        }
        catch (ValidationException exp)
        {
            return null;
        }
        catch (DivideByZeroException ex)
        {
            return null;
        }

        catch (Exception ex)
        {
            return null;
        }
    }

    //public Task<bool> ActivateEdmsOrderInInspectionSystem(Order order)
    //{
    //    try
    //    {
    //        var connection = _dbcontext.Database.GetDbConnection();
    //        if (connection.State != ConnectionState.Open)
    //            connection.Open();

    //        using (var transaction = connection.BeginTransaction())
    //        {
    //            try
    //            {
    //                Order newOrderInInspectionSystem = Order.Create(order.Name, order.Title, order.No, order.Project, order.EdmsId);
    //                newOrderInInspectionSystem.OrderId = OrderId.CreateUnique();

    //                newOrderInInspectionSystem.ITP_NO = order.ITP_NO;
    //                newOrderInInspectionSystem.EdmsId = order.EdmsId;
    //                newOrderInInspectionSystem.Version = 0;
    //                newOrderInInspectionSystem.EquipmentTagNo = order.EquipmentTagNo;

    //                InspectionSubject newInspectionSubject;
    //                for (int i = 0; i < order.Subjects.Countth; i++)
    //                {
    //                    newInspectionSubject = new InspectionSubject();
    //                    newInspectionSubject.Name = subjects[i].Name;
    //                    newInspectionSubject.InspectionSubjectId = InspectionSubjectId.CreateUnique();
    //                    _dbcontext.InspectionSubjects.Add(newInspectionSubject);

    //                    OrderInspectionSubject newOrderInspectionSubject = new OrderInspectionSubject();
    //                    newOrderInspectionSubject.InspectionSubject = newInspectionSubject;
    //                    newOrderInspectionSubject.Order = newOrderInInspectionSystem;
    //                }

    //                Contact newContact;
    //                for (int i = 0; i < orderContacts.Length; i++)
    //                {
    //                    newContact = new Contact();
    //                    newContact.Name = orderContacts[i].Name;
    //                    newContact.Email = orderContacts[i].Email;
    //                    newContact.Phone = orderContacts[i].Phone;
    //                    newContact.Cc = orderContacts[i].Cc;
    //                    newContact.Address = orderContacts[i].Address;
    //                    newContact.Attention = orderContacts[i].Attention;
    //                    newContact.ContactType = orderContacts[i].ContactType;
    //                    newContact.CreatedBy = newOrderInInspectionSystem.CreatedBy;
    //                    newContact.CreatedDate = newOrderInInspectionSystem.CreatedDate;
    //                    newContact.ModifiedBy = newOrderInInspectionSystem.ModifiedBy;
    //                    newContact.ModifiedDate = newOrderInInspectionSystem.ModifiedDate;
    //                    newContact.HomePage = orderContacts[i].HomePage;
    //                    newContact.Fax = orderContacts[i].Fax;
    //                    newContact.DomainId = orderContacts[i].DomainId;
    //                    newContact.Mobile = orderContacts[i].Mobile;
    //                    newContact.IsVisible = orderContacts[i].IsVisible;

    //                    newContact.ContactId = ContactId.CreateUnique();
    //                    _dbcontext.Contacts.Add(newContact);

    //                    OrderContact newOrderContact = new OrderContact();
    //                    newOrderContact.Contact = newContact;
    //                    newOrderContact.Order = newOrderInInspectionSystem;
    //                    _dbcontext.OrderContacts.Add(newOrderContact);
    //                }

    //                _dbcontext.SaveChanges();
    //                transaction.Commit();
    //            }
    //            catch (Exception ex)
    //            {
    //                transaction.Rollback();
    //            }
    //        }
    //    }
    //    catch (Exception exp)
    //    {
    //    }

    //    return Task.FromResult(true);
    //}




}


public class InspectionNotificationIdTypeHandler : SqlMapper.TypeHandler<InspectionNotificationId>
{
    public override void SetValue(IDbDataParameter parameter, InspectionNotificationId value)
    {
        parameter.Value = value?.Value;
    }

    public override InspectionNotificationId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return InspectionNotificationId.Create(Guid.Parse(value.ToString()));
    }
}

public class InspectionReportIdTypeHandler : SqlMapper.TypeHandler<InspectionReportId>
{
    public override void SetValue(IDbDataParameter parameter, InspectionReportId value)
    {
        parameter.Value = value?.Value;
    }

    public override InspectionReportId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return InspectionReportId.Create(Guid.Parse(value.ToString()));
    }
}

public class ProjectIdTypeHandler : SqlMapper.TypeHandler<ProjectId>
{
    public override void SetValue(IDbDataParameter parameter, ProjectId value)
    {
        parameter.Value = value?.Value;
    }

    public override ProjectId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return ProjectId.Create(Guid.Parse(value.ToString()));
    }
}

public class OrderIdTypeHandler : SqlMapper.TypeHandler<OrderId>
{
    public override void SetValue(IDbDataParameter parameter, OrderId value)
    {
        parameter.Value = value?.Value;
    }

    public override OrderId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return OrderId.Create(Guid.Parse(value.ToString()));
    }
}
public class ContactIdTypeHandler : SqlMapper.TypeHandler<ContactId>
{
    public override void SetValue(IDbDataParameter parameter, ContactId value)
    {
        parameter.Value = value?.Value;
    }

    public override ContactId Parse(object value)
    {
        if (value == null || value is DBNull)
            throw new InvalidOperationException("Cannot map null to ContactId");

        return ContactId.Create((Guid)value);
    }

}

public class InspectionSubjectIdTypeHandler : SqlMapper.TypeHandler<InspectionSubjectId>
{
    public override void SetValue(IDbDataParameter parameter, InspectionSubjectId value)
    {
        parameter.Value = value?.Value;
    }

    public override InspectionSubjectId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return InspectionSubjectId.Create(Guid.Parse(value.ToString()));
    }
}
public class AttachmentIdTypeHandler : SqlMapper.TypeHandler<AttachmentId>
{
    public override void SetValue(IDbDataParameter parameter, AttachmentId value)
    {
        parameter.Value = value?.Value;
    }

    public override AttachmentId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return AttachmentId.Create(Guid.Parse(value.ToString()));
    }
}
public class InspectionTypeIdTypeHandler : SqlMapper.TypeHandler<InspectionTypeId>
{
    public override void SetValue(IDbDataParameter parameter, InspectionTypeId value)
    {
        parameter.Value = value?.Value;
    }

    public override InspectionTypeId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return InspectionTypeId.Create(Guid.Parse(value.ToString()));
    }
}

public class InspectionReportAttachmentIdTypeHandler : SqlMapper.TypeHandler<InspectionReportAttachmentId>
{
    public override void SetValue(IDbDataParameter parameter, InspectionReportAttachmentId value)
    {
        parameter.Value = value?.Value;
    }

    public override InspectionReportAttachmentId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return InspectionReportAttachmentId.Create(Guid.Parse(value.ToString()));
    }
}

public class InspectorIdTypeHandler : SqlMapper.TypeHandler<InspectorId>
{
    public override void SetValue(IDbDataParameter parameter, InspectorId value)
    {
        parameter.Value = value?.Value;
    }

    public override InspectorId Parse(object value)
    {
        if (value == null || value is DBNull)
        {
            return null; // or throw a custom exception if null is not acceptable in your domain
        }
        return InspectorId.Create(Guid.Parse(value.ToString()));
    }
}
