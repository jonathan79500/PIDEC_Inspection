using Azure.Core;
using Dapper;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Models;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.EDMS;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.User.Enums;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
//using Pidec.Utils;

namespace Inspection.Persistence.Repositories;

public class InspectionNotificationRepository : IGenericRepository<InspectionNotification, InspectionNotificationId>, IInspectionNotificationRepository
{

    private readonly InspectionDbContext _dbContext;
    private IServiceScopeFactory _serviceProvider;
    private IEDMSService _eDMSService;
    private IUserManagerRepository _userManagerRepository;

    public InspectionNotificationRepository(InspectionDbContext dbContext, IServiceScopeFactory serviceProvider,IEDMSService eDMSService, IUserManagerRepository userManagerRepository) : base()
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _eDMSService = eDMSService;
        _userManagerRepository = userManagerRepository;
    }

    public async Task<Contact> AddNewContact(Contact contact) {

        if (contact == null) {
            throw new Exception("Contact is null");
        }
        var addedContact = _dbContext.Contacts.Add(contact);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(addedContact.Entity);
    }

    public async Task<Contact> GetContactByUserId(string userId) {
        if (userId == null)
        {
            throw new Exception("UserId Is Null");
        }

        return await Task.FromResult(_dbContext.Contacts.Where(ct => ct.UserId == userId).FirstOrDefault());
    }

    public async Task<InspectionNotification> AddNewInspectionNotification(InspectionNotification inspectionNotification) {

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
        catch (Exception exp) {
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
            transaction:adoTransaction))
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
                    RequestInspectionPeriodDays = ino.RequestInspectionPeriodDays,
                    RequestInspectionPersianDateEnd = ino.RequestInspectionPersianDateEnd,
                    RequestInspectionPersianDateStart = ino.RequestInspectionPersianDateStart,
                    RequestInspectionTimesInPeriod = ino.RequestInspectionTimesInPeriod,
                    NotificationNumberParts = ino.NotificationNumberParts,
                    Version = ino.Version


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
      inspectionNotificationContact.ContactId = contact.Id;
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

    public async Task<InspectionNotification> UpdateInspectionNotification(InspectionNotification inspectionNotification)
    {
        InspectionNotification existingInspectionNotification = _dbContext.InspectionNotifications.Where(ins => ins.InspectionNotificationId == inspectionNotification.InspectionNotificationId).AsNoTracking().FirstOrDefault();
        if (existingInspectionNotification != null)
        {          

                try
                {
                    var inspectionNotificationId = inspectionNotification.InspectionNotificationId; // The parent entity you want to remove relationships for.


                    //------- Clean Up Old Contacts and InspectionNotificationContacts in DataBase-----------------
                    var contactsInInspectionNotificationContact = _dbContext.InspectionNotificationsContacts.Where(c => c.InspectionNotificationId == inspectionNotificationId)
                    .Select(isc => isc.ContactId)
                    .Distinct()
                    .ToList();

                    var existedInspectionNotificationContacts = _dbContext.InspectionNotificationsContacts.Where(c => c.InspectionNotificationId == inspectionNotificationId).ToList();
                    _dbContext.InspectionNotificationsContacts.RemoveRange(existedInspectionNotificationContacts);

                    //----Then I  cleanup database from contacts that are not used at the moment
                    //----at first find that contacts are used in which tables-----------------
                    //----Then search which contacts are not referenced in other tables and delete them---------
                    var foreignKeys = _dbContext.Model.FindEntityType(typeof(Contact))?
                    .GetReferencingForeignKeys();

                    var contactsUsedElsewhere = new HashSet<Guid>();

                    foreach (var fk in foreignKeys)
                    {
                        Type relatedTable = fk.DeclaringEntityType.ClrType;

                        // Use reflection to get DbSet dynamically
                        var method = typeof(DbContext).GetMethod(nameof(DbContext.Set), Array.Empty<Type>());
                        var genericMethod = method.MakeGenericMethod(relatedTable);
                        var tableSet = genericMethod.Invoke(_dbContext, null) as IQueryable;


                        var usedContactIds = tableSet
                         .Cast<object>()
                         .AsEnumerable() // Force the remainder of the query to be executed in memory
                         .Select(entity =>
                         {
                             if (fk.Properties.First().PropertyInfo != null)
                             {
                                 var value = fk.Properties.First().PropertyInfo!.GetValue(entity);
                                 if (value == null)
                                 {
                                     return Guid.Empty;
                                 }
                                 else
                                 {
                                     // Explicitly cast to ContactId and get its Guid value.
                                     // Adjust .Value if your ContactId type exposes the Guid differently.
                                     return ((ContactId)value).Value;
                                 }
                             }
                             else
                             {
                                 return Guid.Empty;

                             }
                         }).Distinct().ToList();

                        contactsUsedElsewhere.UnionWith(usedContactIds);
                    }

                    var orphanedContacts = contactsInInspectionNotificationContact
                    .Where(cid => !contactsUsedElsewhere.Contains(cid))
                    .ToList();

                    // Convert your orphaned value objects to their underlying Guid values.
                    var orphanedContactGuids = orphanedContacts.Select(x => x.Value).ToList();

                    // Switch to client evaluation by calling AsEnumerable() so that the .Value accessor is evaluated in C#.
                    var contactsToRemove = _dbContext.Contacts
                        .AsEnumerable() // This forces the remainder of the query to run in memory.
                        .Where(c => orphanedContactGuids.Contains(c.ContactId.Value))
                        .ToList();

                    _dbContext.Contacts.RemoveRange(contactsToRemove);

                    //---------------End Cleanup Contacts and InspectionNotificationContacts---------------------------------

                    //------- Clean Up Old InspectionSubjects and InspectionNotificationSubjects in DataBase-----------------
                    var subjectsInInspectionNotificationSubjects = _dbContext.InspectionNotificationSubjects.Where(c => c.InspectionNotificationId == inspectionNotificationId)
                    .Select(iss => iss.InspectionSubjectId)
                    .Distinct()
                    .ToList();

                    var existedInspectionNotificationSubjects = _dbContext.InspectionNotificationSubjects.Where(c => c.InspectionNotificationId == inspectionNotificationId).ToList();
                    _dbContext.InspectionNotificationSubjects.RemoveRange(existedInspectionNotificationSubjects);

                    //----Then I  cleanup database from InspectionSubjects that are not used at the moment
                    //----at first find that InspectionSubjects are used in which tables-----------------
                    //----Then search which InspectionSubjects are not referenced in other tables and delete them---------
                    var SubjectforeignKeys = _dbContext.Model.FindEntityType(typeof(InspectionSubject))?
                    .GetReferencingForeignKeys();

                    var inspectionSubjectsUsedElsewhere = new HashSet<Guid>();

                    foreach (var fk in SubjectforeignKeys)
                    {
                        Type relatedTable = fk.DeclaringEntityType.ClrType;

                        // Use reflection to get DbSet dynamically
                        var method = typeof(DbContext).GetMethod(nameof(DbContext.Set), Array.Empty<Type>());
                        var genericMethod = method.MakeGenericMethod(relatedTable);
                        var tableSet = genericMethod.Invoke(_dbContext, null) as IQueryable;


                        var usedInspectionSubjectIds = tableSet
                         .Cast<object>()
                         .AsEnumerable() // Force the remainder of the query to be executed in memory
                         .Select(entity =>
                         {
                             var value = fk.Properties.First().PropertyInfo.GetValue(entity);
                             if (value == null)
                             {
                                 return Guid.Empty;
                             }
                             else
                             {
                                 // Explicitly cast to ContactId and get its Guid value.
                                 // Adjust .Value if your ContactId type exposes the Guid differently.
                                 return ((InspectionSubjectId)value).Value;
                             }
                         }).Distinct().ToList();

                        inspectionSubjectsUsedElsewhere.UnionWith(usedInspectionSubjectIds);
                    }

                    var orphanedInspectionSubjects = subjectsInInspectionNotificationSubjects
                    .Where(cid => !inspectionSubjectsUsedElsewhere.Contains(cid))
                    .ToList();

                    // Convert your orphaned value objects to their underlying Guid values.
                    var orphanedInspectionSubjectsGuids = orphanedInspectionSubjects.Select(x => x.Value).ToList();

                    // Switch to client evaluation by calling AsEnumerable() so that the .Value accessor is evaluated in C#.
                    var inspectionSubjectsToRemove = _dbContext.InspectionSubjects
                        .AsEnumerable() // This forces the remainder of the query to run in memory.
                        .Where(c => orphanedInspectionSubjectsGuids.Contains(c.InspectionSubjectId.Value))
                        .ToList();

                    _dbContext.InspectionSubjects.RemoveRange(inspectionSubjectsToRemove);
                    //---------------End Cleanup InspectionSubjects and InspectionNotificationSubjects---------------------------------

                    //------- Clean Up Old InspectionTypes and InspectionNotificationTypes in DataBase-----------------
                    var typesInInspectionNotificationTypes = _dbContext.InspectionNotificationsTypes.Where(c => c.InspectionNotificationId == inspectionNotificationId)
                    .Select(isc => isc.InspectionTypeId)
                    .Distinct()
                    .ToList();

                    var existedInspectionNotificationTypes = _dbContext.InspectionNotificationsTypes.Where(c => c.InspectionNotificationId == inspectionNotificationId).ToList();
                    _dbContext.InspectionNotificationsTypes.RemoveRange(existedInspectionNotificationTypes);

                    //----Then I  cleanup database from contacts that are not used at the moment
                    //----at first find that contacts are used in which tables-----------------
                    //----Then search which contacts are not referenced in other tables and delete them---------
                    var typesforeignKeys = _dbContext.Model.FindEntityType(typeof(InspectionType))?
                    .GetReferencingForeignKeys();

                    var inspectionTypesUsedElsewhere = new HashSet<Guid>();

                    foreach (var fk in typesforeignKeys)
                    {
                        Type relatedTable = fk.DeclaringEntityType.ClrType;

                        // Use reflection to get DbSet dynamically
                        var method = typeof(DbContext).GetMethod(nameof(DbContext.Set), Array.Empty<Type>());
                        var genericMethod = method.MakeGenericMethod(relatedTable);
                        var tableSet = genericMethod.Invoke(_dbContext, null) as IQueryable;


                        var usedInspectionTypeIds = tableSet
                         .Cast<object>()
                         .AsEnumerable() // Force the remainder of the query to be executed in memory
                         .Select(entity =>
                         {
                             var value = fk.Properties.First().PropertyInfo.GetValue(entity);
                             if (value == null)
                             {
                                 return Guid.Empty;
                             }
                             else
                             {
                                 // Explicitly cast to ContactId and get its Guid value.
                                 // Adjust .Value if your ContactId type exposes the Guid differently.
                                 return ((InspectionTypeId)value).Value;
                             }
                         }).Distinct().ToList();

                        inspectionTypesUsedElsewhere.UnionWith(usedInspectionTypeIds);
                    }

                    var orphanedInspectionTypes = typesInInspectionNotificationTypes
                    .Where(cid => !inspectionTypesUsedElsewhere.Contains(cid))
                    .ToList();

                    // Convert your orphaned value objects to their underlying Guid values.
                    var orphanedInspectionTypeGuids = orphanedInspectionTypes.Select(x => x.Value).ToList();

                    // Switch to client evaluation by calling AsEnumerable() so that the .Value accessor is evaluated in C#.
                    var inspectionTypesToRemove = _dbContext.InspectionTypes
                        .AsEnumerable() // This forces the remainder of the query to run in memory.
                        .Where(c => orphanedInspectionTypeGuids.Contains(c.InspectionTypeId.Value))
                        .ToList();

                    _dbContext.InspectionTypes.RemoveRange(inspectionTypesToRemove);

                    _dbContext.SaveChanges();
                    //---------------End Cleanup Contacts and InspectionNotificationContacts---------------------------------


                


                    //--------------Now We can Update InspectionNotification with new Data----------------------

                  
                            try
                            {


                                var existing = await _dbContext.InspectionNotifications
                       //.AsSplitQuery()                     // <<–– tell EF “split the includes”
                       //.AsNoTracking()
                      //.Include(i => i.Order)
                      //  .ThenInclude(o => o.Project)
                      //.Include(i => i.Order)                        
                      //.Include(i => i.Contacts)                          
                      //.Include(i => i.Subjects)
                      //.Include(i => i.Types)
                      .FirstOrDefaultAsync(i =>
                          i.InspectionNotificationId == inspectionNotification.InspectionNotificationId);

                                SyncContacts(existing, inspectionNotification, _dbContext);
                                SyncSubjects(existing, inspectionNotification, _dbContext);
                                SyncTypes(existing, inspectionNotification,_dbContext);

                                if (existing.Circulations != null && existing.Circulations.Count > 0)
                                {
                                    //before this time , Request and circulation are created
                                }
                                else
                                {

                                    Contact creator = existing.Contacts!.Where((contact) => contact.Contact.ContactType == ContactType.Vendor.GetHashCode()).FirstOrDefault()!.Contact;
                                    string currentDatePersian = "";
                                    DateTime currentTime = DateTime.Now;
                                 

                                   // Domain.Entities.Common.Request newRequest = Domain.Entities.Common.Request.CreateNew(creator);

                                    Circulation circulation = Circulation.CreateNew(existingInspectionNotification, creator);
                                    //circulation.InspectionNotification = inspectionNotification;
                                   // circulation.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                                    circulation.PerformerContactType = creator.ContactType;
                                    circulation.CreatedDate = currentTime;
                                    circulation.PerformedActionDateGregorian = currentTime;
                                    circulation.PerformedActionDate = "";// persianDate.ToString();
                                    existing.Circulations!.Add(circulation);
                                }

                                _dbContext.SaveChangesAsync();
                             
                                return existing;
                            }
                            catch (Exception ex)
                            {
                             
                                return null;
                            }                  
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            //}
        }
        return inspectionNotification;
    }

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
                    Attention = inc.Contact.Attention,
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
                    ContactId = contactEntity.ContactId
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
                    InspectionSubjectId = typeEntity.InspectionSubjectId
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
            var typeEntity =  db.InspectionTypes
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
                    InspectionTypeId = typeEntity.InspectionTypeId
                });
            }
            else
            {
                // neither an existing nor a new nav-prop was provided:
                // decide if you want to throw or just skip
            }
        }
    }



    public async Task<InspectionNotification> CreateNewInspectionNotification(InspectionNotification rawinspectionNotification)
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


            var currentLocation = _dbContext.Entry(inspectionNotification.Location!).Entity;
                if (await _dbContext.Addresses.AnyAsync(a =>
                     a.ZipCode == currentLocation.ZipCode && a.Country == currentLocation.Country && a.City == currentLocation.City && a.State == currentLocation.State
                     && a.StreetLine1 == currentLocation.StreetLine1))
                {
                    var location = _dbContext.Addresses.Where(a =>
                     a.ZipCode == currentLocation.ZipCode && a.Country == currentLocation.Country && a.City == currentLocation.City && a.State == currentLocation.State
                     && a.StreetLine1 == currentLocation.StreetLine1).First();

                    inspectionNotification.SetLocation(location);

                    _dbContext.Entry(inspectionNotification.Location!).State = EntityState.Unchanged;
                }
                else
                    _dbContext.Entry(inspectionNotification.Location!).State = EntityState.Added;

                //  _dbContext.Entry(inspectionNotification.Location!).State = EntityState.Unchanged;


              



                foreach (InspectionNotificationContact inscontact in inspectionNotification.Contacts!.ToList())
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
                        if (existingInsContact != null)
                            _dbContext.Entry(inscontact).State = EntityState.Unchanged;
                        else
                            _dbContext.Entry(inscontact).State = EntityState.Added;
                    }
                    else if (inscontact.Contact != null)
                    {
                    // inscontact.InspectionNotificationId = inspectionNotification.Id;
                    InspectionNotificationContact findedContact = rawinspectionNotification.Contacts.Find((cnt) => cnt.ContactId == inscontact.ContactId);

                    if (findedContact != null)
                    {
                        //_dbContext.InspectionSubjects.Add(findedSubject.InspectionSubject);
                        inscontact.Contact = findedContact.Contact;
                    }

                    _dbContext.Entry(inscontact.Contact).State = EntityState.Added;
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

                    if(findedSubject != null) 
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

            


                var entries = _dbContext.ChangeTracker.Entries().ToList();
                foreach (var entry in entries)
                {
                    Console.WriteLine($"{entry.Entity.GetType().Name} - {entry.State}");
                }

                if (inspectionNotification.Circulations != null && inspectionNotification.Circulations.Count > 0)
                {
                    //before this time , Request and circulation are created
                }
                else
                {

        

                Contact creator = inspectionNotification.Contacts!.Where((contact) => contact.Contact.ContactType == ContactType.Vendor.GetHashCode()).FirstOrDefault()!.Contact;
                    string currentDatePersian = "";
                    DateTime currentTime = DateTime.Now;                   

                    Domain.Entities.Common.Request newRequest = Domain.Entities.Common.Request.CreateNew(creator);

                 Domain.Entities.Common.Request savedRequest =  _dbContext.Requests.Add(newRequest).Entity;

                inspectionNotification.Request = savedRequest;

                InspectionNotification savedNewInspection = _dbContext.InspectionNotifications.Add(inspectionNotification).Entity;

                Circulation circulation = Circulation.CreateNew(inspectionNotification,creator);
                    //circulation.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
                    circulation.PerformerContactType = creator.ContactType;
                    circulation.CreatedDate = currentTime;
                    circulation.PerformedActionDateGregorian = currentTime;
                    circulation.PerformedActionDate = "";// persianDate.ToString();
                    inspectionNotification.Circulations!.Add(circulation);

                _dbContext.SaveChanges();

                foreach (Attachment attachment in inspectionNotification.Attachments!.ToList())
                {

                   // var existingAttachment = await _dbContext.Attachments.FirstOrDefaultAsync(atc => atc.Id.Value == attachment.Id.Value);
                    var existingAttachment = await _dbContext.Attachments.FirstOrDefaultAsync(a => a.Id == attachment.Id);


                    CirculationAttachments newCirculationAttachment = new CirculationAttachments();
                    newCirculationAttachment.CirculationId = circulation.Id;
                    _dbContext.Entry(circulation).State = EntityState.Unchanged;

                    if (existingAttachment != null)
                    {
                        newCirculationAttachment.AttachmentId = existingAttachment.AttachmentId;                        
                    }
                    else
                    {
                        //Attachment newAttachment = new Attachment(attachment.AttachmentId);
                        Attachment newAttachment = Attachment.Create(attachment.Name, attachment.Description, attachment.Content, attachment.Version, "-1", "0");
                       // newAttachment = _dbContext.Attachments.Add(newAttachment).Entity;
                        newCirculationAttachment.AttachmentId = newAttachment.AttachmentId;
                        
                    }
                    _dbContext.CirculationAttachments.Add(newCirculationAttachment);
                }

            }


                _dbContext.SaveChanges();
              
                return await Task.FromResult(inspectionNotification);

            }
            catch (Exception ex)
            {
              
                string str = ex.Message;
            }    


        return null;
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
                LeadDepartmentContact.Attention = "";
                LeadDepartmentContact.Address = null;
                LeadDepartmentContact.Phone = "";
                LeadDepartmentContact.Email = eDMSOrderDepartmentLeads.leadEngineer.Trim() + "@pidec.com";
                LeadDepartmentContact.Id = Guid.NewGuid();
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
                (DepartmentleadUser,DeptLeadResetPasswordLink) = _userManagerRepository.RegisterNewUser(DepartmentleadUser,RoleNames).Result;

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
                OriginatorContact.Attention = "";
                OriginatorContact.Address = null;
                OriginatorContact.Phone = "";
                OriginatorContact.Mobile = "";
                OriginatorContact.Email = eDMSOrderDepartmentLeads.originator.Trim() + "@pidec.com";
                OriginatorContact.Id = Guid.NewGuid();
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

                (originatorUser,DeptOriginatorResetPasswordLink) = _userManagerRepository.RegisterNewUser(originatorUser,RoleNames).Result;
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

    public async Task<InspectionNotification> SendNewInspectionNotification(InspectionNotification inspectionNotification,Project project)
    {
        InspectionNotification notificationForSend = null;
       
        string  newRevisedNumber = GenerateNewNotificationNumber(project.Number, inspectionNotification.Order.Name).Result.ToString();
        inspectionNotification.NotificationNumber = newRevisedNumber;      
       
        try
            {

                if (inspectionNotification.InspectionNotificationId != null && !inspectionNotification.InspectionNotificationId.Value.Equals(Guid.Empty))
                {
                    notificationForSend = UpdateInspectionNotification(inspectionNotification).Result;                 
                }
                else
                    notificationForSend = CreateNewInspectionNotification(inspectionNotification).Result;

           // var simulatedVersion = SimulateInpectionDataForNextUsers(notificationForSend,project);

            await UpdateNotificationNumber(NotificationNumber.CreateFromString(notificationForSend!.NotificationNumber!));
        }
            catch (Exception exp)
            {
              
            }         

        return await Task.FromResult(inspectionNotification);     
    }



    public Task<InspectionNotification> GetInspectionNotificationById(InspectionNotificationId inspectionNotificationId)
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
            List<InspectionNotificationType> types;
            List<Circulation> participatedCirculations;

            using (var multi = connection.QueryMultiple(
            "GetInspectionNotification_Id1",
            new { InspectionNotificationId = inspectionNotificationId.Value },
            commandType: CommandType.StoredProcedure,
            transaction: adoTransaction))
            {
                inspectionNotifications = multi.Read<InspectionNotification>().ToList();
                //participatedCirculations = multi.Read<Circulation>().ToList();

                participatedCirculations = multi.Read<Circulation, Contact, Inspection.Domain.Entities.InspectionNotification.Entities.InspectionNotification, Circulation>(
                  (cr, ct,rq) => { cr.PerformerContactPerson = ct; cr.InspectionNotification = rq; return cr; },
                  splitOn: "ContactId,RequestId"
              ).ToList();

                orders = multi.Read<Order>().ToList();
                 contacts = multi.Read<InspectionNotificationContact, Contact, InspectionNotificationContact>(
                    (insc, cnt) => { insc.Contact = cnt; return insc; },
                    splitOn: "ContactId"
                ).ToList();
                 subjects = multi.Read<InspectionNotificationSubject, InspectionSubject, InspectionNotificationSubject>(
                    (inss, ins) => { inss.InspectionSubject = ins; return inss; },
                    splitOn: "InspectionSubjectId"
                ).ToList();
                types = multi.Read<InspectionNotificationType, InspectionType, InspectionNotificationType>(
                    (inst, inty) => { inst.InspectionType = inty; return inst; },
                    splitOn: "InspectionTypeId"
                ).ToList();
            }

            if(inspectionNotifications.Count>0 && inspectionNotifications[0] != null)
            {
                inspectionNotifications[0].Order = orders[0];
                inspectionNotifications[0].Contacts = contacts;
                inspectionNotifications[0].Subjects = subjects;
                inspectionNotifications[0].Types = types;
                inspectionNotifications[0].Circulations = participatedCirculations;
            }
            InspectionNotification result = inspectionNotifications[0];
            return Task.FromResult(result);
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


                inspectionNotifications = multi.Read<InspectionNotification, Order,Circulation,InspectionNotificationContact, Contact, InspectionNotification >(
(inspectionNotification, order,circulation,inspectionNotificationContact, contact) =>
{
    inspectionNotification.Id = inspectionNotification.InspectionNotificationId.Value;
    inspectionNotification.NotificationNumberParts = NotificationNumber.CreateFromString(inspectionNotification.NotificationNumber);
    if(inspectionNotification.Location == null)
        inspectionNotification.Location = new Domain.Entities.Common.ValueObjects.Address();
    
    inspectionNotification.Order = order;
    inspectionNotification.Circulations!.Add(circulation);

    // In case notificationContact exists, attach the contact from the Contacts table.
    if (inspectionNotificationContact != null)
    {
        inspectionNotificationContact.Contact = contact;
        inspectionNotificationContact.Contact.Id = contact.ContactId!.Value;
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

            InspectionNotification result=  inspectionNotifications[0];
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
            "GetDraftedInspectionNotifications_UserName",
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
            "GetSentInspectionNotifications_UserName",
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

    public Task<List<InspectionNotification>> GetInspectionNotificationsByProjUsrIds(string ProjectId, string UserId)
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
                    RequestInspectionPeriodDays = ins.RequestInspectionPeriodDays,
                    RequestInspectionPersianDateEnd = ins.RequestInspectionPersianDateEnd,
                    RequestInspectionPersianDateStart = ins.RequestInspectionPersianDateStart,
                    RequestInspectionTimesInPeriod = ins.RequestInspectionTimesInPeriod,
                    Version = ins.Version,
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
                        inspectionNotificationContact.ContactId = contact.Id;
                        inspectionNotificationContact.Id = inspectionNotificationContact.Id;
                        return inspectionNotificationContact;
                    },
                  splitOn: "ContactId"
                ).ToList();


                

            }

            //connection.Close();


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
         .Include(n => n.Location)

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
            .Include(n => n.Location)

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
        NotificationNumber notificationNumber = _dbContext.NotificationNumbers.Where(nn => nn.ProjectNumber == projectNumber && nn.OrderNumber == orderNumber).FirstOrDefault()!;

        if (notificationNumber?.Id != null)
        {
            // return Task.FromResult(notificationNumber.CreateNewVersion());
            return await Task.FromResult(notificationNumber.IncreaseVersion());
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

    public async Task<Circulation> CompleteCirculation(Circulation circulation)    {
        // Circulation dbCirculation = _dbContext.Circulations.Where(cr=>cr.Id  == circulation.Id).Include(cr=>cr.InspectionNotification).FirstOrDefault();
        var dbCirculation = await _dbContext.Circulations
       .Include(cr => cr.InspectionNotification)
       .FirstOrDefaultAsync(cr => cr.Id == circulation.Id);

         List<Circulation> otherCirculationsInCurrentSequence = new List<Circulation>();
        if (dbCirculation == null)
            throw new InvalidOperationException("Base circulation not found");

        otherCirculationsInCurrentSequence = _dbContext.Circulations.Where(cr=>cr.SequenceNo == circulation.SequenceNo && cr.Id != circulation.Id
        && cr.InspectionNotification.InspectionNotificationId == dbCirculation.InspectionNotification.InspectionNotificationId).ToList();
        int countOfOtherCirculations = otherCirculationsInCurrentSequence.Count;
        if (dbCirculation != null) {
            
            //----------anyway the circulation will be complete
            dbCirculation.PerformedActionDateGregorian = DateTime.UtcNow;
            dbCirculation.ModifiedDate = DateTime.UtcNow;
            dbCirculation.Comment = (circulation.Comment==null || circulation.Comment.Trim() == "") ?  "No Comment" : circulation.Comment;
            dbCirculation.ActionType = circulation.ActionType;
            dbCirculation.QCActionType = circulation.ActionType;
            dbCirculation.PerformedActionDateGregorian = DateTime.UtcNow;

            //_dbContext.SaveChanges();
            await _dbContext.SaveChangesAsync();

            if (circulation.PerformerContactType == ContactType.PIDEC_CC.GetHashCode() || circulation.PerformerContactType == ContactType.PIDEC_TO.GetHashCode())
            {

                //Qc should Approve or Reject other users that work on this request , before doing any action on the Request
                //I mean if QC Approve or Reject this request, other Circulations should be disabled , by completing their ActionTypes

                //var ids = otherCirculationsInCurrentSequence.Select(x => x.Id).ToArray();

                //var query = _dbContext.Circulations.Where(cr => ids.Contains(cr.Id));
                //var sql = query.ToQueryString(); // EF Core 5+
                //Console.WriteLine(sql);

                //var circulationsToUpdate = await _dbContext.Circulations
                //    .Where(cr => ids.Contains(cr.Id))
                //    .ToListAsync();

                //if (circulationsToUpdate.Any())
                //{
                //    foreach (var circ in circulationsToUpdate)
                //    {
                //        circ.QCActionType = circulation.ActionType;
                //        circ.Comment = "Approved By QC Section as substitution";
                //    }

                //    await _dbContext.SaveChangesAsync();
                //}

                Circulation findedCirc;
                for (int c = 0; c < countOfOtherCirculations; c++)
                {
                    findedCirc = _dbContext.Circulations.Where(cr => cr.Id == otherCirculationsInCurrentSequence[c].Id).FirstOrDefault();
                    findedCirc.QCActionType = circulation.ActionType;// otherCirculationsInCurrentSequence[c].QCActionType;
                    findedCirc.Comment = "Approved By QC Section as substitution";
                    _dbContext.SaveChanges();
                }

                if (circulation.ActionType == ActionTypes.ConfirmInspectionNotification.GetHashCode())
                {
                   //Send an email to vendor to inform them
                   //maybe a new circulation should create for QC or vendor so that they complete workflow later
                }
                else if (circulation.ActionType == ActionTypes.RejectInspectionNotification.GetHashCode())
                {
                    //Send an email to vendor to inform them
                }
                else if (circulation.ActionType == ActionTypes.BackwardInspectionNotification.GetHashCode())
                {
                    //Send an email to vendor to inform them
                    //workflow will continue to make new revisions and versions so that finally request be an acceptable version to QC
                   // Circulation nextCirculationforVendor = Circulation.CreateForNextContact(circulation.Request.cre,)
                }
            }
            else if (circulation.PerformerContactType == ContactType.PIDEC_LeadDepartment.GetHashCode() || circulation.PerformerContactType == ContactType.PIDEC_Originator.GetHashCode())
            {            
            }
           // _dbContext.SaveChanges();
            }

        return circulation;
        }
}
