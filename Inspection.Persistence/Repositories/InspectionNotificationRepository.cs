using Azure.Core;
using Dapper;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Models;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.EDMS;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.User.Enums;
using Inspection.Domain.Entities.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pidec.Utils;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
using Request = Inspection.Domain.Entities.Common.Request;
//using Pidec.Utils;

namespace Inspection.Persistence.Repositories;

public class InspectionNotificationRepository : IGenericRepository<InspectionNotification, InspectionNotificationId>, IInspectionNotificationRepository
{

    private readonly InspectionDbContext _dbContext;
    private IServiceScopeFactory _serviceProvider;
    private IEDMSService _eDMSService;
    private IUserManagerRepository _userManagerRepository;

    public InspectionNotificationRepository(InspectionDbContext dbContext, IServiceScopeFactory serviceProvider, IEDMSService eDMSService, IUserManagerRepository userManagerRepository) : base()
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _eDMSService = eDMSService;
        _userManagerRepository = userManagerRepository;
    }

    public async Task<Contact> AddNewContact(Contact contact)
    {

        if (contact == null)
        {
            throw new Exception("Contact is null");
        }
        var addedContact = _dbContext.Contacts.Add(contact);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(addedContact.Entity);
    }

    public async Task<Contact> GetContactByUserId(string userId)
    {
        if (userId == null)
        {
            throw new Exception("UserId Is Null");
        }

        return await Task.FromResult(_dbContext.Contacts.Where(ct => ct.UserId == userId).FirstOrDefault());
    }

    public async Task<InspectionNotification> AddNewInspectionNotification(InspectionNotification inspectionNotification)
    {

        try
        {
            if (inspectionNotification == null)
            {
                throw new ArgumentNullException("inspection has some errors");
            }

            var addedInspectionNotification = await _dbContext.InspectionNotifications.AddAsync(inspectionNotification);
            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(addedInspectionNotification.Entity);
        }
        catch (Exception exp)
        {
            return null;
        }
    }

    public async Task<InspectionNotificationFlow> GetInspectionNotificationFlow(string notificationNumber)
    {
        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();


            List<InspectionNotification> inspectionNotifications;
            List<Contact> contacts;
            List<InspectionType> inspectionTypes;
            List<InspectionNotificationSubject> inspectionNotificationSubjects;
            List<InspectionNotificationContact> inspectionNotificationContacts;
            List<InspectionNotificationType> inspectionNotificationTypes;
            List<Circulation> participatedCirculations;

            using (var multi = connection.QueryMultiple(
            "GetInspectionNotificationData",
            new { NotificationNumber = notificationNumber },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {


                inspectionNotifications = multi.Read().Select(ino => new InspectionNotification()
                {
                    InspectionNotificationId = new InspectionNotificationId(new Guid(ino.InspectionNotificationId.ToString())),
                    ModifiedDate = ino.ModifiedDate,
                    CreatedBy = ino.CreatedBy,
                    CreatedDate = ino.CreatedDate,
                    Description = ino.Description,
                    Id = ino.Id,
                    IssueDate = ino.IssueDate,
                    NotificationNumber = notificationNumber,
                    PersianIssueDate = ino.PersianIssueDate,
                    RequestInspectionDateEnd = ino.RequestInspectionDateEnd,
                    RequestInspectionDateStart = ino.RequestInspectionDateStart,
                    RequestInspectionDueDate = ino.RequestInspectionDueDate,                  
                    RequestInspectionPersianDateEnd = ino.RequestInspectionPersianDateEnd,
                    RequestInspectionPersianDateStart = ino.RequestInspectionPersianDateStart,
                    InspectionAction= ino.InspectionAction,
                    InspectionActionComment = ino.InspectionActionComment,
                    InspectionTimeComment = ino.InspectionTimeComment,
                    NotificationNumberParts = ino.NotificationNumberParts,
                    //Version = ino.Version


                }).ToList();


                participatedCirculations = multi.Read().Select(cr => new Circulation()
                {

                    ActionType = cr.ActionType,
                    CreatedBy = cr.CreatedBy,
                    CreatedDate = cr.CreatedDate,
                    Id = cr.Id,
                    ModifiedBy = cr.ModifiedBy,
                    ModifiedDate = cr.ModifiedDate,
                    PerformedActionDate = cr.PerformedActionDate,
                    PerformedActionDateGregorian = cr.PerformedActionDateGregorian,
                    PerformerContactPerson = cr.PerformerContactPerson,
                    PerformerContactType = cr.PerformerContactType,
                    PerformerPN = cr.PerformerPN,

                    SequenceNo = cr.SequenceNo
                }).ToList();




                inspectionNotifications = multi.Read<InspectionNotification, Order, InspectionNotification>(
  (inspectionNotification, order) =>
  {
      inspectionNotification.Order = order;

      if (inspectionNotification?.InspectionNotificationId! != null)
      {
          inspectionNotification.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotification.InspectionNotificationId.Value.ToString()));
      }

      return inspectionNotification;
  },
  splitOn: "OrderId"
).ToList();



                inspectionNotificationContacts = multi.Read<InspectionNotificationContact, Contact, InspectionNotificationContact>(
  (inspectionNotificationContact, contact) =>
  {
      inspectionNotificationContact.Contact = contact;
      if (inspectionNotificationContact?.InspectionNotificationId! != null)
      {
          inspectionNotificationContact.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotificationContact.InspectionNotificationId.Value.ToString()));
      }
      inspectionNotificationContact.ContactId = contact.ContactId;
      inspectionNotificationContact.Id = inspectionNotificationContact.Id;
      return inspectionNotificationContact;
  },
  splitOn: "ContactId"
).ToList();

                // Third result set

                inspectionNotificationSubjects = multi.Read<InspectionNotificationSubject, InspectionSubject, InspectionNotificationSubject>(
         (inspectionNotificationSubject, inspectionSubject) =>
         {
             inspectionNotificationSubject.InspectionSubject = inspectionSubject;
             if (inspectionNotificationSubject.InspectionNotificationId != null)
             {
                 inspectionNotificationSubject.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotificationSubject.InspectionNotificationId.Value.ToString()));
             }
             inspectionNotificationSubject!.InspectionSubjectId = inspectionSubject.InspectionSubjectId;
             inspectionNotificationSubject.Id = inspectionNotificationSubject.Id;
             return inspectionNotificationSubject;
         },
         splitOn: "InspectionSubjectId"
     ).ToList();


                inspectionNotificationTypes = multi.Read<InspectionNotificationType, InspectionType, InspectionNotificationType>(
    (inspectionNotificationType, inspectionType) =>
    {
        inspectionNotificationType.InspectionType = inspectionType;
        if (inspectionNotificationType.InspectionNotificationId != null)
        {
            inspectionNotificationType.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotificationType.InspectionNotificationId.Value.ToString()));
        }
        inspectionNotificationType!.InspectionTypeId = inspectionType.InspectionTypeId;
        inspectionNotificationType.Id = inspectionNotificationType.Id;
        return inspectionNotificationType;
    },
    splitOn: "InspectionTypeId"
).ToList();

            }

            // connection.Close();


            InspectionNotificationFlow inspectionNotificationFlow = new InspectionNotificationFlow();
            inspectionNotificationFlow.InspectionNotifications = inspectionNotifications;
            inspectionNotificationFlow.Circulations = participatedCirculations;
            inspectionNotificationFlow.Contacts = inspectionNotificationContacts;
            inspectionNotificationFlow.Subjects = inspectionNotificationSubjects;
            inspectionNotificationFlow.Types = inspectionNotificationTypes;

            return await Task.FromResult(inspectionNotificationFlow);
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

    public async Task<List<Circulation>> GiveEditRightToSpecificRole(Circulation contributor, Circulation[] assignees) {

     //   Circulation contributorCirc = _dbContext.Circulations.Where(cr => cr.Id == contributor.Id).FirstOrDefault();
               
        List<Circulation> InspectorCircs = new List<Circulation>();
        List<Circulation> NotRegisteredAssignees = assignees.Where(asn => asn.Id == null || asn.Id == Guid.Empty).ToList();
        InspectorCircs = Circulation.CreateByAdminCirculation(contributor, NotRegisteredAssignees);

        for (int i = 0; i < NotRegisteredAssignees.Count; i++)
        {
            _dbContext.Attach(NotRegisteredAssignees[i].PerformerContactPerson);
        }
        _dbContext.Circulations.AddRange(InspectorCircs);
        _dbContext.SaveChanges();
        return await Task.FromResult(InspectorCircs);
    }

    //public async Task<InspectionNotification> UpdateInspectionNotification(InspectionNotification inspectionNotification)
    //{
    //    InspectionNotification currentInspection = _dbContext.InspectionNotifications.Where(ins => ins.RevisionId == inspectionNotification.RevisionId).Include(ins => ins.Order)
    //                        .Include(ins => ins.Subjects).Include(ins => ins.Types).Include(ins => ins.Locations).Include(ins => ins.Contacts).ThenInclude(cnt => cnt.Contact).FirstOrDefault();

    //    InspectionNotification NewRevisionInspectionNotification = currentInspection.DeepClone();

    //    //InspectionNotification existingInspectionNotification = _dbContext.InspectionNotifications.Where(ins => ins.InspectionNotificationId == inspectionNotification.InspectionNotificationId)
    //    //    .AsNoTracking().FirstOrDefault();
    //    //if (existingInspectionNotification != null)
    //    //{

    //    //    try
    //    //    {
    //    //        var inspectionNotificationId = inspectionNotification.InspectionNotificationId; // The parent entity you want to remove relationships for.


    //    //        //------- Clean Up Old Contacts and InspectionNotificationContacts in DataBase-----------------
    //    //        var contactsInInspectionNotificationContact = _dbContext.InspectionNotificationsContacts.Where(c => c.InspectionNotificationId == inspectionNotificationId)
    //    //        .Select(isc => isc.ContactId)
    //    //        .Distinct()
    //    //        .ToList();

    //    //        var existedInspectionNotificationContacts = _dbContext.InspectionNotificationsContacts.Where(c => c.InspectionNotificationId == inspectionNotificationId).ToList();
    //    //        _dbContext.InspectionNotificationsContacts.RemoveRange(existedInspectionNotificationContacts);

    //    //        //----Then I  cleanup database from contacts that are not used at the moment
    //    //        //----at first find that contacts are used in which tables-----------------
    //    //        //----Then search which contacts are not referenced in other tables and delete them---------
    //    //        var foreignKeys = _dbContext.Model.FindEntityType(typeof(Contact))?
    //    //        .GetReferencingForeignKeys();

    //    //        var contactsUsedElsewhere = new HashSet<Guid>();

    //    //        foreach (var fk in foreignKeys)
    //    //        {
    //    //            Type relatedTable = fk.DeclaringEntityType.ClrType;

    //    //            // Use reflection to get DbSet dynamically
    //    //            var method = typeof(DbContext).GetMethod(nameof(DbContext.Set), Array.Empty<Type>());
    //    //            var genericMethod = method.MakeGenericMethod(relatedTable);
    //    //            var tableSet = genericMethod.Invoke(_dbContext, null) as IQueryable;


    //    //            var usedContactIds = tableSet
    //    //             .Cast<object>()
    //    //             .AsEnumerable() // Force the remainder of the query to be executed in memory
    //    //             .Select(entity =>
    //    //             {
    //    //                 if (fk.Properties != null && fk.Properties.Count>0 && fk.Properties.First().PropertyInfo != null)
    //    //                 {
    //    //                     var value = fk.Properties.First().PropertyInfo!.GetValue(entity);
    //    //                     if (value == null)
    //    //                     {
    //    //                         return Guid.Empty;
    //    //                     }
    //    //                     else
    //    //                     {
    //    //                         // Explicitly cast to ContactId and get its Guid value.
    //    //                         // Adjust .Value if your ContactId type exposes the Guid differently.
    //    //                         return ContactId.Create((Guid)value).Value;
    //    //                         //return ((ContactId)value).Value;
    //    //                     }
    //    //                 }
    //    //                 else
    //    //                 {
    //    //                     return Guid.Empty;

    //    //                 }
    //    //             }).Distinct().ToList();

    //    //            contactsUsedElsewhere.UnionWith(usedContactIds);
    //    //        }

    //    //        var orphanedContacts = contactsInInspectionNotificationContact
    //    //        .Where(cid => !contactsUsedElsewhere.Contains(cid.Value))
    //    //        .ToList();

    //    //        // Convert your orphaned value objects to their underlying Guid values.
    //    //        var orphanedContactGuids = orphanedContacts.Select(x => x).ToList();

    //    //        // Switch to client evaluation by calling AsEnumerable() so that the .Value accessor is evaluated in C#.
    //    //        var contactsToRemove = _dbContext.Contacts
    //    //            .AsEnumerable() // This forces the remainder of the query to run in memory.
    //    //            .Where(c => orphanedContactGuids.Contains(c.ContactId))
    //    //            .ToList();

    //    //        _dbContext.Contacts.RemoveRange(contactsToRemove);

    //    //        //---------------End Cleanup Contacts and InspectionNotificationContacts---------------------------------

    //    //        //------- Clean Up Old InspectionSubjects and InspectionNotificationSubjects in DataBase-----------------
    //    //        var subjectsInInspectionNotificationSubjects = _dbContext.InspectionNotificationSubjects.Where(c => c.InspectionNotificationId == inspectionNotificationId)
    //    //        .Select(iss => iss.InspectionSubjectId)
    //    //        .Distinct()
    //    //        .ToList();

    //    //        var existedInspectionNotificationSubjects = _dbContext.InspectionNotificationSubjects.Where(c => c.InspectionNotificationId == inspectionNotificationId).ToList();
    //    //        _dbContext.InspectionNotificationSubjects.RemoveRange(existedInspectionNotificationSubjects);

    //    //        //----Then I  cleanup database from InspectionSubjects that are not used at the moment
    //    //        //----at first find that InspectionSubjects are used in which tables-----------------
    //    //        //----Then search which InspectionSubjects are not referenced in other tables and delete them---------
    //    //        var SubjectforeignKeys = _dbContext.Model.FindEntityType(typeof(InspectionSubject))?
    //    //        .GetReferencingForeignKeys();

    //    //        var inspectionSubjectsUsedElsewhere = new HashSet<Guid>();

    //    //        foreach (var fk in SubjectforeignKeys)
    //    //        {
    //    //            Type relatedTable = fk.DeclaringEntityType.ClrType;

    //    //            // Use reflection to get DbSet dynamically
    //    //            var method = typeof(DbContext).GetMethod(nameof(DbContext.Set), Array.Empty<Type>());
    //    //            var genericMethod = method.MakeGenericMethod(relatedTable);
    //    //            var tableSet = genericMethod.Invoke(_dbContext, null) as IQueryable;


    //    //            var usedInspectionSubjectIds = tableSet
    //    //             .Cast<object>()
    //    //             .AsEnumerable() // Force the remainder of the query to be executed in memory
    //    //             .Select(entity =>
    //    //             {
    //    //                 var value = fk.Properties.First().PropertyInfo.GetValue(entity);
    //    //                 if (value == null)
    //    //                 {
    //    //                     return Guid.Empty;
    //    //                 }
    //    //                 else
    //    //                 {
    //    //                     // Explicitly cast to ContactId and get its Guid value.
    //    //                     // Adjust .Value if your ContactId type exposes the Guid differently.
    //    //                     return ((InspectionSubjectId)value).Value;
    //    //                 }
    //    //             }).Distinct().ToList();

    //    //            inspectionSubjectsUsedElsewhere.UnionWith(usedInspectionSubjectIds);
    //    //        }

    //    //        var orphanedInspectionSubjects = subjectsInInspectionNotificationSubjects
    //    //        .Where(cid => !inspectionSubjectsUsedElsewhere.Contains(cid))
    //    //        .ToList();

    //    //        // Convert your orphaned value objects to their underlying Guid values.
    //    //        var orphanedInspectionSubjectsGuids = orphanedInspectionSubjects.Select(x => x.Value).ToList();

    //    //        // Switch to client evaluation by calling AsEnumerable() so that the .Value accessor is evaluated in C#.
    //    //        var inspectionSubjectsToRemove = _dbContext.InspectionSubjects
    //    //            .AsEnumerable() // This forces the remainder of the query to run in memory.
    //    //            .Where(c => orphanedInspectionSubjectsGuids.Contains(c.InspectionSubjectId.Value))
    //    //            .ToList();

    //    //        _dbContext.InspectionSubjects.RemoveRange(inspectionSubjectsToRemove);
    //    //        //---------------End Cleanup InspectionSubjects and InspectionNotificationSubjects---------------------------------

    //    //        //------- Clean Up Old InspectionTypes and InspectionNotificationTypes in DataBase-----------------
    //    //        var typesInInspectionNotificationTypes = _dbContext.InspectionNotificationsTypes.Where(c => c.InspectionNotificationId == inspectionNotificationId)
    //    //        .Select(isc => isc.InspectionTypeId)
    //    //        .Distinct()
    //    //        .ToList();

    //    //        var existedInspectionNotificationTypes = _dbContext.InspectionNotificationsTypes.Where(c => c.InspectionNotificationId == inspectionNotificationId).ToList();
    //    //        _dbContext.InspectionNotificationsTypes.RemoveRange(existedInspectionNotificationTypes);

    //    //        //----Then I  cleanup database from contacts that are not used at the moment
    //    //        //----at first find that contacts are used in which tables-----------------
    //    //        //----Then search which contacts are not referenced in other tables and delete them---------
    //    //        var typesforeignKeys = _dbContext.Model.FindEntityType(typeof(InspectionType))?
    //    //        .GetReferencingForeignKeys();

    //    //        var inspectionTypesUsedElsewhere = new HashSet<Guid>();

    //    //        foreach (var fk in typesforeignKeys)
    //    //        {
    //    //            Type relatedTable = fk.DeclaringEntityType.ClrType;

    //    //            // Use reflection to get DbSet dynamically
    //    //            var method = typeof(DbContext).GetMethod(nameof(DbContext.Set), Array.Empty<Type>());
    //    //            var genericMethod = method.MakeGenericMethod(relatedTable);
    //    //            var tableSet = genericMethod.Invoke(_dbContext, null) as IQueryable;


    //    //            var usedInspectionTypeIds = tableSet
    //    //             .Cast<object>()
    //    //             .AsEnumerable() // Force the remainder of the query to be executed in memory
    //    //             .Select(entity =>
    //    //             {
    //    //                 var value = fk.Properties.First().PropertyInfo.GetValue(entity);
    //    //                 if (value == null)
    //    //                 {
    //    //                     return Guid.Empty;
    //    //                 }
    //    //                 else
    //    //                 {
    //    //                     // Explicitly cast to ContactId and get its Guid value.
    //    //                     // Adjust .Value if your ContactId type exposes the Guid differently.
    //    //                     return ((InspectionTypeId)value).Value;
    //    //                 }
    //    //             }).Distinct().ToList();

    //    //            inspectionTypesUsedElsewhere.UnionWith(usedInspectionTypeIds);
    //    //        }

    //    //        var orphanedInspectionTypes = typesInInspectionNotificationTypes
    //    //        .Where(cid => !inspectionTypesUsedElsewhere.Contains(cid))
    //    //        .ToList();

    //    //        // Convert your orphaned value objects to their underlying Guid values.
    //    //        var orphanedInspectionTypeGuids = orphanedInspectionTypes.Select(x => x.Value).ToList();

    //    //        // Switch to client evaluation by calling AsEnumerable() so that the .Value accessor is evaluated in C#.
    //    //        var inspectionTypesToRemove = _dbContext.InspectionTypes
    //    //            .AsEnumerable() // This forces the remainder of the query to run in memory.
    //    //            .Where(c => orphanedInspectionTypeGuids.Contains(c.InspectionTypeId.Value))
    //    //            .ToList();

    //    //        _dbContext.InspectionTypes.RemoveRange(inspectionTypesToRemove);

    //    //        _dbContext.SaveChanges();
    //    //        //---------------End Cleanup Contacts and InspectionNotificationContacts---------------------------------





    //    //        //--------------Now We can Update InspectionNotification with new Data----------------------


    //    //        try
    //    //        {


    //    //            Revision Newrevision = new Revision();
    //    //            Revision currentRevision = _dbContext.Revisions.Where(rvs => rvs.RevisionId == inspectionNotification.RevisionId).FirstOrDefault();
    //    //            Newrevision.RequestId = currentRevision.RequestId;
    //    //            Newrevision.VersionNumber = currentRevision.VersionNumber;
    //    //            Newrevision.RevisionId = Guid.NewGuid();
    //    //            Newrevision.RevisionNumber = currentRevision.RevisionNumber + 1;
    //    //            Newrevision.Comment = "Created in doing Changes By vendor";
    //    //            Newrevision.Status = "Changed";
    //    //            //Newrevision.RevisionId = Guid.NewGuid();
    //    //            Newrevision = _dbContext.Revisions.Add(Newrevision).Entity;
    //    //            _dbContext.SaveChanges();

    //    //            var existing = await _dbContext.InspectionNotifications.Include(ins=>ins.Order)
    //    //  .FirstOrDefaultAsync(i =>
    //    //      i.InspectionNotificationId == inspectionNotification.InspectionNotificationId);

    //    //            SyncContacts(existing, inspectionNotification, _dbContext);
    //    //            SyncSubjects(existing, inspectionNotification, _dbContext);
    //    //            SyncTypes(existing, inspectionNotification, _dbContext);
    //    //            SyncLocations(existing, inspectionNotification, _dbContext);

    //    //            existing.OrderName = inspectionNotification.OrderName;                                      
    //    //            existing.IssueDate = DateTime.Now;
    //    //            existing.RequestInspectionDueDate = inspectionNotification.RequestInspectionDueDate;
    //    //            existing.Description = inspectionNotification.Description;
    //    //            existing.EquipmentTagNo = inspectionNotification.EquipmentTagNo;
    //    //            existing.InspectionAction = inspectionNotification.InspectionAction;
    //    //            existing.InspectionActionComment = inspectionNotification.InspectionActionComment;
    //    //            existing.InspectionTimeComment = inspectionNotification.InspectionTimeComment;
    //    //            existing.ModifiedDate = DateTime.Now;
    //    //            existing.RequestInspectionDateStart = inspectionNotification.RequestInspectionDateStart;
    //    //            existing.RequestInspectionDateEnd = inspectionNotification.RequestInspectionDateEnd;
    //    //            existing.RequestInspectionPersianDateEnd = inspectionNotification.RequestInspectionPersianDateEnd;
    //    //            existing.RequestInspectionPersianDateStart = inspectionNotification.RequestInspectionPersianDateStart;
    //    //            existing.RevisionId = Newrevision.RevisionId;
    //    //            existing.RequestInspectionDateStart = inspectionNotification.RequestInspectionDateStart;

    //    //            _dbContext.SaveChangesAsync();

    //    //            return existing;
    //    //        }
    //    //        catch (Exception ex)
    //    //        {

    //    //            return null;
    //    //        }

    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        Console.WriteLine($"Error: {ex.Message}");
    //    //    }
    //    //    //}
    //    //}
    //    //return inspectionNotification;
    //}

    /// <summary>
    /// Syncs the child InspectionNotificationContact collection on the tracked `existing`
    /// entity to match exactly the DTO’s Contacts (by ContactId).
    /// Adds new links, removes old ones, and lets EF handle deletes/inserts.
    /// </summary>
    private static void SyncContacts(
        InspectionNotification existing,
        InspectionNotification dto,
        InspectionDbContext db)
    {
        // remove old
        var incoming = dto.Contacts!.ToList();
        var toRemove = existing.Contacts
            .Where(et => !incoming.Any(i => i.ContactId == et.ContactId))
            .ToList();
        toRemove.ForEach(old => existing.Contacts.Remove(old));

        // add or update
        foreach (var inc in incoming)
        {
            // skip if already linked
            if (existing.Contacts.Any(et => et.ContactId == inc.ContactId))
                continue;

            // does this type exist?
            var contactEntity = db.Contacts
                .FirstOrDefault(t => t.ContactId == inc.ContactId);

            if (contactEntity == null && inc.Contact != null)
            {
                // create the new type
                contactEntity = new Contact
                {
                    ContactId = inc.Contact.ContactId,
                    Name = inc.Contact.Name,
                    Note = inc.Contact.Note,
                    Fax = inc.Contact.Fax,
                    Cc = inc.Contact.Cc,
                    Email = inc.Contact.Email,
                    ContactType = inc.Contact.ContactType,
                    HomePage = inc.Contact.HomePage,
                    Address = inc.Contact.Address,
                    Phone = inc.Contact.Phone,
                    IsVisible = inc.Contact.IsVisible,
                    UserName = inc.Contact.UserName,
                    DomainId = inc.Contact.DomainId,
                    Mobile = inc.Contact.Mobile
                    // …other fields…
                };
                db.Contacts.Add(contactEntity);
                db.SaveChanges();    // flush so we get a valid FK
            }

            if (contactEntity != null)
            {
                // now create the link
                existing.Contacts.Add(new InspectionNotificationContact
                {
                    InspectionNotificationId = existing.InspectionNotificationId,
                    ContactId = contactEntity.ContactId,
                    Contact = null,
                    InspectionNotification=null
                });
            }
            else
            {
                // neither an existing nor a new nav-prop was provided:
                // decide if you want to throw or just skip
            }
        }
    }

    /// <summary>
    /// Syncs the InspectionNotification.Subjects collection so that
    /// existing.Subjects exactly matches dto.Subjects by InspectionSubjectId.
    /// </summary>
    private static void SyncSubjects(
        InspectionNotification existing,
        InspectionNotification dto,
        InspectionDbContext db)
    {
        // remove old
        var incoming = dto.Subjects!.ToList();
        var toRemove = existing.Subjects
            .Where(et => !incoming.Any(i => i.InspectionSubjectId == et.InspectionSubjectId))
            .ToList();
        toRemove.ForEach(old => existing.Subjects.Remove(old));

        // add or update
        foreach (var inc in incoming)
        {
            // skip if already linked
            if (existing.Subjects.Any(et => et.InspectionSubjectId == inc.InspectionSubjectId))
                continue;

            // does this type exist?
            var typeEntity = db.InspectionSubjects
                .FirstOrDefault(t => t.InspectionSubjectId == inc.InspectionSubjectId);

            if (typeEntity == null && inc.InspectionSubject != null)
            {
                // create the new type
                typeEntity = new InspectionSubject
                {
                    InspectionSubjectId = inc.InspectionSubject.InspectionSubjectId,
                    Name = inc.InspectionSubject.Name,

                    // …other fields…
                };
                db.InspectionSubjects.Add(typeEntity);
                db.SaveChanges();    // flush so we get a valid FK
            }

            if (typeEntity != null)
            {
                // now create the link
                existing.Subjects.Add(new InspectionNotificationSubject
                {
                    InspectionNotificationId = existing.InspectionNotificationId,
                    InspectionSubjectId = typeEntity.InspectionSubjectId,
                    InspectionNotification= null,
                    InspectionSubject = null
                });
            }
            else
            {
                // neither an existing nor a new nav-prop was provided:
                // decide if you want to throw or just skip
            }
        }
    }

    /// <summary>
    /// Syncs the InspectionNotification.Types collection so that
    /// existing.Types exactly matches dto.Types by InspectionTypeId.
    /// </summary>
    private static void SyncTypes(
     InspectionNotification existing,
     InspectionNotification dto,
     InspectionDbContext db)
    {
        // remove old
        var incoming = dto.Types!.ToList();
        var toRemove = existing.Types
            .Where(et => !incoming.Any(i => i.InspectionTypeId == et.InspectionTypeId))
            .ToList();
        toRemove.ForEach(old => existing.Types.Remove(old));

        // add or update
        foreach (var inc in incoming)
        {
            // skip if already linked
            if (existing.Types.Any(et => et.InspectionTypeId == inc.InspectionTypeId))
                continue;

            // does this type exist?
            var typeEntity = db.InspectionTypes
                .FirstOrDefault(t => t.InspectionTypeId == inc.InspectionTypeId);

            if (typeEntity == null && inc.InspectionType != null)
            {
                // create the new type
                typeEntity = new InspectionType
                {
                    InspectionTypeId = inc.InspectionType.InspectionTypeId,
                    Name = inc.InspectionType.Name,
                    // …other fields…
                };
                db.InspectionTypes.Add(typeEntity);
                db.SaveChanges();    // flush so we get a valid FK
            }

            if (typeEntity != null)
            {
                // now create the link
                existing.Types.Add(new InspectionNotificationType
                {
                    InspectionNotificationId = existing.InspectionNotificationId,
                    InspectionTypeId = typeEntity.InspectionTypeId,
                    InspectionNotification = null,
                    InspectionType = null
                });
            }
            else
            {
                // neither an existing nor a new nav-prop was provided:
                // decide if you want to throw or just skip
            }
        }
    }



    private static void SyncLocations(
   InspectionNotification existing,
   InspectionNotification dto,
   InspectionDbContext db)
    {
        // remove old
        var incoming = dto.Locations!.ToList();
        var toRemove = existing.Locations
            .Where(et => !incoming.Any(i => i.AddressId == et.AddressId))
            .ToList();
        toRemove.ForEach(old => existing.Locations.Remove(old));

        // add or update
        foreach (var inc in incoming)
        {
            // skip if already linked
            if (existing.Locations.Any(et => et.AddressId == inc.AddressId))
                continue;

            // does this Location exist?
            var locationEntity = db.Addresses
                .FirstOrDefault(t => t.Id == inc.AddressId);

            if (locationEntity == null && inc.Location != null)
            {
                // create the new type
                if (inc.Location.Id != null && inc.Location.Id != Guid.Empty)
                    locationEntity = new Address
                    {
                        Id = inc.Location.Id,
                        City = inc.Location.City,
                        Country = inc.Location.Country,
                        State = inc.Location.State,
                        StreetLine1 = inc.Location.StreetLine1,
                        StreetLine2 = inc.Location.StreetLine2,
                        ZipCode = inc.Location.ZipCode,
                        CreatedDate = DateTime.Now
                    };
                else
                    locationEntity = new Address
                    {
                        Id = Guid.NewGuid(),
                        City = inc.Location.City,
                        Country = inc.Location.Country,
                        State = inc.Location.State,
                        StreetLine1 = inc.Location.StreetLine1,
                        StreetLine2 = inc.Location.StreetLine2,
                        ZipCode = inc.Location.ZipCode,
                        CreatedDate = DateTime.Now
                    };
            
                db.Addresses.Add(locationEntity);
                db.SaveChanges();    // flush so we get a valid FK
            }

            if (locationEntity != null)
            {
                // now create the link
                existing.Locations.Add(new InspectionNotificationLocation
                {
                    InspectionNotificationId = existing.InspectionNotificationId,
                    AddressId = locationEntity.Id,
                    InspectionNotification = null,
                    Location = null
                });
            }
            else
            {
                // neither an existing nor a new nav-prop was provided:
                // decide if you want to throw or just skip
            }
        }
    }


    public async Task<(InspectionNotification, Circulation)> CreateNewInspectionNotification(InspectionNotification rawinspectionNotification, NotificationNumber newNotificationNumber, List<Attachment> attachments)
    {

        InspectionNotification inspectionNotification = null;

        try
        {

            inspectionNotification = InspectionNotification.CreateByUniqueId(rawinspectionNotification);

            if (await _dbContext.Orders.AnyAsync(o =>
            o.EdmsId == _dbContext.Entry(inspectionNotification.Order).Entity.EdmsId))
            {
                var ord = _dbContext.Orders.Where(ord => ord.EdmsId == _dbContext.Entry(inspectionNotification.Order).Entity.EdmsId).FirstOrDefault();
                inspectionNotification.SetOrder(ord!);
                _dbContext.Entry(inspectionNotification.Order!).State = EntityState.Unchanged;
            }
            else
                _dbContext.Entry(inspectionNotification.Order!).State = EntityState.Added;


            inspectionNotification.Contacts!.Clear();

            foreach (InspectionNotificationContact inscontact in rawinspectionNotification.Contacts!.ToList())
            {
                // inscontact.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                var existingContact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.ContactId == inscontact.ContactId);

                Contact contactEntity;

                if (existingContact != null)
                {
                    // If Table3 exists, use it
                    contactEntity = existingContact;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(contactEntity).State = EntityState.Unchanged;

                    var existingInsContact = await _dbContext.InspectionNotificationsContacts.FirstOrDefaultAsync(c => c.ContactId == inscontact.ContactId && c.InspectionNotificationId == inscontact.InspectionNotificationId);

                    inscontact.Contact = null;
                    if (existingInsContact != null)
                        _dbContext.Entry(inscontact).State = EntityState.Unchanged;
                    else
                    {
                        inscontact.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        _dbContext.Entry(inscontact).State = EntityState.Added;
                    }
                    
                }
                else if (inscontact.ContactId != null)
                {
                    // inscontact.InspectionNotificationId = inspectionNotification.Id;
                    InspectionNotificationContact findedContact = rawinspectionNotification.Contacts.Find((cnt) => cnt.ContactId == inscontact.ContactId);

                    if (findedContact != null)
                    {
                        //_dbContext.InspectionSubjects.Add(findedSubject.InspectionSubject);
                        existingContact = _dbContext.Contacts.Add(findedContact.Contact).Entity;
                        inscontact.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        inscontact.ContactId = existingContact.ContactId;
                        inscontact.Contact = null;
                    }
                    _dbContext.Entry(inscontact).State = EntityState.Added;
                }
                else
                {
                    contactEntity = null;
                    inspectionNotification.Contacts!.Remove(inscontact);
                }
            }
       


            foreach (InspectionNotificationSubject insSubject in inspectionNotification.Subjects!.ToList())
            {
                //insSubject.InspectionNotificationId = inspectionNotification.InspectionNotificationId;

                var existingSubject = await _dbContext.InspectionSubjects.FirstOrDefaultAsync(c => c.InspectionSubjectId == insSubject.InspectionSubjectId);


                InspectionSubject subjectEntity;

                if (existingSubject != null)
                {
                    // If Table3 exists, use it
                    subjectEntity = existingSubject;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(subjectEntity).State = EntityState.Unchanged;

                    var existingInsSubject = await _dbContext.InspectionNotificationSubjects.FirstOrDefaultAsync(c => c.InspectionSubjectId == insSubject.InspectionSubjectId && c.InspectionNotificationId == insSubject.InspectionNotificationId);
                    if (existingInsSubject != null)
                        _dbContext.Entry(insSubject).State = EntityState.Unchanged;
                    else
                        _dbContext.Entry(insSubject).State = EntityState.Added;

                }
                else if (insSubject.InspectionSubjectId != null)
                {
                    InspectionNotificationSubject findedSubject = rawinspectionNotification.Subjects.Find((subj) => subj.InspectionSubjectId == insSubject.InspectionSubjectId);

                    if (findedSubject != null)
                    {
                        //_dbContext.InspectionSubjects.Add(findedSubject.InspectionSubject);
                        insSubject.InspectionSubject = findedSubject.InspectionSubject;
                    }
                    // insSubject.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                    _dbContext.Entry(insSubject.InspectionSubject).State = EntityState.Added;
                    _dbContext.Entry(insSubject).State = EntityState.Added;
                }
                else
                {
                    subjectEntity = null;
                    inspectionNotification.Subjects.Remove(insSubject);
                }

            }

            foreach (InspectionNotificationLocation insLocation in inspectionNotification.Locations!.ToList())
            {

                Address existingLocation = null;
                //string country = insLocation.Location!.Country!;
                //string city = insLocation.Location!.City!;
                //string state = insLocation.Location!.State!;
                //string streetLine1 = insLocation.Location!.StreetLine1!;
                //string streetLine2 = insLocation.Location!.StreetLine2!;
                //string zipCode = insLocation.Location!.ZipCode!;

                //var existAddress = await _dbContext.Addresses.AnyAsync(a =>
                //      a.ZipCode == zipCode && a.Country == country && a.City == city && a.State == state
                //      && a.StreetLine1 == streetLine1);
                //if (existAddress)
                //{
                //    existingLocation = _dbContext.Addresses.Where(a =>
                //     a.ZipCode == zipCode && a.Country == country && a.City == city && a.State == state
                //     && a.StreetLine1 == streetLine1).FirstOrDefault();


                //}
                existingLocation = await _dbContext.Addresses.FirstOrDefaultAsync(c => c.Id == insLocation.AddressId);


                Address locationEntity;

                if (existingLocation != null)
                {
                    // If Table3 exists, use it
                    locationEntity = existingLocation;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(locationEntity).State = EntityState.Unchanged;

                    var existingInsLocation = await _dbContext.InspectionNotificationsLocations.FirstOrDefaultAsync(c => c.AddressId == insLocation.AddressId && c.InspectionNotificationId == insLocation.InspectionNotificationId);
                    if (existingInsLocation != null)
                        _dbContext.Entry(insLocation).State = EntityState.Unchanged;
                    else
                        _dbContext.Entry(insLocation).State = EntityState.Added;

                }
                else if (insLocation.AddressId != null)
                {
                    InspectionNotificationLocation findedLocation = rawinspectionNotification.Locations.Find((lct) => lct.AddressId == insLocation.AddressId);

                    if (findedLocation != null)
                    {
                        //_dbContext.InspectionSubjects.Add(findedSubject.InspectionSubject);
                        insLocation.Location = findedLocation.Location;
                    }

                    _dbContext.Entry(insLocation.Location).State = EntityState.Added;
                    _dbContext.Entry(insLocation).State = EntityState.Added;
                }
                else
                {
                    locationEntity = null;
                    inspectionNotification.Locations!.Remove(insLocation);
                }

            }


            foreach (InspectionNotificationType insType in inspectionNotification.Types!.ToList())
            {
                //insType.InspectionNotificationId = inspectionNotification.InspectionNotificationId;

                var existingType = await _dbContext.InspectionTypes.FirstOrDefaultAsync(c => c.InspectionTypeId == insType.InspectionTypeId);


                InspectionType typeEntity;

                if (existingType != null)
                {
                    // If Table3 exists, use it
                    typeEntity = existingType;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(typeEntity).State = EntityState.Unchanged;

                    var existingInsType = await _dbContext.InspectionNotificationsTypes.FirstOrDefaultAsync(c => c.InspectionTypeId == insType.InspectionTypeId && c.InspectionNotificationId == insType.InspectionNotificationId);
                    if (existingInsType != null)
                        _dbContext.Entry(insType).State = EntityState.Unchanged;
                    else
                        _dbContext.Entry(insType).State = EntityState.Added;

                }
                else if (insType.InspectionTypeId != null)
                {
                    InspectionNotificationType findedType = rawinspectionNotification.Types.Find((typ) => typ.InspectionTypeId == insType.InspectionTypeId);

                    if (findedType != null)
                    {
                        //_dbContext.InspectionSubjects.Add(findedSubject.InspectionSubject);
                        insType.InspectionType = findedType.InspectionType;
                    }

                    _dbContext.Entry(insType.InspectionType).State = EntityState.Added;
                    _dbContext.Entry(insType).State = EntityState.Added;
                }
                else
                {
                    typeEntity = null;
                    inspectionNotification.Types!.Remove(insType);
                }

            }


            //var entries = _dbContext.ChangeTracker.Entries().ToList();
            //foreach (var entry in entries)
            //{
            //    Console.WriteLine($"{entry.Entity.GetType().Name} - {entry.State}");
            //}

            //if (inspectionNotification.Revision != null && inspectionNotification.Revision.RevisionId != null)
            //{
            //    //before this time , Request and circulation are created
            //}
            //else
            //{

                ContactType vendorType = ContactType.Vendor;

                // Get int value
                int vendorTypeValue = (int)vendorType;

                Contact creator = rawinspectionNotification.Contacts?.FirstOrDefault(c => c.Contact != null && c.Contact.ContactType == vendorTypeValue)?.Contact;
                string currentDatePersian = "";
                DateTime currentTime = DateTime.Now;

                Domain.Entities.Common.Request newRequest = Domain.Entities.Common.Request.CreateNewRequestWithRevision(RequestTypes.InspectionNotification, creator);

                newRequest.Revisions[0].RevisionNumber = newNotificationNumber.Revision;
                newRequest.Revisions[0].VersionNumber = newNotificationNumber.Version;
                newRequest.Revisions[0].Comment = "First Time Create";
                newRequest.Revisions[0].IsFinal = false;
                newRequest.Revisions[0].SubmittedBy = creator;
                newRequest.Revisions[0].SubmittedDate = DateTime.Now;


            InspectionNotification savedNewInspection = _dbContext.InspectionNotifications.Add(inspectionNotification).Entity;
          

            InspectionNotificationRevision inspectionNotificationRevision = new InspectionNotificationRevision();
            inspectionNotificationRevision.Revision = newRequest.Revisions[0];
            inspectionNotificationRevision.RevisionId = newRequest.Revisions[0].RevisionId;
            inspectionNotificationRevision.InspectionNotificationId = savedNewInspection.InspectionNotificationId;
            //inspectionNotificationRevision.InspectionNotification = savedNewInspection;
            _dbContext.InspectionNotificationRevisions.Add(inspectionNotificationRevision);


                Circulation circulation = Circulation.CreateNew(newRequest.Revisions[0], creator);

                circulation.PerformerContactType = creator.ContactType;
                circulation.CreatedDate = currentTime;
                circulation.PerformedActionDateGregorian = currentTime;
                circulation.PerformedActionDate = "";
                // inspectionNotification.Circulations!.Add(circulation);
                //newRequest.Revisions[0].Circulations!.Add(circulation);
                circulation = _dbContext.Circulations.Add(circulation).Entity;

                newRequest.Revisions[0].CreatedByCirculationId = circulation.Id;

                _dbContext.SaveChanges();


            

            CirculationAttachments addedCA;
                if(circulation.Attachments != null)
                    circulation.Attachments.Clear();

                foreach (Attachment attachment in attachments)//rawinspectionNotification.Attachments!.ToList())
                {

                    // var existingAttachment = await _dbContext.Attachments.FirstOrDefaultAsync(atc => atc.Id.Value == attachment.Id.Value);
                   
                var existingAttachment = await _dbContext.Attachments
                       .FirstOrDefaultAsync(a => a.AttachmentId == attachment.Id && a.DocNo == attachment.DocNo
                       && a.ContentType == attachment.ContentType && a.Description == attachment.Description && a.Name == attachment.Name && a.Status == attachment.Status);


                CirculationAttachments newCirculationAttachment = new CirculationAttachments();
                    newCirculationAttachment.Id = Guid.NewGuid();
                    newCirculationAttachment.CirculationId = circulation.Id;
                    _dbContext.Entry(circulation).State = EntityState.Unchanged;

                    if (existingAttachment != null)
                    {
                        _dbContext.Entry(existingAttachment).State = EntityState.Unchanged;
                        newCirculationAttachment.AttachmentId = existingAttachment.AttachmentId;
                    }
                    else
                    {
                        Attachment newAttachment = Attachment.Create(attachment.Name, attachment.Description, attachment.Content, attachment.Version, "-1", "0", attachment.ContentType,attachment.DocNo,attachment.Status);
                        newAttachment.AttachmentId = attachment.Id;
                        _dbContext.Entry(newAttachment).State = EntityState.Added;
                        newCirculationAttachment.AttachmentId = newAttachment.AttachmentId;
                    }
                    _dbContext.Entry(newCirculationAttachment).State = EntityState.Added;
                   addedCA = _dbContext.CirculationAttachments.Add(newCirculationAttachment).Entity;
                   // circulation.Attachments.Add(addedCA);

                }


                _dbContext.SaveChanges();
                


                return await Task.FromResult((inspectionNotification, circulation));
           // }
        }
        catch (Exception ex)
        {

            string str = ex.Message;
        }


        return (null, null);
    }

  
//public async Task<(InspectionNotification, Circulation)> ModifyInspectionNotification(
//    Circulation currentCirculation,
//    InspectionNotification rawInspectionNotification,
//    List<Attachment> attachments)
//    {
//        try
//        {
//            // ============================================================
//            // 1. Load existing Circulation
//            // ============================================================

//            var dbCirculation = await _dbContext.Circulations
//                .FirstOrDefaultAsync(x => x.Id == currentCirculation.Id);

//            if (dbCirculation == null)
//                throw new Exception(
//                    $"Circulation {currentCirculation.Id} was not found.");


//            // ============================================================
//            // 2. Load existing InspectionNotification
//            // ============================================================

//            var inspectionNotification =
//                await _dbContext.InspectionNotifications
//                    .FirstOrDefaultAsync(x =>
//                        x.InspectionNotificationId ==
//                        rawInspectionNotification.InspectionNotificationId);

//            if (inspectionNotification == null)
//                throw new Exception(
//                    $"InspectionNotification " +
//                    $"{rawInspectionNotification.InspectionNotificationId} " +
//                    $"was not found.");


//            // ============================================================
//            // 3. Update scalar properties of InspectionNotification
//            // ============================================================

//            // Do NOT replace the entity with CreateByUniqueId().
//            // We are modifying the existing database entity.

            
//            inspectionNotification.NotificationNumber =
//                NotificationNumber
//                    .CreateFromString(
//                        rawInspectionNotification.NotificationNumber)
//                    .IncreaseRevision()
//                    .ToString();
            
//            inspectionNotification.EquipmentTagNo =
//                rawInspectionNotification.EquipmentTagNo;            

//            inspectionNotification.InspectionTimeComment =
//                rawInspectionNotification.InspectionTimeComment;

//            inspectionNotification.InspectionAction =
//                rawInspectionNotification.InspectionAction;

//            inspectionNotification.RequestInspectionDueDate =
//                rawInspectionNotification.RequestInspectionDueDate;

//            inspectionNotification.RequestInspectionPersianDateStart =
//                rawInspectionNotification.RequestInspectionPersianDateStart;

//            inspectionNotification.RequestInspectionDateStart =
//                rawInspectionNotification.RequestInspectionDateStart;

//            inspectionNotification.RequestInspectionPersianDateEnd =
//                rawInspectionNotification.RequestInspectionPersianDateEnd;

//            inspectionNotification.RequestInspectionDateEnd =
//                rawInspectionNotification.RequestInspectionDateEnd;

//            inspectionNotification.IssueDate =
//                rawInspectionNotification.IssueDate;

//            inspectionNotification.PersianIssueDate =
//                rawInspectionNotification.PersianIssueDate;


//            // ============================================================
//            // 4. Order
//            // ============================================================

//            if (rawInspectionNotification.Order != null)
//            {
//                var existingOrder = await _dbContext.Orders
//                    .FirstOrDefaultAsync(o =>
//                        o.EdmsId ==
//                        rawInspectionNotification.Order.EdmsId);

//                if (existingOrder != null)
//                {
//                    inspectionNotification.SetOrder(existingOrder);
//                }
//                else
//                {
//                    var newOrder = rawInspectionNotification.Order;

//                    _dbContext.Orders.Add(newOrder);

//                    inspectionNotification.SetOrder(newOrder);
//                }
//            }


//            // ============================================================
//            // 5. Contacts
//            // ============================================================

//            if (rawInspectionNotification.Contacts != null)
//            {
//                foreach (var incomingContactLink
//                         in rawInspectionNotification.Contacts.ToList())
//                {
//                    if (incomingContactLink.Contact == null &&
//                        incomingContactLink.ContactId == null)
//                    {
//                        continue;
//                    }

//                    Contact? contact = null;

//                    // ----------------------------------------------------
//                    // Find existing Contact
//                    // ----------------------------------------------------

//                    if (incomingContactLink.ContactId != null)
//                    {
//                        contact = await _dbContext.Contacts
//                            .FirstOrDefaultAsync(c =>
//                                c.ContactId ==
//                                incomingContactLink.ContactId);
//                    }

//                    // ----------------------------------------------------
//                    // Contact does not exist -> INSERT
//                    // ----------------------------------------------------

//                    if (contact == null)
//                    {
//                        if (incomingContactLink.Contact == null)
//                            continue;

//                        contact = incomingContactLink.Contact;

//                        _dbContext.Contacts.Add(contact);
//                    }
//                    else
//                    {
//                        // ------------------------------------------------
//                        // Contact exists -> UPDATE
//                        // ------------------------------------------------

//                        if (incomingContactLink.Contact != null)
//                        {
//                            contact.Name =
//                                incomingContactLink.Contact.Name;

//                            contact.Mobile =
//                                incomingContactLink.Contact.Mobile;

//                            contact.Phone =
//                                incomingContactLink.Contact.Phone;

//                            contact.ContactType =
//                                incomingContactLink.Contact.ContactType;
//                        }
//                    }

//                    // ----------------------------------------------------
//                    // Add relationship if it doesn't already exist
//                    // ----------------------------------------------------

//                    var relationshipExists =
//                        await _dbContext
//                            .InspectionNotificationsContacts
//                            .AnyAsync(x =>
//                                x.InspectionNotificationId ==
//                                    inspectionNotification
//                                        .InspectionNotificationId &&
//                                x.ContactId ==
//                                    contact.ContactId);

//                    if (!relationshipExists)
//                    {
//                        _dbContext.InspectionNotificationsContacts.Add(
//                            new InspectionNotificationContact
//                            {
//                                InspectionNotificationId =
//                                    inspectionNotification
//                                        .InspectionNotificationId,

//                                ContactId =
//                                    contact.ContactId
//                            });
//                    }
//                }
//            }


//            // ============================================================
//            // 6. Subjects
//            // ============================================================

//            if (rawInspectionNotification.Subjects != null)
//            {
//                foreach (var incomingSubjectLink
//                         in rawInspectionNotification.Subjects.ToList())
//                {
//                    if (incomingSubjectLink.InspectionSubjectId == null &&
//                        incomingSubjectLink.InspectionSubject == null)
//                    {
//                        continue;
//                    }

//                    InspectionSubject? subject = null;

//                    // ----------------------------------------------------
//                    // Find existing Subject
//                    // ----------------------------------------------------

//                    if (incomingSubjectLink.InspectionSubjectId != null)
//                    {
//                        subject = await _dbContext.InspectionSubjects
//                            .FirstOrDefaultAsync(x =>
//                                x.InspectionSubjectId ==
//                                incomingSubjectLink.InspectionSubjectId);
//                    }

//                    // ----------------------------------------------------
//                    // Subject does not exist -> INSERT
//                    // ----------------------------------------------------

//                    if (subject == null)
//                    {
//                        if (incomingSubjectLink.InspectionSubject == null)
//                            continue;

//                        subject = incomingSubjectLink.InspectionSubject;

//                        _dbContext.InspectionSubjects.Add(subject);
//                    }
//                    else
//                    {
//                        // ------------------------------------------------
//                        // Subject exists -> UPDATE
//                        // ------------------------------------------------

//                        if (incomingSubjectLink.InspectionSubject != null)
//                        {
//                            subject.Name =
//                                incomingSubjectLink
//                                    .InspectionSubject.Name;

//                        }
//                    }

//                    // ----------------------------------------------------
//                    // Add relationship if missing
//                    // ----------------------------------------------------

//                    var relationshipExists =
//                        await _dbContext
//                            .InspectionNotificationSubjects
//                            .AnyAsync(x =>
//                                x.InspectionNotificationId ==
//                                    inspectionNotification
//                                        .InspectionNotificationId &&
//                                x.InspectionSubjectId ==
//                                    subject.InspectionSubjectId);

//                    if (!relationshipExists)
//                    {
//                        _dbContext.InspectionNotificationSubjects.Add(
//                            new InspectionNotificationSubject
//                            {
//                                InspectionNotificationId =
//                                    inspectionNotification
//                                        .InspectionNotificationId,

//                                InspectionSubjectId =
//                                    subject.InspectionSubjectId
//                            });
//                    }
//                }
//            }


//            // ============================================================
//            // 7. Locations
//            // ============================================================

//            if (rawInspectionNotification.Locations != null)
//            {
//                foreach (var incomingLocationLink
//                         in rawInspectionNotification.Locations.ToList())
//                {
//                    if (incomingLocationLink.AddressId == null &&
//                        incomingLocationLink.Location == null)
//                    {
//                        continue;
//                    }

//                    Address? address = null;

//                    // ----------------------------------------------------
//                    // Find existing Address
//                    // ----------------------------------------------------

//                    if (incomingLocationLink.AddressId != null)
//                    {
//                        address = await _dbContext.Addresses
//                            .FirstOrDefaultAsync(x =>
//                                x.Id ==
//                                incomingLocationLink.AddressId);
//                    }

//                    // ----------------------------------------------------
//                    // Address does not exist -> INSERT
//                    // ----------------------------------------------------

//                    if (address == null)
//                    {
//                        if (incomingLocationLink.Location == null)
//                            continue;

//                        address = incomingLocationLink.Location;

//                        _dbContext.Addresses.Add(address);
//                    }
//                    else
//                    {
//                        // ------------------------------------------------
//                        // Address exists -> UPDATE
//                        // ------------------------------------------------

//                        if (incomingLocationLink.Location != null)
//                        {
//                            address.Country =
//                                incomingLocationLink.Location.Country;

//                            address.City =
//                                incomingLocationLink.Location.City;

//                            address.State =
//                                incomingLocationLink.Location.State;

//                            address.ZipCode =
//                                incomingLocationLink.Location.ZipCode;

//                            address.StreetLine1 =
//                                incomingLocationLink.Location.StreetLine1;

//                            address.StreetLine2 =
//                                incomingLocationLink.Location.StreetLine2;
//                        }
//                    }

//                    // ----------------------------------------------------
//                    // Add relationship if missing
//                    // ----------------------------------------------------

//                    var relationshipExists =
//                        await _dbContext
//                            .InspectionNotificationsLocations
//                            .AnyAsync(x =>
//                                x.InspectionNotificationId ==
//                                    inspectionNotification
//                                        .InspectionNotificationId &&
//                                x.AddressId ==
//                                    address.Id);

//                    if (!relationshipExists)
//                    {
//                        _dbContext.InspectionNotificationsLocations.Add(
//                            new InspectionNotificationLocation
//                            {
//                                InspectionNotificationId =
//                                    inspectionNotification
//                                        .InspectionNotificationId,

//                                AddressId =
//                                    address.Id
//                            });
//                    }
//                }
//            }


//            // ============================================================
//            // 8. Inspection Types
//            // ============================================================

//            if (rawInspectionNotification.Types != null)
//            {
//                foreach (var incomingTypeLink
//                         in rawInspectionNotification.Types.ToList())
//                {
//                    if (incomingTypeLink.InspectionTypeId == null &&
//                        incomingTypeLink.InspectionType == null)
//                    {
//                        continue;
//                    }

//                    InspectionType? type = null;

//                    // ----------------------------------------------------
//                    // Find existing Type
//                    // ----------------------------------------------------

//                    if (incomingTypeLink.InspectionTypeId != null)
//                    {
//                        type = await _dbContext.InspectionTypes
//                            .FirstOrDefaultAsync(x =>
//                                x.InspectionTypeId ==
//                                incomingTypeLink.InspectionTypeId);
//                    }

//                    // ----------------------------------------------------
//                    // Type does not exist -> INSERT
//                    // ----------------------------------------------------

//                    if (type == null)
//                    {
//                        if (incomingTypeLink.InspectionType == null)
//                            continue;

//                        type = incomingTypeLink.InspectionType;

//                        _dbContext.InspectionTypes.Add(type);
//                    }
//                    else
//                    {
//                        // ------------------------------------------------
//                        // Type exists -> UPDATE
//                        // ------------------------------------------------

//                        if (incomingTypeLink.InspectionType != null)
//                        {
//                            type.Name =
//                                incomingTypeLink.InspectionType.Name;
//                        }
//                    }

//                    // ----------------------------------------------------
//                    // Add relationship if missing
//                    // ----------------------------------------------------

//                    var relationshipExists =
//                        await _dbContext
//                            .InspectionNotificationsTypes
//                            .AnyAsync(x =>
//                                x.InspectionNotificationId ==
//                                    inspectionNotification
//                                        .InspectionNotificationId &&
//                                x.InspectionTypeId ==
//                                    type.InspectionTypeId);

//                    if (!relationshipExists)
//                    {
//                        _dbContext.InspectionNotificationsTypes.Add(
//                            new InspectionNotificationType
//                            {
//                                InspectionNotificationId =
//                                    inspectionNotification
//                                        .InspectionNotificationId,

//                                InspectionTypeId =
//                                    type.InspectionTypeId
//                            });
//                    }
//                }
//            }


//            // ============================================================
//            // 9. Save notification + side objects
//            // ============================================================

//            await _dbContext.SaveChangesAsync();


//            // ============================================================
//            // 10. Create Revision
//            // ============================================================

//            var currentRevision =
//                await _dbContext.Revisions
//                    .FirstOrDefaultAsync(x =>
//                        x.RevisionId == dbCirculation.RevisionId);

//            if (currentRevision == null)
//                throw new Exception(
//                    $"Revision {dbCirculation.RevisionId} was not found.");

//            var newRevision = new Revision
//            {
//                RequestId = currentRevision.RequestId,

//                VersionNumber = currentRevision.VersionNumber,

//                RevisionId = Guid.NewGuid(),

//                RevisionNumber =
//                    currentRevision.RevisionNumber + 1,

//                Comment =
//                    "Created in Modify process by Vendor",

//                Status =
//                    "ModifiedByVendor",

//                SubmittedDate =
//                    DateTime.Now,

//                CreatedByCirculationId =
//                    dbCirculation.Id
//            };

//            _dbContext.Revisions.Add(newRevision);


//            // ============================================================
//            // 11. Connect Notification to Revision
//            // ============================================================

//            var notificationRevision =
//                new InspectionNotificationRevision
//                {
//                    InspectionNotificationId =
//                        inspectionNotification
//                            .InspectionNotificationId,

//                    RevisionId =
//                        newRevision.RevisionId,

//                    Revision =
//                        newRevision
//                };

//            _dbContext.InspectionNotificationRevisions.Add(
//                notificationRevision);


//            // ============================================================
//            // 12. Update Circulation
//            // ============================================================

//            dbCirculation.PerformedActionDateGregorian =
//                DateTime.Now;

//            dbCirculation.PerformedActionDate =
//                GetCurrentDate();

//            dbCirculation.ActionType = 1;


//            // ============================================================
//            // 13. Attachments
//            // ============================================================

//            if (attachments != null)
//            {
//                foreach (var attachment in attachments)
//                {
//                    if (attachment == null)
//                        continue;

//                    Attachment? existingAttachment =
//                        null;

//                    if (attachment.Id != null)
//                    {
//                        existingAttachment =
//                            await _dbContext.Attachments
//                                .FirstOrDefaultAsync(a =>
//                                    a.AttachmentId == attachment.Id);
//                    }

//                    // ----------------------------------------------------
//                    // Existing attachment
//                    // ----------------------------------------------------

//                    AttachmentId attachmentId;

//                    if (existingAttachment != null)
//                    {
//                        attachmentId =
//                            existingAttachment.AttachmentId;
//                    }
//                    else
//                    {
//                        // ------------------------------------------------
//                        // New attachment
//                        // ------------------------------------------------

//                        var newAttachment =
//                            Attachment.Create(
//                                attachment.Name,
//                                attachment.Description,
//                                attachment.Content,
//                                attachment.Version,
//                                "-1",
//                                "0",
//                                attachment.ContentType,
//                                attachment.DocNo,
//                                attachment.Status);

//                        newAttachment.AttachmentId =
//                            AttachmentId.CreateUnique();

//                        _dbContext.Attachments.Add(
//                            newAttachment);

//                        attachmentId =
//                            newAttachment.AttachmentId;
//                    }

//                    // ----------------------------------------------------
//                    // Check whether attachment is already connected
//                    // ----------------------------------------------------

//                    var relationshipExists =
//                        await _dbContext.CirculationAttachments
//                            .AnyAsync(x =>
//                                x.CirculationId ==
//                                    dbCirculation.Id &&
//                                x.AttachmentId ==
//                                    attachmentId);

//                    if (!relationshipExists)
//                    {
//                        _dbContext.CirculationAttachments.Add(
//                            new CirculationAttachments
//                            {
//                                Id = Guid.NewGuid(),

//                                CirculationId =
//                                    dbCirculation.Id,

//                                AttachmentId =
//                                    attachmentId
//                            });
//                    }
//                }
//            }


//            // ============================================================
//            // 14. Final save
//            // ============================================================

//            await _dbContext.SaveChangesAsync();


//            return (
//                inspectionNotification,
//                dbCirculation
//            );
//        }
//        catch (Exception ex)
//        {
//            // IMPORTANT:
//            // Do not silently swallow the exception.
//            // Log it and rethrow it so the caller gets the error.

//            // _logger.LogError(
//            //     ex,
//            //     "Error modifying InspectionNotification {Id}",
//            //     rawInspectionNotification.InspectionNotificationId);

//            return (null, null);
//        }
        
//    }

  
    public string GetCurrentDate()
    {
        PersianDate pDate = PersianCelendar.GregorianToJalali(DateTime.Now);

        string dpCurrentDate = JsonConvert.SerializeObject(pDate.Year.ToString() + "/" + pDate.Month.ToString() + "/" + pDate.Day.ToString(), Formatting.Indented);
        return dpCurrentDate;
    }

    public async Task<(InspectionNotification, Circulation)> ModifyInspectionNotification(Circulation currentCirculation, InspectionNotification rawinspectionNotification, List<Attachment> attachments)
    {

        InspectionNotification inspectionNotification = null;

        try
        {

            inspectionNotification = InspectionNotification.CreateByUniqueId(rawinspectionNotification);//rawinspectionNotification.DeepClone();

            if (await _dbContext.Orders.AnyAsync(o =>
            o.EdmsId == _dbContext.Entry(inspectionNotification.Order).Entity.EdmsId))
            {
                var ord = _dbContext.Orders.Where(ord => ord.EdmsId == _dbContext.Entry(inspectionNotification.Order).Entity.EdmsId).FirstOrDefault();
                inspectionNotification.SetOrder(ord!);
                _dbContext.Entry(inspectionNotification.Order!).State = EntityState.Unchanged;
            }
            else
                _dbContext.Entry(inspectionNotification.Order!).State = EntityState.Added;


            inspectionNotification.Contacts!.Clear();

            foreach (InspectionNotificationContact inscontact in rawinspectionNotification.Contacts!.ToList())
            {
                // inscontact.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                var existingContact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.ContactId == inscontact.ContactId && c.ContactType == inscontact.Contact.ContactType
                && c.Mobile == inscontact.Contact.Mobile && c.Phone == inscontact.Contact.Phone && c.Name == inscontact.Contact.Name);

                Contact contactEntity;

                if (existingContact != null)
                {
                    // If Table3 exists, use it
                    contactEntity = existingContact;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(contactEntity).State = EntityState.Unchanged;

                    var existingInsContact = await _dbContext.InspectionNotificationsContacts.FirstOrDefaultAsync(c => c.ContactId == inscontact.ContactId
                    && c.InspectionNotificationId == inspectionNotification.InspectionNotificationId);

                    inscontact.Contact = null;
                    if (existingInsContact != null)
                        _dbContext.Entry(inscontact).State = EntityState.Unchanged;
                    else
                    {
                        inscontact.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        _dbContext.Entry(inscontact).State = EntityState.Added;
                    }

                }
                else if (inscontact.ContactId != null)
                {
                    // inscontact.InspectionNotificationId = inspectionNotification.Id;
                    InspectionNotificationContact findedContact = rawinspectionNotification.Contacts.Find((cnt) => cnt.ContactId == inscontact.ContactId);

                    if (findedContact != null)
                    {
                        //_dbContext.InspectionSubjects.Add(findedSubject.InspectionSubject);
                        existingContact = _dbContext.Contacts.Add(findedContact.Contact).Entity;
                        inscontact.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        inscontact.ContactId = existingContact.ContactId;
                        inscontact.Contact = null;
                    }
                    _dbContext.Entry(inscontact).State = EntityState.Added;
                }
                else
                {
                    contactEntity = null;
                    inspectionNotification.Contacts!.Remove(inscontact);
                }
            }


            inspectionNotification.Subjects!.Clear();
            foreach (InspectionNotificationSubject insSubject in rawinspectionNotification.Subjects!.ToList())
            {
                var existingSubject = await _dbContext.InspectionSubjects.FirstOrDefaultAsync(c => c.InspectionSubjectId == insSubject.InspectionSubjectId);


                InspectionSubject subjectEntity;

                if (existingSubject != null)
                {
                    // If Table3 exists, use it
                    subjectEntity = existingSubject;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(subjectEntity).State = EntityState.Unchanged;

                    var existingInsSubject = await _dbContext.InspectionNotificationSubjects.
                        FirstOrDefaultAsync(c => c.InspectionSubjectId == insSubject.InspectionSubjectId
                        && c.InspectionNotificationId == inspectionNotification.InspectionNotificationId);

                    insSubject.InspectionSubject = null;
                    if (existingInsSubject != null)
                        _dbContext.Entry(insSubject).State = EntityState.Unchanged;
                    else
                    {
                        insSubject.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        _dbContext.Entry(insSubject).State = EntityState.Added;
                    }

                }
                else if (insSubject.InspectionSubjectId != null)
                {
                    InspectionNotificationSubject findedSubject = rawinspectionNotification.Subjects.Find((subj) => subj.InspectionSubjectId == insSubject.InspectionSubjectId);

                    if (findedSubject != null)
                    {
                        existingSubject = _dbContext.InspectionSubjects.Add(findedSubject.InspectionSubject).Entity;
                        insSubject.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        insSubject.InspectionSubjectId = existingSubject.InspectionSubjectId;
                        insSubject.InspectionSubject = null;
                        //insSubject.InspectionSubject = findedSubject.InspectionSubject;
                    }
                    // insSubject.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                    //_dbContext.Entry(insSubject.InspectionSubject).State = EntityState.Added;
                    _dbContext.Entry(insSubject).State = EntityState.Added;
                }
                else
                {
                    subjectEntity = null;
                    inspectionNotification.Subjects.Remove(insSubject);
                }

            }

            inspectionNotification.Locations!.Clear();
            foreach (InspectionNotificationLocation insLocation in rawinspectionNotification.Locations!.ToList())
            {

                Address existingLocation = null;

                existingLocation = await _dbContext.Addresses.FirstOrDefaultAsync(c => c.Id == insLocation.AddressId && c.Country == insLocation.Location.Country
                && c.City == insLocation.Location.City && c.State == insLocation.Location.State &&
                c.ZipCode == insLocation.Location.ZipCode && c.StreetLine1 == insLocation.Location.StreetLine1
                 && c.StreetLine2 == insLocation.Location.StreetLine2);


                Address locationEntity;

                if (existingLocation != null)
                {
                    // If Table3 exists, use it
                    locationEntity = existingLocation;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(locationEntity).State = EntityState.Unchanged;

                    var existingInsLocation = await _dbContext.InspectionNotificationsLocations.
                        FirstOrDefaultAsync(c => c.AddressId == insLocation.AddressId && c.InspectionNotificationId == inspectionNotification.InspectionNotificationId);
                    insLocation.Location = null;

                    if (existingInsLocation != null)
                        _dbContext.Entry(insLocation).State = EntityState.Unchanged;
                    else
                    {
                        insLocation.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        _dbContext.Entry(insLocation).State = EntityState.Added;
                    }
                }
                else if (insLocation.AddressId != null)
                {
                    InspectionNotificationLocation findedLocation = rawinspectionNotification.Locations.Find((lct) => lct.AddressId == insLocation.AddressId);

                    if (findedLocation != null)
                    {
                        existingLocation = _dbContext.Addresses.Add(findedLocation.Location).Entity;
                        insLocation.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        insLocation.AddressId = existingLocation.Id;
                        insLocation.Location = null;
                        //insLocation.Location = findedLocation.Location;
                    }

                    //_dbContext.Entry(insLocation.Location).State = EntityState.Added;
                    _dbContext.Entry(insLocation).State = EntityState.Added;
                }
                else
                {
                    locationEntity = null;
                    inspectionNotification.Locations!.Remove(insLocation);
                }

            }

            inspectionNotification.Types!.Clear();
            foreach (InspectionNotificationType insType in rawinspectionNotification.Types!.ToList())
            {
                var existingType = await _dbContext.InspectionTypes.FirstOrDefaultAsync(c => c.InspectionTypeId == insType.InspectionTypeId);


                InspectionType typeEntity;

                if (existingType != null)
                {
                    // If Table3 exists, use it
                    typeEntity = existingType;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(typeEntity).State = EntityState.Unchanged;

                    var existingInsType = await _dbContext.InspectionNotificationsTypes.
                        FirstOrDefaultAsync(c => c.InspectionTypeId == insType.InspectionTypeId && c.InspectionNotificationId == inspectionNotification.InspectionNotificationId);

                    insType.InspectionType = null;
                    if (existingInsType != null)
                        _dbContext.Entry(insType).State = EntityState.Unchanged;
                    else
                    {
                        insType.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        _dbContext.Entry(insType).State = EntityState.Added;
                    }
                }
                else if (insType.InspectionTypeId != null)
                {
                    InspectionNotificationType findedType = rawinspectionNotification.Types.Find((typ) => typ.InspectionTypeId == insType.InspectionTypeId);

                    if (findedType != null)
                    {
                        existingType = _dbContext.InspectionTypes.Add(findedType.InspectionType).Entity;
                        insType.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                        insType.InspectionTypeId = existingType.InspectionTypeId;
                        insType.InspectionType = null;
                        //insType.InspectionType = findedType.InspectionType;
                    }

                    // _dbContext.Entry(insType.InspectionType).State = EntityState.Added;
                    _dbContext.Entry(insType).State = EntityState.Added;
                }
                else
                {
                    typeEntity = null;
                    inspectionNotification.Types!.Remove(insType);
                }

            }
            ContactType vendorType = ContactType.Vendor;

            // Get int value
            int vendorTypeValue = (int)vendorType;

            // Contact creator = rawinspectionNotification.Contacts?.FirstOrDefault(c => c.Contact != null && c.Contact.ContactType == vendorTypeValue)?.Contact;
            string currentDatePersian = "";
            DateTime currentTime = DateTime.Now;

            inspectionNotification.NotificationNumber = NotificationNumber.CreateFromString(rawinspectionNotification.NotificationNumber).IncreaseRevision().ToString();

            Circulation dbCirculation = _dbContext.Circulations.Where(cr => cr.Id == currentCirculation.Id).FirstOrDefault();

            Revision Newrevision = new Revision();
            Revision currentRevision = _dbContext.Revisions.Where(rvs => rvs.RevisionId == dbCirculation.RevisionId).FirstOrDefault();
            Newrevision.RequestId = currentRevision.RequestId;
            Newrevision.VersionNumber = currentRevision.VersionNumber;
            Newrevision.RevisionId = Guid.NewGuid();
            Newrevision.RevisionNumber = currentRevision.RevisionNumber + 1;
            Newrevision.Comment = "Created in Modify process by Vendor";
            Newrevision.Status = "ModifiedByVendor";
            Newrevision.SubmittedDate = DateTime.Now;
            Newrevision.CreatedByCirculationId = dbCirculation.Id;
            Newrevision = _dbContext.Revisions.Add(Newrevision).Entity;

            // _dbContext.SaveChanges();

            InspectionNotification savedModifiedInspection = _dbContext.InspectionNotifications.Add(inspectionNotification).Entity;

            _dbContext.SaveChanges();
            InspectionNotificationRevision inspectionNotificationRevision = new InspectionNotificationRevision();
            //   inspectionNotificationRevision.InspectionNotification = savedModifiedInspection;
            inspectionNotificationRevision.InspectionNotificationId = savedModifiedInspection.InspectionNotificationId;
            inspectionNotificationRevision.RevisionId = Newrevision.RevisionId;
            inspectionNotificationRevision.Revision = Newrevision;

            _dbContext.InspectionNotificationRevisions.Add(inspectionNotificationRevision);



            dbCirculation.PerformedActionDateGregorian = currentTime;
            dbCirculation.PerformedActionDate = GetCurrentDate();
            dbCirculation.ActionType = 1;


            _dbContext.SaveChanges();

            foreach (var attachment in attachments)
            {
                if (attachment == null) continue;

                var existingAttachment = await _dbContext.Attachments
                    .FirstOrDefaultAsync(a => a.AttachmentId == attachment.Id && a.DocNo == attachment.DocNo
                    && a.ContentType == attachment.ContentType && a.Description == attachment.Description && a.Name == attachment.Name && a.Status == attachment.Status);

                var newCirculationAttachment = new CirculationAttachments
                {
                    Id = Guid.NewGuid(),
                    CirculationId = dbCirculation.Id
                };

                if (existingAttachment != null)
                {
                    newCirculationAttachment.AttachmentId = existingAttachment.AttachmentId;

                }
                else
                {
                    var newAttachment = Attachment.Create(
                        attachment.Name,
                        attachment.Description,
                        attachment.Content,
                        attachment.Version,
                        "-1",
                        "0",
                        attachment.ContentType,
                        attachment.DocNo,
                        attachment.Status);

                    newAttachment.AttachmentId = AttachmentId.CreateUnique();//attachment.Id;
                    _dbContext.Attachments.Add(newAttachment);

                    newCirculationAttachment.AttachmentId = newAttachment.AttachmentId;
                }

                _dbContext.CirculationAttachments.Add(newCirculationAttachment);
            }

            _dbContext.SaveChanges();

            return await Task.FromResult((inspectionNotification, currentCirculation));

        }
        catch (Exception ex)
        {

            string str = ex.Message;
        }


        return (null, null);
    }



    public Task<bool> SaveInspectionNotificationCirculations(List<Circulation> circulations)
    {
        try
        {
            _dbContext.Circulations.AddRange(circulations);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            return Task.FromResult(false);
        }
    }

    //public InspectionNotification SimulateInpectionDataForNextUsers(InspectionNotification inspectionNotification,Project project)
    //{
    //    string projectId =  project.FSId;
    //    string orderDotId = inspectionNotification.Order.EdmsId;



    //    try
    //    {
    //        var newInstance = inspectionNotification.DeepClone();


    //        Inspection.Domain.Entities.Common.Request mainRequest = inspectionNotification.Request;

    //        List<InspectionNotificationContact> PidecContacts = inspectionNotification.Contacts!.Where((contact) => contact.Contact.ContactType == ContactType.PIDEC_TO.GetHashCode() || contact.Contact.ContactType == ContactType.PIDEC_CC.GetHashCode()).ToList();

    //        Contact creator = inspectionNotification.Contacts!.Where((contact) => contact.Contact.ContactType == ContactType.Vendor.GetHashCode()).FirstOrDefault()!.Contact;


    //        string currentDatePersian = "";
    //        DateTime currentTime = DateTime.Now;

    //        if (PidecContacts.Count > 0)
    //        {


    //            PidecContacts.ForEach((pContact) =>
    //            {                    
    //                Circulation nextCirculation = Circulation.CreateForNextContact(mainRequest, pContact.Contact);

    //                newInstance.Circulations.Add(nextCirculation);
    //            });

    //        }

    //        var departmentLeads = _eDMSService.GetOrderDepartmentLeads(orderDotId, projectId);
    //        List<Contact> departmentLeadsContacts = GetDepartmentLeadContacts(departmentLeads);

    //        if (departmentLeadsContacts.Count > 0)
    //        {


    //            departmentLeadsContacts.ForEach((dContact) =>
    //            {                   
    //                Circulation nextCirculation = Circulation.CreateForNextContact(mainRequest, dContact);

    //                newInstance.Circulations.Add(nextCirculation);
    //            });

    //        }

    //        _dbContext.InspectionNotifications.Add(newInstance);
    //        _dbContext.SaveChanges();

    //        return newInstance;
    //    }
    //    catch (Exception exp)
    //    {         
    //        return null;
    //    }        
    //}

    private List<Contact> GetDepartmentLeadContacts(Task<EDMSOrderDepartmentLeads> departmentLeads)
    {
        List<Contact> PidecDepartmentContacts = new List<Contact>();
        SystematicUser originatorUser = new SystematicUser();
        SystematicUser DepartmentleadUser = new SystematicUser();
        EDMSOrderDepartmentLeads eDMSOrderDepartmentLeads = departmentLeads.Result;

        List<SystematicUser> usersLikeOriginator;
        List<SystematicUser> usersLikeDeptLead;


        string DeptLeadResetPasswordLink = "";
        string DeptOriginatorResetPasswordLink = "";

        SystematicRole LeadDepartmentRole;
        SystematicRole originatorRole;

        Contact LeadDepartmentContact;
        Contact OriginatorContact;

        List<string> RoleNames = new List<string>();
        try
        {
            usersLikeOriginator = _dbContext.Users.Where(usr => usr.UserName == eDMSOrderDepartmentLeads.originator && usr.DomainId == 0 && usr.IsActive == true).ToList();
            usersLikeDeptLead = _dbContext.Users.Where(usr => usr.UserName == eDMSOrderDepartmentLeads.leadEngineer && usr.DomainId == 0 && usr.IsActive == true).ToList();


            // LeadDepartmentRole = _userManagerRepository.AddNewRole(eDMSOrderDepartmentLeads.deptCode + "_Lead", UserType.PIDECDepartmentLead.GetHashCode().ToString()).Result;//_dbContext.Roles.Add(LeadDepartmentRole).Entity;

            //originatorRole = _userManagerRepository.AddNewRole(eDMSOrderDepartmentLeads.deptCode + "_Originator", UserType.PIDECDepartmentOriginator.GetHashCode().ToString()).Result;//_dbContext.Roles.Add(originatorRole).Entity;
            LeadDepartmentRole = _userManagerRepository.AddNewRole("PIDECDepartment_Lead", UserType.PIDECDepartmentLead.GetHashCode().ToString()).Result;//_dbContext.Roles.Add(LeadDepartmentRole).Entity;

            originatorRole = _userManagerRepository.AddNewRole("PIDECDepartment_Originator", UserType.PIDECDepartmentOriginator.GetHashCode().ToString()).Result;//_dbContext.Roles.Add(originatorRole).Entity;


            if (usersLikeDeptLead.Count == 0)
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
                LeadDepartmentContact.ContactId = ContactId.CreateUnique();
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
                (DepartmentleadUser, DeptLeadResetPasswordLink) = _userManagerRepository.RegisterNewUser(DepartmentleadUser, RoleNames).Result;

                LeadDepartmentContact.UserId = DepartmentleadUser.Id;

                PidecDepartmentContacts.Add(LeadDepartmentContact);
                _dbContext.Contacts.Add(LeadDepartmentContact);

                _userManagerRepository.AddUserToRole(DepartmentleadUser, "PIDECDepartment_Lead");
            }
            else
            {
                var existedPidecLead = _dbContext.Contacts.Where(ct => ct.UserId == usersLikeDeptLead[0].Id).FirstOrDefault();
                PidecDepartmentContacts.Add(existedPidecLead);

            }

            if (usersLikeOriginator.Count == 0)
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
                OriginatorContact.ContactId = ContactId.CreateUnique();
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

                (originatorUser, DeptOriginatorResetPasswordLink) = _userManagerRepository.RegisterNewUser(originatorUser, RoleNames).Result;
                OriginatorContact.UserId = originatorUser.Id;

                _dbContext.Contacts.Add(OriginatorContact);
                PidecDepartmentContacts.Add(OriginatorContact);
                _userManagerRepository.AddUserToRole(DepartmentleadUser, "PIDECDepartment_Originator");
            }
            else
            {
                var existedPidecOriginator = _dbContext.Contacts.Where(ct => ct.UserId == usersLikeOriginator[0].Id).FirstOrDefault();
                PidecDepartmentContacts.Add(existedPidecOriginator);
            }
        }
        catch (Exception exp)
        {
            return null;
        }
        return PidecDepartmentContacts;
    }

    //public async Task<InspectionNotification> SendNewInspectionNotification(InspectionNotification inspectionNotification,Project project)
    //{
    //    InspectionNotification notificationForSend = null;

    //    string  newRevisedNumber = GenerateNewNotificationNumber(project.Number, inspectionNotification.Order.Name).Result.ToString();
    //    inspectionNotification.NotificationNumber = newRevisedNumber;      

    //    try
    //        {

    //            if (inspectionNotification.InspectionNotificationId != null && !inspectionNotification.InspectionNotificationId.Value.Equals(Guid.Empty))
    //            {
    //                notificationForSend = UpdateInspectionNotification(inspectionNotification).Result;                 
    //            }
    //            else
    //                notificationForSend = CreateNewInspectionNotification(inspectionNotification).Result;

    //       // var simulatedVersion = SimulateInpectionDataForNextUsers(notificationForSend,project);

    //        await UpdateNotificationNumber(NotificationNumber.CreateFromString(notificationForSend!.NotificationNumber!));
    //    }
    //        catch (Exception exp)
    //        {

    //        }         

    //    return await Task.FromResult(inspectionNotification);     
    //}

    public Task<(List<InspectionNotification>, List<InspectionReport>)> GetInspectionNotificationByRequestId(Guid requestId, string usrId)
    {

        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotification> inspectionNotifications;
            List<InspectionNotificationContact>? contacts;
            List<Order> orders;
            List<InspectionType> inspectionTypes;
            List<InspectionNotificationSubject> subjects;
            List<InspectionNotificationLocation> locations;
            List<InspectionNotificationType> types;
            List<Circulation> participatedCirculations;
            List<InspectionReport> relatedInspectionReports;
            List<InspectionNotificationRevision> revisionsRelatedToInsNotification;

            var circulationDict = new Dictionary<Guid, Circulation>();

            using (var multi = connection.QueryMultiple(
            "GetInspectionNotification_requestId_userId0",
            new { RequestId = requestId, userId = usrId },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {
            
                // Read first result set (main data)
                var notificationLookup = new Dictionary<Guid, InspectionNotification>();
                var revisionLookup = new Dictionary<Guid, Revision>();
                var circulationLookup = new Dictionary<Guid, Circulation>();
                var orderLookup = new Dictionary<Guid, Order>();

                inspectionNotifications = multi.Read<Circulation, Contact, Request, Revision, InspectionNotification, InspectionNotificationRevision, InspectionNotification>(
                    (cr, ct, rq, rvs, ins,insRe) =>
                    {
                        // 1. Get or create InspectionNotification
                        if (!notificationLookup.TryGetValue(ins.InspectionNotificationId, out var notification))
                        {
                            notification = ins;
                            notification.Revisions = new List<InspectionNotificationRevision>();
                            notificationLookup.Add(notification.InspectionNotificationId, notification);
                        }

                        // 2. Get or create Revision (attach Request)
                        if (!revisionLookup.TryGetValue(rvs.RevisionId, out var revision))
                        {
                            revision = rvs;
                            revision.Request = rq;
                            revision.InspectionNotifications = new List<InspectionNotificationRevision>();
                            revision.Circulations = new List<Circulation>();
                            revisionLookup.Add(revision.RevisionId, revision);
                        }

                        // 3. Handle InspectionNotificationRevision link
                        if (insRe != null && !notification.Revisions.Any(x => x.Id == insRe.Id))
                        {
                            insRe.InspectionNotification = notification;
                            insRe.Revision = revision;
                            notification.Revisions.Add(insRe);
                            revision.InspectionNotifications.Add(insRe);
                        }

                        // 4. Get or create Circulation (attach Contact)
                        if (!circulationLookup.TryGetValue(cr.Id, out var circulation))
                        {
                            circulation = cr;
                            circulation.PerformerContactPerson = ct;
                            circulation.Attachments = new List<CirculationAttachments>(); // will be filled later
                            circulationLookup.Add(cr.Id, circulation);
                            revision.Circulations.Add(circulation);
                        }
                        return notification;
                    },
                    splitOn: "ContactId,RequestId,RevisionId,InspectionNotificationId,Id").Distinct().ToList();

                // Read second result set (attachments)
                List<CirculationAttachments> crattachments = multi.Read<CirculationAttachments, Attachment, CirculationAttachments>(
                    (cra, atc) =>
                    {
                        cra.Attachment = atc;
                        return cra;
                    },
                    splitOn: "AttachmentId")
                    .ToList();

                // Now attach attachments to circulations
                foreach (var cra in crattachments)
                {
                    if (circulationLookup.TryGetValue(cra.CirculationId.Value, out var circ))
                    {
                        circ.Attachments.Add(cra);
                    }
                }

                orders = multi.Read<Order,Project,OrderInspectionSubject,InspectionSubject,Order>(
                     (ord, prj,ordIns,inss) =>
                     {

                         if (orderLookup.TryGetValue(ord.OrderId.Value, out var findedorder))
                         {
                             ordIns.InspectionSubject = inss;
                             findedorder.Subjects.Add(ordIns);                             
                             return findedorder;
                         }
                         else
                         {
                             ordIns.InspectionSubject = inss;
                             ord.Subjects.Add(ordIns);
                             ord.Project = prj;
                             orderLookup.Add(ord.OrderId.Value, ord);
                             return ord;
                         }
                         
                     },
                     splitOn: "ProjectId,OrderInspectionSubjectId,InspectionSubjectId"
                    ).ToList();

                contacts = multi.Read<InspectionNotificationContact, Contact, InspectionNotificationContact>(
                   (insc, cnt) => { insc.Contact = cnt; return insc; },
                   splitOn: "ContactId"
               ).ToList();

             
                foreach (var insc in contacts)
                {
                    if (notificationLookup.TryGetValue(insc.InspectionNotificationId.Value, out var ins))
                    {
                        ins.Contacts.Add(insc);                    
                    }
                }

                foreach (var ins in notificationLookup)
                {
                    ins.Value.SetOrder(orders[0]);
                }

                subjects = multi.Read<InspectionNotificationSubject, InspectionSubject, InspectionNotificationSubject>(
                  (inss, ins) => { inss.InspectionSubject = ins; return inss; },
                  splitOn: "InspectionSubjectId"
              ).ToList();


                foreach (var inss in subjects)
                {
                    if (notificationLookup.TryGetValue(inss.InspectionNotificationId.Value, out var ins))
                    {
                        ins.Subjects.Add(inss);
                    }
                }

                locations = multi.Read<InspectionNotificationLocation, Address, InspectionNotificationLocation>(
                  (insl, adr) => { insl.Location = adr; return insl; },
                  splitOn: "AddressId"
                ).ToList();
                

                foreach (var insl in locations)
                {
                    if (notificationLookup.TryGetValue(insl.InspectionNotificationId.Value, out var ins))
                    {
                        ins.Locations.Add(insl);
                    }
                }

                types = multi.Read<InspectionNotificationType, InspectionType, InspectionNotificationType>(
                  (inst, ins) => { inst.InspectionType = ins; return inst; },
                  splitOn: "InspectionTypeId"
              ).ToList();


                foreach (var inst in types)
                {
                    if (notificationLookup.TryGetValue(inst.InspectionNotificationId.Value, out var ins))
                    {
                        ins.Types.Add(inst);
                    }
                }

                // Result is notificationLookup.Values
                inspectionNotifications = notificationLookup.Values.ToList();

            

                relatedInspectionReports = multi.Read<InspectionReport, Revision, InspectionReport>(
                    (insr, rvs) => { insr.Revision = rvs; return insr; },
                    splitOn: "RevisionId").ToList();


              
            }

          
            List<InspectionNotification> result = inspectionNotifications.Count > 0 ? inspectionNotifications : null;
            return Task.FromResult((result, relatedInspectionReports));
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    public Task<(List<InspectionNotification>, List<InspectionReport>)> GetInspectionNotificationByRequestId0(Guid requestId,string usrId)
    {

        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotification> inspectionNotifications;
            List<InspectionNotificationContact>? contacts;
            List<Order> orders;
            List<InspectionType> inspectionTypes;
            List<InspectionNotificationSubject> subjects;
            List<InspectionNotificationLocation> locations;
            List<InspectionNotificationType> types;
            List<Circulation> participatedCirculations;
            List<InspectionReport> relatedInspectionReports;
            List<InspectionNotificationRevision> revisionsRelatedToInsNotification;

            var circulationDict = new Dictionary<Guid, Circulation>();

            using (var multi = connection.QueryMultiple(
            "GetInspectionNotification_requestId_userId",
            new { RequestId = requestId , userId = usrId },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {
                // inspectionNotifications = multi.Read<InspectionNotification>().ToList();
                //participatedCirculations = multi.Read<Circulation>().ToList();

                //  participatedCirculations = multi.Read<Circulation, Contact, Inspection.Domain.Entities.InspectionNotification.Entities.InspectionNotification, Circulation>(
                //    (cr, ct,rq) => { cr.PerformerContactPerson = ct; cr.InspectionNotification = rq; return cr; },
                //    splitOn: "ContactId,RequestId"
                //).ToList();


                // Read first result set (main data)
                var notificationLookup = new Dictionary<Guid, InspectionNotification>();
                var revisionLookup = new Dictionary<Guid, Revision>();
                var circulationLookup = new Dictionary<Guid, Circulation>();

                var test = multi.Read<dynamic>().ToList();
                if (test.Any())
                {
                    var first = test.First() as IDictionary<string, object>;
                    var columnNames = string.Join(", ", first.Keys);
                    Console.WriteLine("Columns in result set: " + columnNames);
                    // Or use a breakpoint to inspect
                }

                // Optional: link lookup if needed
                var insRevLookup = new Dictionary<Guid, InspectionNotificationRevision>();

                var mainData = multi.Read<Circulation, Contact, Request, Revision, InspectionNotificationRevision, InspectionNotification,  InspectionNotification>(
                    (cr, ct, rq, rvs, insRev, ins) =>
                    {
                        // 1. Get or create InspectionNotification
                        if (!notificationLookup.TryGetValue(ins.InspectionNotificationId, out var notification))
                        {
                            notification = ins;
                            notification.Revisions = new List<InspectionNotificationRevision>();
                            notificationLookup.Add(notification.InspectionNotificationId, notification);
                        }

                        // 2. Get or create Revision (attach Request)
                        if (!revisionLookup.TryGetValue(rvs.RevisionId, out var revision))
                        {
                            revision = rvs;
                            revision.Request = rq;
                            revision.InspectionNotifications = new List<InspectionNotificationRevision>();
                            revision.Circulations = new List<Circulation>();
                            revisionLookup.Add(revision.RevisionId, revision);
                        }

                        // 3. Handle InspectionNotificationRevision link
                        if (insRev != null && !notification.Revisions.Any(x => x.Id == insRev.Id))
                        {
                            insRev.InspectionNotification = notification;
                            insRev.Revision = revision;
                            notification.Revisions.Add(insRev);
                            revision.InspectionNotifications.Add(insRev);
                        }

                        // 4. Get or create Circulation (attach Contact)
                        if (!circulationLookup.TryGetValue(cr.Id, out var circulation))
                        {
                            circulation = cr;
                            circulation.PerformerContactPerson = ct;
                            circulation.Attachments = new List<CirculationAttachments>(); // will be filled later
                            circulationLookup.Add(cr.Id, circulation);
                            revision.Circulations.Add(circulation);
                        }
                        return notification; 
                    },
                    splitOn: "ContactId,RequestId,RevisionId,InspectionNotificationRevisionId,InspectionNotificationId")
                    .Distinct()
                    .ToList();

                // Read second result set (attachments)
                List<CirculationAttachments> crattachments = multi.Read<CirculationAttachments, Attachment, CirculationAttachments>(
                    (cra, atc) =>
                    {
                        cra.Attachment = atc;
                        return cra;
                    },
                    splitOn: "AttachmentId")
                    .ToList();

                // Now attach attachments to circulations
                foreach (var cra in crattachments)
                {
                    if (circulationLookup.TryGetValue(cra.CirculationId.Value, out var circ))
                    {
                        circ.Attachments.Add(cra);
                    }
                }

                // Result is notificationLookup.Values
                inspectionNotifications = notificationLookup.Values.ToList();



                orders = multi.Read<Order>().ToList();
                contacts = multi.Read<InspectionNotificationContact, Contact, InspectionNotificationContact>(
                   (insc, cnt) => { insc.Contact = cnt; return insc; },
                   splitOn: "ContactId"
               ).ToList();
                subjects = multi.Read<InspectionNotificationSubject, InspectionSubject, InspectionNotificationSubject>(
                   (inss, ins) => { inss.InspectionSubject = ins; return inss; },
                   splitOn: "InspectionSubjectId"
               ).ToList();
                locations = multi.Read<InspectionNotificationLocation, Address, InspectionNotificationLocation>(
                   (insl, adr) => { insl.Location = adr; return insl; },
                   splitOn: "AddressId"
               ).ToList();
                types = multi.Read<InspectionNotificationType, InspectionType, InspectionNotificationType>(
                    (inst, inty) => { inst.InspectionType = inty; return inst; },
                    splitOn: "InspectionTypeId"
                ).ToList();

                relatedInspectionReports = multi.Read<InspectionReport, Revision, InspectionReport>(
                    (insr, rvs) => { insr.Revision = rvs; return insr; },
                    splitOn: "RevisionId").ToList();
            }

            if (inspectionNotifications.Count > 0 && inspectionNotifications[0] != null)
            {
                inspectionNotifications[0].Order = orders.Count > 0 ? orders[0] : null;
                inspectionNotifications[0].Contacts = contacts;
                inspectionNotifications[0].Subjects = subjects;
                inspectionNotifications[0].Types = types;
                inspectionNotifications[0].Locations = locations;
                //inspectionNotifications[0].Revisions =


            }
            List<InspectionNotification> result = inspectionNotifications.Count > 0 ? inspectionNotifications : null;
            return Task.FromResult((result, relatedInspectionReports));
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    public Task<(List<InspectionNotification>,List<InspectionReport>)> GetInspectionNotificationById(InspectionNotificationId inspectionNotificationId)
    {

        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotification> inspectionNotifications;
            List<InspectionNotificationContact>? contacts;
            List<Order> orders;
            List<InspectionType> inspectionTypes;
            List<InspectionNotificationSubject> subjects;
            List<InspectionNotificationLocation> locations;
            List<InspectionNotificationType> types;
            List<Circulation> participatedCirculations;
            List<InspectionReport> relatedInspectionReports;
            List<InspectionNotificationRevision> revisionsRelatedToInsNotification;

            var circulationDict = new Dictionary<Guid, Circulation>();

            using (var multi = connection.QueryMultiple(
            "GetInspectionNotification_Id2",
            new { InspectionNotificationId = inspectionNotificationId.Value },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {
                // inspectionNotifications = multi.Read<InspectionNotification>().ToList();
                //participatedCirculations = multi.Read<Circulation>().ToList();

                //  participatedCirculations = multi.Read<Circulation, Contact, Inspection.Domain.Entities.InspectionNotification.Entities.InspectionNotification, Circulation>(
                //    (cr, ct,rq) => { cr.PerformerContactPerson = ct; cr.InspectionNotification = rq; return cr; },
                //    splitOn: "ContactId,RequestId"
                //).ToList();


                var notificationlookup = new Dictionary<Guid, InspectionNotification>();
                var circulationLookup = new Dictionary<Guid, Circulation>();
                inspectionNotifications = multi.Read<Circulation, Contact, Request, Revision, InspectionNotification, CirculationAttachments, Attachment, InspectionNotification>(
                    (cr, ct, rq, rvs, ins, cra, atc) =>
                    {
                    if (!notificationlookup.TryGetValue(ins.InspectionNotificationId, out var notification)) 
                        {
                            notification = ins; 
                            //notification.Revisions = rvs; 
                            //notification.Revision.Circulations = new List<Circulation>();
                            notificationlookup.Add(notification.InspectionNotificationId, notification);
                        } 
                            // attach contact
                         cr.PerformerContactPerson = ct; 
                        // attach request
                        rvs.Request = rq;

                        // -------------------------------
                        // Circulation (N)
                        // -------------------------------
                        if (!circulationLookup.TryGetValue(cr.Id, out var circulation))
                        {
                            circulation = cr;
                            circulation.PerformerContactPerson = ct;
                            circulation.Attachments = new List<CirculationAttachments>();

                            circulationLookup.Add(cr.Id, circulation);
                            //notification.Revision.Circulations.Add(circulation);
                        }

                        // -------------------------------
                        // Attachments (N)
                        // -------------------------------
                        if (cra != null)
                        {
                            cra.Attachment = atc;
                            cr.Attachments ??= new List<CirculationAttachments>();
                            circulation.Attachments.Add(cra);
                        }

                      
                        // add circulation to revision
                        //if(!notification.Revision.Circulations.Contains(cr))
                        //    notification.Revision.Circulations.Add(cr);
                        

                            return notification; 
                    },
                    splitOn: "ContactId,RequestId,RevisionId,InspectionNotificationId,Id,AttachmentId").Distinct().ToList();
                   


                orders = multi.Read<Order>().ToList();
                contacts = multi.Read<InspectionNotificationContact, Contact, InspectionNotificationContact>(
                   (insc, cnt) => { insc.Contact = cnt; return insc; },
                   splitOn: "ContactId"
               ).ToList();
                subjects = multi.Read<InspectionNotificationSubject, InspectionSubject, InspectionNotificationSubject>(
                   (inss, ins) => { inss.InspectionSubject = ins; return inss; },
                   splitOn: "InspectionSubjectId"
               ).ToList();
                locations = multi.Read<InspectionNotificationLocation, Address, InspectionNotificationLocation>(
                   (insl, adr) => { insl.Location = adr; return insl; },
                   splitOn: "AddressId"
               ).ToList();
                types = multi.Read<InspectionNotificationType, InspectionType, InspectionNotificationType>(
                    (inst, inty) => { inst.InspectionType = inty; return inst; },
                    splitOn: "InspectionTypeId"
                ).ToList();

                relatedInspectionReports = multi.Read<InspectionReport, Revision, InspectionReport>(
                    (insr, rvs) => { insr.Revision = rvs;return insr; },
                    splitOn: "RevisionId").ToList();
            }

            if (inspectionNotifications.Count > 0 && inspectionNotifications[0] != null)
            {
                inspectionNotifications[0].Order = orders.Count>0 ? orders[0] : null;
                inspectionNotifications[0].Contacts = contacts;
                inspectionNotifications[0].Subjects = subjects;
                inspectionNotifications[0].Types = types;
                inspectionNotifications[0].Locations = locations;
               // inspectionNotifications[0].Revisions = 
                
            }
            List<InspectionNotification> result = inspectionNotifications.Count>0 ? inspectionNotifications : null;
            return Task.FromResult((result,relatedInspectionReports));
        }
        catch (Exception ex)
        {
            return null;
        }
    }


    public Task<InspectionNotification> GetInspectionNotificationById1(InspectionNotificationId inspectionNotificationId)
    {

        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotification> inspectionNotifications;
            List<Contact> contacts;
            List<InspectionType> inspectionTypes;
            List<InspectionNotificationSubject> inspectionNotificationSubjects;
            List<InspectionNotificationContact> inspectionNotificationContacts;
            List<InspectionNotificationType> inspectionNotificationTypes;
            List<Circulation> participatedCirculations;

            using (var multi = connection.QueryMultiple(
            "GetInspectionNotification_Id",
            new { InspectionNotificationId = inspectionNotificationId.Value },
            commandType: CommandType.StoredProcedure, transaction: adoTransaction))
            {


                inspectionNotifications = multi.Read<InspectionNotification, Order, Circulation, InspectionNotificationContact, Contact, InspectionNotification>(
(inspectionNotification, order, circulation, inspectionNotificationContact, contact) =>
{
    inspectionNotification.Id = inspectionNotification.InspectionNotificationId.Value;
    inspectionNotification.NotificationNumberParts = NotificationNumber.CreateFromString(inspectionNotification.NotificationNumber);
   

    inspectionNotification.Order = order;   

    // In case notificationContact exists, attach the contact from the Contacts table.
    if (inspectionNotificationContact != null)
    {
        inspectionNotificationContact.Contact = contact;
        inspectionNotificationContact.Contact.ContactId = contact.ContactId;
        // Add to the collection.
        inspectionNotification.Contacts!.Add(inspectionNotificationContact);
    }


    if (inspectionNotification?.InspectionNotificationId! != null)
    {
        inspectionNotification.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotification.InspectionNotificationId.Value.ToString()));
    }

    return inspectionNotification;
},
splitOn: "OrderId,CirculationId,InspectionNotificationContactId,ContactId"
).ToList();



                //inspectionNotifications = multi.Read().Select(ino => new InspectionNotification()
                //{
                //    InspectionNotificationId = new InspectionNotificationId(new Guid(ino.InspectionNotificationId.ToString())),
                //    ModifiedDate = ino.ModifiedDate,
                //    CreatedBy = ino.CreatedBy,
                //    CreatedDate = ino.CreatedDate,
                //    Description = ino.Description,
                //    Id = ino.Id,
                //    IssueDate = ino.IssueDate,
                //    NotificationNumber = notificationNumber,
                //    PersianIssueDate = ino.PersianIssueDate,
                //    RequestInspectionDateEnd = ino.RequestInspectionDateEnd,
                //    RequestInspectionDateStart = ino.RequestInspectionDateStart,
                //    RequestInspectionDueDate = ino.RequestInspectionDueDate,
                //    RequestInspectionPeriodDays = ino.RequestInspectionPeriodDays,
                //    RequestInspectionPersianDateEnd = ino.RequestInspectionPersianDateEnd,
                //    RequestInspectionPersianDateStart = ino.RequestInspectionPersianDateStart,
                //    RequestInspectionTimesInPeriod = ino.RequestInspectionTimesInPeriod,
                //    NotificationNumberParts = ino.NotificationNumberParts,
                //    Version = ino.Version


                //}).ToList();


                //participatedCirculations = multi.Read().Select(cr => new Circulation()
                //{

                //    ActionType = cr.ActionType,
                //    CreatedBy = cr.CreatedBy,
                //    CreatedDate = cr.CreatedDate,
                //    Id = cr.Id,
                //    ModifiedBy = cr.ModifiedBy,
                //    ModifiedDate = cr.ModifiedDate,
                //    PerformedActionDate = cr.PerformedActionDate,
                //    PerformedActionDateGregorian = cr.PerformedActionDateGregorian,
                //    PerformerContactPerson = cr.PerformerContactPerson,
                //    PerformerContactType = cr.PerformerContactType,
                //    PerformerPN = cr.PerformerPN,

                //    SequenceNo = cr.SequenceNo
                //}).ToList();







                //                inspectionNotificationContacts = multi.Read<InspectionNotificationContact, Contact, InspectionNotificationContact>(
                //  (inspectionNotificationContact, contact) =>
                //  {
                //      inspectionNotificationContact.Contact = contact;
                //      if (inspectionNotificationContact?.InspectionNotificationId! != null)
                //      {
                //          inspectionNotificationContact.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotificationContact.InspectionNotificationId.Value.ToString()));
                //      }
                //      inspectionNotificationContact.ContactId = contact.Id;
                //      inspectionNotificationContact.Id = inspectionNotificationContact.Id;
                //      return inspectionNotificationContact;
                //  },
                //  splitOn: "ContactId"
                //).ToList();

                //                // Third result set

                //                inspectionNotificationSubjects = multi.Read<InspectionNotificationSubject, InspectionSubject, InspectionNotificationSubject>(
                //         (inspectionNotificationSubject, inspectionSubject) =>
                //         {
                //             inspectionNotificationSubject.InspectionSubject = inspectionSubject;
                //             if (inspectionNotificationSubject.InspectionNotificationId != null)
                //             {
                //                 inspectionNotificationSubject.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotificationSubject.InspectionNotificationId.Value.ToString()));
                //             }
                //             inspectionNotificationSubject!.InspectionSubjectId = inspectionSubject.InspectionSubjectId;
                //             inspectionNotificationSubject.Id = inspectionNotificationSubject.Id;
                //             return inspectionNotificationSubject;
                //         },
                //         splitOn: "InspectionSubjectId"
                //     ).ToList();


                //                inspectionNotificationTypes = multi.Read<InspectionNotificationType, InspectionType, InspectionNotificationType>(
                //    (inspectionNotificationType, inspectionType) =>
                //    {
                //        inspectionNotificationType.InspectionType = inspectionType;
                //        if (inspectionNotificationType.InspectionNotificationId != null)
                //        {
                //            inspectionNotificationType.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotificationType.InspectionNotificationId.Value.ToString()));
                //        }
                //        inspectionNotificationType!.InspectionTypeId = inspectionType.InspectionTypeId;
                //        inspectionNotificationType.Id = inspectionNotificationType.Id;
                //        return inspectionNotificationType;
                //    },
                //    splitOn: "InspectionTypeId"
                //).ToList();

            }

            // connection.Close();


            //            InspectionNotificationFlow inspectionNotificationFlow = new InspectionNotificationFlow();
            //            inspectionNotificationFlow.InspectionNotifications = inspectionNotifications;
            //            inspectionNotificationFlow.Circulations = participatedCirculations;
            //            inspectionNotificationFlow.Contacts = inspectionNotificationContacts;
            //            inspectionNotificationFlow.Subjects = inspectionNotificationSubjects;
            //            inspectionNotificationFlow.Types = inspectionNotificationTypes;

            //return await Task.FromResult(inspectionNotificationFlow);

            InspectionNotification result = inspectionNotifications[0];
            return Task.FromResult(result);
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


    public Task<List<InspectionNotificationHistoryDto>> GetInspectionNotificationHistory(Guid requestId)
    {
        List<InspectionNotification> draftedNotificationList = new List<InspectionNotification>();
        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotificationHistoryDto> inspectionNotifications;
            List<Circulation> participatedCirculations;

            using (var multi = connection.QueryMultiple(
            "GetInspectionNotificationRequestHistoryById",
            new { RequestId = requestId },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {

                inspectionNotifications = multi.Read().Select(ins => new InspectionNotificationHistoryDto()
                {
                    InspectionNotificationId = ins.InspectionNotificationId.ToString(),
                    IssueDate = ins.IssueDate,
                    PersianInspectionEndDate = ins.PersianInspectionEndDate == null ? "" : ins.PersianInspectionEndDate,
                    CirculationId = ins.CirculationId,
                    RevisionId = ins.RevisionId,                    
                    RevisionNumber = ins.RevisionNumber,
                    ActionType = ins.ActionType,
                    ActionName = ins.ActionName,
                    PerformedActionDateGregorian = ins.PerformedActionDateGregorian,
                    PerformerContactType = ins.PerformerContactType,
                    ContactType = ins.ContactType,
                    UserName = ins.UserName,
                    InspectionEndDate = ins.InspectionEndDate == null ? "" : ins.InspectionEndDate,
                    InspectionStartDate = ins.InspectionStartDate == null ? "" : ins.InspectionStartDate,
                    NotificationMCContacts = ins.NotificationMCContacts == null ? "" : ins.NotificationMCContacts,
                    NotificationOwnerContacts = ins.NotificationOwnerContacts == null ? "" : ins.NotificationOwnerContacts,
                    NotificationPIDECContacts = ins.NotificationPIDECContacts == null ? "" : ins.NotificationPIDECContacts,
                    NotificationSubjects = ins.NotificationSubjects == null ? "" : ins.NotificationSubjects,
                    NotificationTypes = ins.NotificationTypes == null ? "" : ins.NotificationTypes,
                    PersianInspectionStartDate = ins.PersianInspectionStartDate,
                    RelatedOrderName = ins.RelatedOrderName,
                    RelatedOrderNumber = ins.RelatedOrderNumber == null ? "" : ins.RelatedOrderNumber,
                    RelatedProjectName = ins.RelatedProjectName == null ? "" : ins.RelatedProjectName,
                    RequestId = ins.RequestId,
                    SequenceNo = ins.SequenceNo,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,

                }).ToList();

            }

            // connection.Close();


            return Task.FromResult(inspectionNotifications);
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

    public Task<List<InspectionNotificationDataSummaryDto>> GetDraftInspectionNotificationsByUserName(string userName)
    {
        List<InspectionNotification> draftedNotificationList = new List<InspectionNotification>();
        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotificationDataSummaryDto> inspectionNotifications;
            List<Circulation> participatedCirculations;

            using (var multi = connection.QueryMultiple(
            "GetDraftedInspectionNotifications_UserName1",
            new { userName = userName },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {               

                inspectionNotifications = multi.Read().Select(ins => new InspectionNotificationDataSummaryDto()
                {
                    InspectionNotificationId = ins.InspectionNotificationId.ToString(),
                    IssueDate = ins.IssueDate,
                    PersianInspectionEndDate = ins.PersianInspectionEndDate == null ? "" : ins.PersianInspectionEndDate,
                    CirculationId = ins.CirculationId,
                    RevisionId = ins.RevisionId,
                    InspectionEndDate = ins.InspectionEndDate == null ? "" : ins.InspectionEndDate,
                    InspectionStartDate = ins.InspectionStartDate == null ? "" : ins.InspectionStartDate,
                    NotificationMCContacts = ins.NotificationMCContacts == null ? "" : ins.NotificationMCContacts,
                    NotificationOwnerContacts = ins.NotificationOwnerContacts == null ? "" : ins.NotificationOwnerContacts,
                    NotificationPIDECContacts = ins.NotificationPIDECContacts == null ? "" : ins.NotificationPIDECContacts,
                    NotificationSubjects = ins.NotificationSubjects == null ? "" : ins.NotificationSubjects,
                    NotificationTypes = ins.NotificationTypes == null ? "" : ins.NotificationTypes,
                    PersianInspectionStartDate = ins.PersianInspectionStartDate,
                    RelatedOrderName = ins.RelatedOrderName,
                    RelatedOrderNumber = ins.RelatedOrderNumber == null ? "" : ins.RelatedOrderNumber,
                    RelatedProjectName = ins.RelatedProjectName == null ? "" : ins.RelatedProjectName,
                    RequestId = ins.RequestId,
                    SequenceNo = ins.SequenceNo,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,

                }).ToList();              

            }

            // connection.Close();


            return Task.FromResult(inspectionNotifications);
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

    public Task<List<InspectionNotificationDataSummaryDto>> GetSentInspectionNotificationsByUserName(string userName)
    {
        List<InspectionNotification> draftedNotificationList = new List<InspectionNotification>();
        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotificationDataSummaryDto> inspectionNotifications;
            List<Circulation> participatedCirculations;

            using (var multi = connection.QueryMultiple(
            "GetSentInspectionNotifications_UserName1",
            new { userName = userName },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {
                //inspectionNotifications = multi.Read().Select(ins => new InspectionNotification()
                //{
                //    InspectionNotificationId = new InspectionNotificationId(new Guid(ins.InspectionNotificationId.ToString())),
                //    IssueDate = ins.IssueDate,
                //    Name = ins.Name,
                //    NotificationNumber = ins.NotificationNumber,
                //    PersianIssueDate = ins.PersianIssueDate,
                //    RequestInspectionDateEnd = ins.RequestInspectionDateEnd,
                //    RequestInspectionDateStart = ins.RequestInspectionDateStart,
                //    RequestInspectionDueDate = ins.RequestInspectionDueDate,
                //    RequestInspectionPeriodDays = ins.RequestInspectionPeriodDays,
                //    RequestInspectionPersianDateEnd = ins.RequestInspectionPersianDateEnd,
                //    RequestInspectionPersianDateStart = ins.RequestInspectionPersianDateStart,
                //    RequestInspectionTimesInPeriod = ins.RequestInspectionTimesInPeriod,
                //    Version = ins.Version,
                //    ModifiedBy = ins.ModifiedBy,
                //    ModifiedDate = ins.ModifiedDate,
                //    CreatedBy = ins.CreatedBy,
                //    CreatedDate = ins.CreatedDate

                //}).ToList();
           
                inspectionNotifications = multi.Read().Select(ins => new InspectionNotificationDataSummaryDto()
                {
                    InspectionNotificationId = ins.InspectionNotificationId.ToString(),
                    IssueDate = ins.IssueDate,
                    PersianInspectionEndDate = ins.PersianInspectionEndDate == null ? "" : ins.PersianInspectionEndDate,
                    CirculationId = ins.CirculationId,
                    InspectionEndDate = ins.InspectionEndDate == null ? "" : ins.InspectionEndDate,
                    InspectionStartDate = ins.InspectionStartDate == null ? "" : ins.InspectionStartDate,
                    NotificationMCContacts = ins.NotificationMCContacts == null ? "" : ins.NotificationMCContacts,
                    NotificationOwnerContacts = ins.NotificationOwnerContacts == null ? "" : ins.NotificationOwnerContacts,
                    NotificationPIDECContacts = ins.NotificationPIDECContacts == null ? "" : ins.NotificationPIDECContacts,
                    NotificationSubjects = ins.NotificationSubjects == null ? "" : ins.NotificationSubjects,
                    NotificationTypes = ins.NotificationTypes == null ? "" : ins.NotificationTypes,
                    PersianInspectionStartDate = ins.PersianInspectionStartDate,
                    RelatedOrderName = ins.RelatedOrderName,
                    RelatedOrderNumber = ins.RelatedOrderNumber == null ? "" : ins.RelatedOrderNumber,
                    RelatedProjectName = ins.RelatedProjectName == null ? "" : ins.RelatedProjectName,
                    RequestId = ins.RequestId,
                    SequenceNo = ins.SequenceNo,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,

                }).ToList();

                // projects = multi.Read<Project>().ToList();

                //circulations = multi.Read().Select(cr => new Circulation()
                //{
                //    Id = new Guid(cr.Id.ToString()),
                //    Request = cr.Request,
                //    ActionType = cr.ActionType,
                //    CreatedBy = cr.CreatedBy,
                //    CreatedDate = cr.CreatedDate,
                //    ModifiedBy = cr.ModifiedBy,
                //    ModifiedDate = cr.ModifiedDate,
                //    PerformedActionDate = cr.PerformedActionDate,
                //    PerformedActionDateGregorian = cr.PerformedActionDateGregorian,
                //    PerformerContactPerson = cr.PerformerContactPerson,
                //    PerformerContactType = cr.PerformerContactType,
                //    PerformerPN = cr.PerformerPN,
                //    SequenceNo = cr.SequenceNo
                //}).ToList();

            }

            // connection.Close();


            return Task.FromResult(inspectionNotifications);
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

    public Task<List<InspectionNotificationDataSummaryDto>> GetCompletedInspectionNotificationsByUserName(string userName)
    {
        List<InspectionNotification> draftedNotificationList = new List<InspectionNotification>();
        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotificationDataSummaryDto> inspectionNotifications;
            List<Circulation> participatedCirculations;

            using (var multi = connection.QueryMultiple(
            "GetCompletedInspectionNotifications_UserName",
            new { userName = userName },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {


                inspectionNotifications = multi.Read().Select(ins => new InspectionNotificationDataSummaryDto()
                {
                    InspectionNotificationId = ins.InspectionNotificationId.ToString(),
                    IssueDate = ins.IssueDate,
                    PersianInspectionEndDate = ins.PersianInspectionEndDate == null ? "" : ins.PersianInspectionEndDate,
                    CirculationId = ins.CirculationId,
                    InspectionEndDate = ins.InspectionEndDate == null ? "" : ins.InspectionEndDate,
                    InspectionStartDate = ins.InspectionStartDate == null ? "" : ins.InspectionStartDate,
                    NotificationMCContacts = ins.NotificationMCContacts == null ? "" : ins.NotificationMCContacts,
                    NotificationOwnerContacts = ins.NotificationOwnerContacts == null ? "" : ins.NotificationOwnerContacts,
                    NotificationPIDECContacts = ins.NotificationPIDECContacts == null ? "" : ins.NotificationPIDECContacts,
                    NotificationSubjects = ins.NotificationSubjects == null ? "" : ins.NotificationSubjects,
                    NotificationTypes = ins.NotificationTypes == null ? "" : ins.NotificationTypes,
                    PersianInspectionStartDate = ins.PersianInspectionStartDate,
                    RelatedOrderName = ins.RelatedOrderName,
                    RelatedOrderNumber = ins.RelatedOrderNumber == null ? "" : ins.RelatedOrderNumber,
                    RelatedProjectName = ins.RelatedProjectName == null ? "" : ins.RelatedProjectName,
                    RequestId = ins.RequestId,
                    SequenceNo = ins.SequenceNo,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,

                }).ToList();
            }




            return Task.FromResult(inspectionNotifications);
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

    public Task<List<InspectionNotificationDataSummaryDto>> GetNotStartedInspectionNotificationsByUserName(string userName)
    {
        List<InspectionNotification> draftedNotificationList = new List<InspectionNotification>();
        try
        {

            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            List<InspectionNotificationDataSummaryDto> inspectionNotifications;
            List<Circulation> participatedCirculations;

            using (var multi = connection.QueryMultiple(
            "GetNotStartedInspectionNotifications_UserName1",
            new { userName = userName },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {


                inspectionNotifications = multi.Read().Select(ins => new InspectionNotificationDataSummaryDto()
                {
                    InspectionNotificationId = ins.InspectionNotificationId.ToString(),
                    IssueDate = ins.IssueDate,
                    PersianInspectionEndDate = ins.PersianInspectionEndDate == null ? "" : ins.PersianInspectionEndDate,
                    CirculationId = ins.CirculationId,
                    InspectionEndDate = ins.InspectionEndDate == null ? "" : ins.InspectionEndDate,
                    InspectionStartDate = ins.InspectionStartDate == null ? "" : ins.InspectionStartDate,
                    NotificationMCContacts = ins.NotificationMCContacts == null ? "" : ins.NotificationMCContacts,
                    NotificationOwnerContacts = ins.NotificationOwnerContacts == null ? "" : ins.NotificationOwnerContacts,
                    NotificationPIDECContacts = ins.NotificationPIDECContacts == null ? "" : ins.NotificationPIDECContacts,
                    NotificationSubjects = ins.NotificationSubjects == null ? "" : ins.NotificationSubjects,
                    NotificationTypes = ins.NotificationTypes == null ? "" : ins.NotificationTypes,
                    PersianInspectionStartDate = ins.PersianInspectionStartDate,
                    RelatedOrderName = ins.RelatedOrderName,
                    RelatedOrderNumber = ins.RelatedOrderNumber == null ? "" : ins.RelatedOrderNumber,
                    RelatedProjectName = ins.RelatedProjectName == null ? "" : ins.RelatedProjectName,
                    RequestId = ins.RequestId,
                    SequenceNo = ins.SequenceNo,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,

                }).ToList();
            }




            return Task.FromResult(inspectionNotifications);
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

    public Task<List<InspectionNotificationDataSummaryDto>> GetInspectionNotificationsByProjUsrIds(string ProjectId, string UserId)
    {

        try
        {


            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();


            List<InspectionNotification> inspectionNotifications;
            List<Circulation> circulations;
            List<InspectionType> inspectionTypes;
            List<InspectionNotificationContact> inspectionNotificationContacts;
            List<OrderContact> orderContacts;
            List<Circulation> participatedCirculations;
            List<InspectionNotificationDataSummaryDto> inspectionNotificationsSummary;
            if (ProjectId == "")
            {
                ProjectId = Guid.Empty.ToString();
            }
            using (var multi = connection.QueryMultiple(
            "GetInspectionNotificationData_ProjId_UserId",
            new { ProjectId = new Guid(ProjectId), UserId = UserId },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {

                inspectionNotifications = multi.Read().Select(ins => new InspectionNotification()
                {
                    InspectionNotificationId = new InspectionNotificationId(new Guid(ins.InspectionNotificationId.ToString())),
                    IssueDate = ins.IssueDate,
                    Name = ins.Name,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,
                    RequestInspectionDateEnd = ins.RequestInspectionDateEnd,
                    RequestInspectionDateStart = ins.RequestInspectionDateStart,
                    RequestInspectionDueDate = ins.RequestInspectionDueDate,                   
                    RequestInspectionPersianDateEnd = ins.RequestInspectionPersianDateEnd,
                    RequestInspectionPersianDateStart = ins.RequestInspectionPersianDateStart,
                    InspectionAction = ins.InspectionAction,
                    InspectionTimeComment = ins.InspectionTimeComment,
                    InspectionActionComment = ins.InspectionActionComment,
                    //Version = ins.Version,
                    ModifiedBy = ins.ModifiedBy,
                    ModifiedDate = ins.ModifiedDate,
                    CreatedBy = ins.CreatedBy,
                    CreatedDate = ins.CreatedDate

                }).ToList();


                // projects = multi.Read<Project>().ToList();

                circulations = multi.Read().Select(cr => new Circulation()
                {
                    Id = new Guid(cr.Id.ToString()),
                    ActionType = cr.ActionType,
                    CreatedBy = cr.CreatedBy,
                    CreatedDate = cr.CreatedDate,
                    ModifiedBy = cr.ModifiedBy,
                    ModifiedDate = cr.ModifiedDate,
                    PerformedActionDate = cr.PerformedActionDate,
                    PerformedActionDateGregorian = cr.PerformedActionDateGregorian,
                    PerformerContactPerson = cr.PerformerContactPerson,
                    PerformerContactType = cr.PerformerContactType,
                    PerformerPN = cr.PerformerPN,
                    SequenceNo = cr.SequenceNo
                }).ToList();



                // Third result set

                inspectionNotificationContacts = multi.Read<InspectionNotificationContact, Contact, InspectionNotificationContact>(
                    (inspectionNotificationContact, contact) =>
                    {
                        inspectionNotificationContact.Contact = contact;
                        if (inspectionNotificationContact?.InspectionNotificationId! != null)
                        {
                            inspectionNotificationContact.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotificationContact.InspectionNotificationId.Value.ToString()));
                        }
                        inspectionNotificationContact.ContactId = contact.ContactId;
                        inspectionNotificationContact.Id = inspectionNotificationContact.Id;
                        return inspectionNotificationContact;
                    },
                  splitOn: "ContactId"
                ).ToList();


                inspectionNotificationsSummary = multi.Read().Select(ins => new InspectionNotificationDataSummaryDto()
                {
                    InspectionNotificationId = ins.InspectionNotificationId.ToString(),
                    IssueDate = ins.IssueDate,
                    PersianInspectionEndDate = ins.PersianInspectionEndDate == null ? "" : ins.PersianInspectionEndDate,
                    CirculationId = ins.CirculationId,
                    InspectionEndDate = ins.InspectionEndDate == null ? "" : ins.InspectionEndDate,
                    InspectionStartDate = ins.InspectionStartDate == null ? "" : ins.InspectionStartDate,
                    NotificationMCContacts = ins.NotificationMCContacts == null ? "" : ins.NotificationMCContacts,
                    NotificationOwnerContacts = ins.NotificationOwnerContacts == null ? "" : ins.NotificationOwnerContacts,
                    NotificationPIDECContacts = ins.NotificationPIDECContacts == null ? "" : ins.NotificationPIDECContacts,
                    NotificationSubjects = ins.NotificationSubjects == null ? "" : ins.NotificationSubjects,
                    NotificationTypes = ins.NotificationTypes == null ? "" : ins.NotificationTypes,
                    PersianInspectionStartDate = ins.PersianInspectionStartDate,
                    RelatedOrderName = ins.RelatedOrderName,
                    RelatedOrderNumber = ins.RelatedOrderNumber == null ? "" : ins.RelatedOrderNumber,
                    RelatedProjectName = ins.RelatedProjectName == null ? "" : ins.RelatedProjectName,
                    RequestId = ins.RequestId,
                    SequenceNo = ins.SequenceNo,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,

                }).ToList();


            }

            //connection.Close();


            return Task.FromResult(inspectionNotificationsSummary);
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

    Task<InspectionNotification> IGenericRepository<InspectionNotification, InspectionNotificationId>.Add(InspectionNotification entity)
    {

        try
        {
            Order order = _dbContext.Orders.Find(entity.Order!.Id)!;
            _dbContext.Entry(order).State = EntityState.Unchanged;

            _dbContext.InspectionNotifications.Add(entity);

            _dbContext.SaveChanges();

        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }

        return null;
    }

    Task IGenericRepository<InspectionNotification, InspectionNotificationId>.Delete(InspectionNotification entity)
    {
        throw new NotImplementedException();
    }


    async Task<InspectionNotification> IGenericRepository<InspectionNotification, InspectionNotificationId>.Get(InspectionNotificationId id)
    {
        var inspectionNotification = await _dbContext.InspectionNotifications
         .Include(n => n.Contacts)
         .Include(n => n.Order)         

         .FirstOrDefaultAsync(n => n.InspectionNotificationId == id);

        return inspectionNotification!;
    }

    async Task<IReadOnlyList<InspectionNotification>> IGenericRepository<InspectionNotification, InspectionNotificationId>.GetAll()
    {
        try
        {
            var inspectionNotification = await _dbContext.InspectionNotifications
            //.Include(n=>n.Id)
            .Include(n => n.Contacts)
            .Include(n => n.Order!.Project)            

            // .Include(n => n.Contacts)
            .ToListAsync();

            return inspectionNotification!;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    Task<InspectionNotification> IGenericRepository<InspectionNotification, InspectionNotificationId>.Update(InspectionNotification entity)
    {
        throw new NotImplementedException();
    }

    public async Task<NotificationNumber> GenerateNewNotificationNumber(string projectNumber, string orderNumber)
    {
        NotificationNumber notificationNumber = _dbContext.NotificationNumbers.AsNoTracking().Where(nn => nn.ProjectNumber == projectNumber && nn.OrderNumber == orderNumber).FirstOrDefault()!;

        if (notificationNumber?.Id != null)
        {            
            // return Task.FromResult(notificationNumber.CreateNewVersion());
            return await Task.FromResult(notificationNumber.CreateNewVersion());
        }
        else
        {
            return await Task.FromResult(NotificationNumber.CreateFromNull(projectNumber, orderNumber, "NTF"));
        }

    }

    public Task<bool> UpdateNotificationNumber(NotificationNumber notificationNumber)
    {
        try
        {
            NotificationNumber latestNotificationNumber = _dbContext.NotificationNumbers.Where(nn => nn.ProjectNumber == notificationNumber.ProjectNumber && nn.OrderNumber == notificationNumber.OrderNumber
                                                && nn.InfoPart == notificationNumber.InfoPart).FirstOrDefault()!;
            if (latestNotificationNumber != null)
            {
                if (latestNotificationNumber.Version == notificationNumber.Version)
                {
                    if (latestNotificationNumber.Revision == notificationNumber.Revision)
                        return Task.FromResult(false);
                    else
                        latestNotificationNumber.IncreaseRevision();
                }
                else if(latestNotificationNumber.Version > notificationNumber.Version)//---it means no new Notification is generated
                {
                    return Task.FromResult(false);
                }
                else //---It means a new notification is created
                    latestNotificationNumber.IncreaseVersion();

                _dbContext.NotificationNumbers.Update(latestNotificationNumber);
                return Task.FromResult(true);
            }
            else
            {
                NotificationNumber newNotificationNumber = new NotificationNumber();
                newNotificationNumber.InfoPart = notificationNumber.InfoPart;
                newNotificationNumber.OrderNumber = notificationNumber.OrderNumber;
                newNotificationNumber.ProjectNumber = notificationNumber.ProjectNumber;
                newNotificationNumber.Version = notificationNumber.Version;
                newNotificationNumber.Revision = notificationNumber.Revision;

                _dbContext.NotificationNumbers.Add(newNotificationNumber);
                return Task.FromResult(true);
            }
        }
        catch (Exception exp)
        {
            return Task.FromResult(false);
        }
    }



    public Task<List<InspectionNotificationDataSummaryDto>> GetReadyNotificationsToAttachInspectionReportByProjIdUserId(string ProjectId, string UserId)
    {

        try
        {


            var connection = _dbContext.Database.GetDbConnection();
            // ensure the connection is open
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var adoTransaction =
                _dbContext.Database.CurrentTransaction?.GetDbTransaction();


            List<InspectionNotification> inspectionNotifications;
            List<Circulation> circulations;
            List<InspectionType> inspectionTypes;
            List<InspectionNotificationContact> inspectionNotificationContacts;
            List<OrderContact> orderContacts;
            List<Circulation> participatedCirculations;
            List<InspectionNotificationDataSummaryDto> inspectionNotificationsSummary;

            using (var multi = connection.QueryMultiple(
            " [GetReadyNotificationsToAttachInspectionReport_UserId1]",
            new { ProjectId = new Guid(ProjectId), UserId = UserId },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {

                inspectionNotifications = multi.Read().Select(ins => new InspectionNotification()
                {
                    InspectionNotificationId = new InspectionNotificationId(new Guid(ins.InspectionNotificationId.ToString())),
                    IssueDate = ins.IssueDate,
                    Name = ins.Name,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,
                    RequestInspectionDateEnd = ins.RequestInspectionDateEnd,
                    RequestInspectionDateStart = ins.RequestInspectionDateStart,
                    RequestInspectionDueDate = ins.RequestInspectionDueDate,                   
                    RequestInspectionPersianDateEnd = ins.RequestInspectionPersianDateEnd,
                    RequestInspectionPersianDateStart = ins.RequestInspectionPersianDateStart,
                    InspectionTimeComment = ins.InspectionTimeComment,
                    InspectionAction = ins.InspectionAction,
                    InspectionActionComment = ins.InspectionActionComment,
                    //Version = ins.Version,
                    ModifiedBy = ins.ModifiedBy,
                    ModifiedDate = ins.ModifiedDate,
                    CreatedBy = ins.CreatedBy,
                    CreatedDate = ins.CreatedDate

                }).ToList();


                // projects = multi.Read<Project>().ToList();

                circulations = multi.Read().Select(cr => new Circulation()
                {
                    Id = new Guid(cr.Id.ToString()),
                    ActionType = cr.ActionType,
                    CreatedBy = cr.CreatedBy,
                    CreatedDate = cr.CreatedDate,
                    ModifiedBy = cr.ModifiedBy,
                    ModifiedDate = cr.ModifiedDate,
                    PerformedActionDate = cr.PerformedActionDate,
                    PerformedActionDateGregorian = cr.PerformedActionDateGregorian,
                    PerformerContactPerson = cr.PerformerContactPerson,
                    PerformerContactType = cr.PerformerContactType,
                    PerformerPN = cr.PerformerPN,
                    SequenceNo = cr.SequenceNo
                }).ToList();



                // Third result set

                inspectionNotificationContacts = multi.Read<InspectionNotificationContact, Contact, InspectionNotificationContact>(
                    (inspectionNotificationContact, contact) =>
                    {
                        inspectionNotificationContact.Contact = contact;
                        if (inspectionNotificationContact?.InspectionNotificationId! != null)
                        {
                            inspectionNotificationContact.InspectionNotificationId = InspectionNotificationId.Create(new Guid(inspectionNotificationContact.InspectionNotificationId.Value.ToString()));
                        }
                        inspectionNotificationContact.ContactId = contact.ContactId;
                        inspectionNotificationContact.Id = inspectionNotificationContact.Id;
                        return inspectionNotificationContact;
                    },
                  splitOn: "ContactId"
                ).ToList();


                inspectionNotificationsSummary = multi.Read().Select(ins => new InspectionNotificationDataSummaryDto()
                {
                    InspectionNotificationId = ins.InspectionNotificationId.ToString(),
                    IssueDate = ins.IssueDate,
                    PersianInspectionEndDate = ins.PersianInspectionEndDate == null ? "" : ins.PersianInspectionEndDate,
                    CirculationId = ins.CirculationId,
                    InspectionEndDate = ins.InspectionEndDate == null ? "" : ins.InspectionEndDate,
                    InspectionStartDate = ins.InspectionStartDate == null ? "" : ins.InspectionStartDate,
                    NotificationMCContacts = ins.NotificationMCContacts == null ? "" : ins.NotificationMCContacts,
                    NotificationOwnerContacts = ins.NotificationOwnerContacts == null ? "" : ins.NotificationOwnerContacts,
                    NotificationPIDECContacts = ins.NotificationPIDECContacts == null ? "" : ins.NotificationPIDECContacts,
                    NotificationSubjects = ins.NotificationSubjects == null ? "" : ins.NotificationSubjects,
                    NotificationTypes = ins.NotificationTypes == null ? "" : ins.NotificationTypes,
                    PersianInspectionStartDate = ins.PersianInspectionStartDate,
                    RelatedOrderName = ins.RelatedOrderName,
                    RelatedOrderNumber = ins.RelatedOrderNumber == null ? "" : ins.RelatedOrderNumber,
                    RelatedProjectName = ins.RelatedProjectName == null ? "" : ins.RelatedProjectName,
                    RequestId = ins.RequestId,
                    SequenceNo = ins.SequenceNo,
                    NotificationNumber = ins.NotificationNumber,
                    PersianIssueDate = ins.PersianIssueDate,

                }).ToList();


            }

            //connection.Close();


            return Task.FromResult(inspectionNotificationsSummary);
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


    public async Task<(InspectionNotification, Circulation)> CompleteCirculation(Circulation circulation,InspectionNotification inspectionNotification, List<Attachment> attachments)
    {
        InspectionNotification NewRevisionInspectionNotification = null;
        Revision Newrevision = new Revision();
        try
        {
            // Circulation dbCirculation = _dbContext.Circulations.Where(cr=>cr.Id  == circulation.Id).Include(cr=>cr.InspectionNotification).FirstOrDefault();
            var dbCirculation = await _dbContext.Circulations
           .Include(cr => cr.Revision).Include(cr => cr.PerformerContactPerson)
           .FirstOrDefaultAsync(cr => cr.Id == circulation.Id);

            List<Circulation> otherCirculationsInCurrentSequence = new List<Circulation>();
            if (dbCirculation == null)
                throw new InvalidOperationException("Base circulation not found");

            //otherCirculationsInCurrentSequence = _dbContext.Circulations.Where(cr => cr.SequenceNo == circulation.SequenceNo && cr.Id != circulation.Id
            //&& cr.Revision.RevisionId == dbCirculation.Revision.RevisionId).ToList();

            otherCirculationsInCurrentSequence = _dbContext.Circulations.Where(cr => cr.RevisionId == circulation.RevisionId && cr.Id != circulation.Id
            && cr.Revision.RevisionId == dbCirculation.Revision.RevisionId).ToList();

            int countOfOtherCirculations = otherCirculationsInCurrentSequence.Count;


            if (dbCirculation != null)
            {

                //----------anyway the circulation will be complete
                dbCirculation.PerformedActionDateGregorian = DateTime.Now;
                dbCirculation.ModifiedDate = DateTime.Now;
                dbCirculation.Comment = (circulation.Comment == null || circulation.Comment.Trim() == "") ? "No Comment" : circulation.Comment;
                dbCirculation.ActionType = circulation.ActionType;
                dbCirculation.QCActionType = circulation.ActionType;
                

                //_dbContext.SaveChanges();

                foreach (var attachment in attachments)
                {
                    if (attachment == null) continue;

                    var existingAttachment = await _dbContext.Attachments
                        .FirstOrDefaultAsync(a => a.AttachmentId == attachment.Id && a.DocNo == attachment.DocNo 
                        && a.ContentType == attachment.ContentType && a.Description==attachment.Description && a.Name== attachment.Name && a.Status==attachment.Status);

                    var newCirculationAttachment = new CirculationAttachments
                    {
                        Id = Guid.NewGuid(),
                        CirculationId = dbCirculation.Id
                    };

                    if (existingAttachment != null)
                    {                        
                        newCirculationAttachment.AttachmentId = existingAttachment.AttachmentId;
                       
                    }
                    else
                    {
                        var newAttachment = Attachment.Create(
                            attachment.Name,
                            attachment.Description,
                            attachment.Content,
                            attachment.Version,
                            "-1",
                            "0",
                            attachment.ContentType,
                            attachment.DocNo,
                            attachment.Status);

                        newAttachment.AttachmentId = AttachmentId.CreateUnique();//attachment.Id;
                        _dbContext.Attachments.Add(newAttachment);

                        newCirculationAttachment.AttachmentId = newAttachment.AttachmentId;
                    }

                    _dbContext.CirculationAttachments.Add(newCirculationAttachment);
                }



                _dbContext.SaveChanges();
                
              

                if (circulation.PerformerContactType == (int)ContactType.Vendor)
                {
                    if (circulation.ActionType == ActionTypes.ConfirmInspectionNotification.GetHashCode())
                    {
                        //InspectionNotificationRevision currentInspectionRevision =  _dbContext.InspectionNotificationRevisions.Where(insR => insR.RevisionId == dbCirculation.RevisionId).Include(insR => insR.InspectionNotification).FirstOrDefault();

                        //InspectionNotification currentInspection = _dbContext.InspectionNotifications.Where(ins => ins.InspectionNotificationId == currentInspectionRevision.RevisionId).Include(ins => ins.Order)
                        //    .Include(ins => ins.Subjects).Include(ins => ins.Types).Include(ins => ins.Locations).Include(ins => ins.Contacts).ThenInclude(cnt => cnt.Contact).FirstOrDefault();

                        NewRevisionInspectionNotification = inspectionNotification.DeepClone();// currentInspection.DeepClone();
                      
                        Revision currentRevision = _dbContext.Revisions.Where(rvs => rvs.RevisionId == dbCirculation.RevisionId).FirstOrDefault();
                        Newrevision.RequestId = currentRevision.RequestId;
                        Newrevision.VersionNumber = currentRevision.VersionNumber;
                        Newrevision.RevisionId = Guid.NewGuid();
                        Newrevision.RevisionNumber = currentRevision.RevisionNumber+1;
                        Newrevision.Comment = "Modified By Vendor to Confirm";
                        Newrevision.Status = "Modified";
                        Newrevision.SubmittedDate = DateTime.Now;
                        Newrevision = _dbContext.Revisions.Add(Newrevision).Entity;
                        _dbContext.SaveChanges();

                        NewRevisionInspectionNotification = _dbContext.InspectionNotifications.Add(NewRevisionInspectionNotification).Entity;

                        InspectionNotificationRevision inspectionNotificationRevision = new InspectionNotificationRevision();
                        inspectionNotificationRevision.InspectionNotification = NewRevisionInspectionNotification;
                        inspectionNotificationRevision.InspectionNotificationId = NewRevisionInspectionNotification.InspectionNotificationId;
                        inspectionNotificationRevision.RevisionId = Newrevision.RevisionId;
                        inspectionNotificationRevision.Revision = Newrevision;

                        _dbContext.InspectionNotificationRevisions.Add(inspectionNotificationRevision);

                    }
                }
                else if (circulation.PerformerContactType == ContactType.PIDEC_CC.GetHashCode() || circulation.PerformerContactType == ContactType.PIDEC_TO.GetHashCode())
                {

                    //Qc should Approve or Reject other users that work on this request , before doing any action on the Request
                    //I mean if QC Approve or Reject this request, other Circulations should be disabled , by completing their ActionTypes

                    //------------When QC_Lead or QC_Manager Backwards inspection Notification---------------
                    //----------- although a new Row will be added to Revisions Table, Revision Number will be Fixed (Not Increased) -----------------------------------------
                    //----------If Vendor do some changes and again send that to stakeholders, in this step a new Row with increased revision will be added to Revisions Table 
                    //----------The Only Activity that causes a version change is "Change" activity---------------------------------------------------------------------------
                    //--------------------------------------------------------------------------------------------------------------------------------------------------------
                    //InspectionNotification currentInspection = _dbContext.InspectionNotifications.Where(ins => ins.RevisionId == dbCirculation.RevisionId).Include(ins => ins.Order)
                    //        .Include(ins => ins.Subjects).Include(ins => ins.Types).Include(ins => ins.Locations).Include(ins => ins.Contacts).ThenInclude(cnt => cnt.Contact).FirstOrDefault();

                    InspectionNotificationRevision currentInspectionRevision = _dbContext.InspectionNotificationRevisions.Where(insR => insR.RevisionId == dbCirculation.RevisionId).Include(insR => insR.InspectionNotification).FirstOrDefault();

                    InspectionNotification currentInspection = _dbContext.InspectionNotifications.Where(ins => ins.InspectionNotificationId == currentInspectionRevision.InspectionNotificationId).Include(ins => ins.Order)
                        .Include(ins => ins.Subjects).Include(ins => ins.Types).Include(ins => ins.Locations).Include(ins => ins.Contacts).ThenInclude(cnt => cnt.Contact).FirstOrDefault();


                    NewRevisionInspectionNotification = currentInspection.DeepClone();

                   

                    Circulation findedCirc;
                    for (int c = 0; c < countOfOtherCirculations; c++)
                    {
                        findedCirc = _dbContext.Circulations.Where(cr => cr.Id == otherCirculationsInCurrentSequence[c].Id && (cr.ActionType ==null  || cr.PerformedActionDateGregorian == null)).FirstOrDefault();
                        if (findedCirc != null && findedCirc.Id != null && findedCirc.Id != Guid.Empty)
                        {
                            findedCirc.QCActionType = circulation.ActionType;
                            if (circulation.ActionType == (int)ActionTypes.BackwardInspectionNotification)
                                findedCirc.Comment = "Backwarded By QC Section as substitution!";
                            else if (circulation.ActionType == (int)ActionTypes.ConfirmInspectionNotification)
                                findedCirc.Comment = "Approved By QC Section as substitution!";
                            else if (circulation.ActionType == (int)ActionTypes.RejectInspectionNotification)
                                findedCirc.Comment = "Rejected By QC Section as substitution!";
                            if(findedCirc.CreatorComment == null)
                                findedCirc.CreatorComment = "";
                            _dbContext.SaveChanges();
                        }
                    }

                    if (circulation.ActionType == ActionTypes.ConfirmInspectionNotification.GetHashCode())
                    {
                        //Send an email to vendor to inform them
                        //maybe a new circulation should create for QC or vendor so that they complete workflow later


                        Revision currentRevision = _dbContext.Revisions.Where(rvs => rvs.RevisionId == dbCirculation.RevisionId).FirstOrDefault();
                        Newrevision.RequestId = currentRevision.RequestId;
                        Newrevision.VersionNumber = currentRevision.VersionNumber;
                        Newrevision.RevisionId = Guid.NewGuid();
                        Newrevision.RevisionNumber = currentRevision.RevisionNumber;
                        Newrevision.Comment = "Created By QC to Confirm";
                        Newrevision.Status = "Confirmed";
                        Newrevision.SubmittedDate = DateTime.Now;
                        Newrevision = _dbContext.Revisions.Add(Newrevision).Entity;
                        _dbContext.SaveChanges();
                    }
                    else if (circulation.ActionType == ActionTypes.RejectInspectionNotification.GetHashCode())
                    {
                        //Send an email to vendor to inform them

                        Revision currentRevision = _dbContext.Revisions.Where(rvs => rvs.RevisionId == dbCirculation.RevisionId).FirstOrDefault();
                        Newrevision.RequestId = currentRevision.RequestId;
                        Newrevision.VersionNumber = currentRevision.VersionNumber;
                        Newrevision.RevisionId = Guid.NewGuid();
                        Newrevision.RevisionNumber = currentRevision.RevisionNumber;
                        Newrevision.Comment = "Created By QC to Reject";
                        Newrevision.Status = "Rejected";
                        Newrevision.SubmittedDate = DateTime.Now;
                        Newrevision = _dbContext.Revisions.Add(Newrevision).Entity;
                        _dbContext.SaveChanges();
                    }
                    else if (circulation.ActionType == ActionTypes.BackwardInspectionNotification.GetHashCode())
                    {

                       NewRevisionInspectionNotification.NotificationNumber = NotificationNumber.CreateFromString(NewRevisionInspectionNotification.NotificationNumber).ToString();

                        Revision currentRevision = _dbContext.Revisions.Where(rvs => rvs.RevisionId == dbCirculation.RevisionId).FirstOrDefault();
                        Newrevision.RequestId = currentRevision.RequestId;
                        Newrevision.VersionNumber = currentRevision.VersionNumber;
                        Newrevision.RevisionId = Guid.NewGuid();
                        Newrevision.RevisionNumber = currentRevision.RevisionNumber;
                        Newrevision.Comment = "Created in Backward process";
                        Newrevision.Status = "Backwarded";
                        Newrevision.SubmittedDate = DateTime.Now;
                        Newrevision.CreatedByCirculationId = dbCirculation.Id;
                        Newrevision = _dbContext.Revisions.Add(Newrevision).Entity;
                        _dbContext.SaveChanges();

                        Circulation NewCirculationForVendor = Circulation.CreateNew(Newrevision, circulation.PerformerContactPerson);
                        NewCirculationForVendor.CreatedBy = circulation.PerformerContactPerson.UserId;
                        int VendorType = (int)ContactType.Vendor;
                        NewCirculationForVendor.PerformerContactPerson = currentInspection.Contacts.Where(cnt => cnt.Contact.ContactType == VendorType).FirstOrDefault().Contact;
                        NewCirculationForVendor.PerformerContactType = VendorType;
                        NewCirculationForVendor.CreatedDate = DateTime.Now;
                        NewCirculationForVendor.SequenceNo = 0;
                        NewCirculationForVendor.PreviousActionType = circulation.ActionType;


                        NewCirculationForVendor = _dbContext.Circulations.Add(NewCirculationForVendor).Entity;
                       // Newrevision.CreatedByCirculationId = NewCirculationForVendor.Id;


                        //Send an email to vendor to inform them
                        //workflow will continue to make new revisions and versions so that finally request be an acceptable version to QC
                        // Circulation nextCirculationforVendor = Circulation.CreateForNextContact(circulation.Request.cre,)
                    }
                                       

                    NewRevisionInspectionNotification = _dbContext.InspectionNotifications.Add(NewRevisionInspectionNotification).Entity;

                    InspectionNotificationRevision inspectionNotificationRevision = new InspectionNotificationRevision();
                    inspectionNotificationRevision.InspectionNotification = NewRevisionInspectionNotification;
                    inspectionNotificationRevision.InspectionNotificationId = NewRevisionInspectionNotification.InspectionNotificationId;
                    inspectionNotificationRevision.RevisionId = Newrevision.RevisionId;
                    inspectionNotificationRevision.Revision = Newrevision;

                    _dbContext.InspectionNotificationRevisions.Add(inspectionNotificationRevision);

                    //NewRevisionInspectionNotification.Revision = Newrevision;
                    //NewRevisionInspectionNotification.RevisionId = Newrevision.RevisionId;

                    


                }
                else if (circulation.PerformerContactType == ContactType.PIDEC_LeadDepartment.GetHashCode() || circulation.PerformerContactType == ContactType.PIDEC_Originator.GetHashCode())
                {
                }
                _dbContext.SaveChanges();
            }
            return await Task.FromResult((NewRevisionInspectionNotification, dbCirculation));
        }
        catch (Exception exp) {

            return (null,null);
        }

        
    }
}
