
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using Inspection.Domain.Entities.Orders;
using System.Collections.Generic;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.Contacts;
using System.Net;
using Inspection.Domain.Entities.ValueObjects;
using System.Text.Json;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Inspection.Domain.Entities.Inspector;
//using Pidec.Utils;

namespace Inspection.Domain.Entities.InspectionNotification.Entities;

public sealed class InspectionNotification:AggregateRoot<InspectionNotificationId>
{
    public InspectionNotification()
    {
        
    }

    private InspectionNotification(InspectionNotificationId inspectionNotificationId) : base(inspectionNotificationId)
    {
        InspectionNotificationId = inspectionNotificationId;
    }

    private InspectionNotification (InspectionNotificationId inspectionNotificationId,string name, NotificationNumber notificationNumberParts,string notificationNmber
        , DateTime issueDate, string persianIssueDate, DateTime requestInspectionDateStart, DateTime requestInspectionDateEnd, string requestInspectionPersianDateStart,
        string requestInspectionPersianDateEnd, Order order,
         string description, string inspectionTimeComment,string equipmentTagNo,int? requestInspectionDueDate, string? orderName,string? inspectionAction) :base(inspectionNotificationId)
    { 
        InspectionNotificationId = inspectionNotificationId;
        Name = name;
        NotificationNumberParts = notificationNumberParts;
        NotificationNumber = notificationNmber;
        IssueDate = issueDate;
        PersianIssueDate = persianIssueDate;
        RequestInspectionDateStart = requestInspectionDateStart;
        RequestInspectionDateEnd = requestInspectionDateEnd;
        RequestInspectionPersianDateEnd = requestInspectionPersianDateEnd;
        RequestInspectionPersianDateStart = requestInspectionPersianDateStart;
        InspectionTimeComment = inspectionTimeComment;
        Order = order;        
        OrderName = orderName;
        EquipmentTagNo = equipmentTagNo;
        Description = description;
        RequestInspectionDueDate = requestInspectionDueDate;
        InspectionAction = inspectionAction;
    }


    public InspectionNotificationId InspectionNotificationId { get; set; }
    //public Guid? RevisionId { get; set; }
    //public Revision? Revision { get; set; } = null!;   
    public string? Name { get;  set; }= string.Empty;
    public string? OrderName { get; set; } = string.Empty;
    public string? EquipmentTagNo { get; set; } = string.Empty;
    public NotificationNumber NotificationNumberParts { get; set; }
    public string NotificationNumber { get;  set; }  
    public DateTime? IssueDate { get;  set; } = DateTime.Now;
    public string? PersianIssueDate { get; set; } = string.Empty;
    public DateTime? RequestInspectionDateStart { get;  set; } = DateTime.Now;
    public DateTime? RequestInspectionDateEnd { get;  set; } = DateTime.Now;
    public string? RequestInspectionPersianDateStart { get;  set; } = string.Empty;
    public string? RequestInspectionPersianDateEnd { get;  set; } = string.Empty;
    public int? RequestInspectionDueDate { get;  set; }
    public string? InspectionTimeComment { get; set; } = string.Empty;
    public string? InspectionAction { get; set; } = string.Empty;
    public string? InspectionActionComment { get; set; } = string.Empty;
    //public int? RequestInspectionTimesInPeriod { get;  set; }
    //public int? RequestInspectionPeriodDays { get;  set; } //for emaple we want to do inspection twice each 7 days , so RequestInspectionTimesInPeriod will be 2 and RequestInspectionPeriodDays will be 7 
    public Order? Order { get;  set; }
   
    public string? Description { get;  set; } = string.Empty;

    private List<InspectionNotificationLocation>? _locations = new List<InspectionNotificationLocation>();
    private List<InspectionNotificationType> _types = new List<InspectionNotificationType>() ;
    private List<InspectionNotificationSubject> _subjects = new List<InspectionNotificationSubject>();  
    private List<InspectionNotificationContact>? _contacts = new List<InspectionNotificationContact>();
    private List<InspectionNotificationRevision>? _revisions = new List<InspectionNotificationRevision>();
    private List<InspectionNotificationInspector>? _inspectors = new List<InspectionNotificationInspector>();

    public List<InspectionNotificationLocation>? Locations { get => _locations; set => _locations = value; }
    public List<InspectionNotificationContact>? Contacts { get => _contacts; set => _contacts = value; }
    public List<InspectionNotificationSubject>? Subjects { get => _subjects; set => _subjects = value; }
    public List<InspectionNotificationType>? Types { get => _types; set => _types = value; }
    public List<InspectionNotificationRevision>? Revisions { get => _revisions; set => _revisions = value; }
    public List<InspectionNotificationInspector>? Inspectors { get => _inspectors; set => _inspectors = value; }

    public static InspectionNotification Create(string ?name,NotificationNumber ?notificationNumberParts,string? notificationNumber
       , DateTime? IssueDate,string? PersianIssueDate,DateTime? RequestInspectionDateStart,DateTime? RequestInspectionDateEnd,string? RequestInspectionPersianDateStart,
        string RequestInspectionPersianDateEnd, Order ?order,
         string? description,string? inspectionTimeComment,string? equipmentTagNo,int? requestInspectionDueDate,string? orderName,string? inspectionAction)
    {
        var inspectionNotification =  new InspectionNotification( InspectionNotificationId.CreateUnique() ,name!, notificationNumberParts!,notificationNumber, IssueDate!.Value, PersianIssueDate, RequestInspectionDateStart!.Value
        ,RequestInspectionDateEnd!.Value, RequestInspectionPersianDateStart!, RequestInspectionPersianDateEnd!, order!,   description!, inspectionTimeComment!,equipmentTagNo!,requestInspectionDueDate!,orderName!,inspectionAction!);
        inspectionNotification.RaiseEvent(new InspectionNotificationCreatedEvent(inspectionNotification.InspectionNotificationId));
        return inspectionNotification;
    }

    public static InspectionNotification CreateByUniqueId(InspectionNotification inspectionNotification)
    {
        var ins = inspectionNotification;
        if(ins.Name == null)
            ins.Name = string.Empty;      
        if(ins.RequestInspectionDateStart == null)
            ins.RequestInspectionDateStart = DateTime.Now;
        if (ins.RequestInspectionDateEnd == null)
            ins.RequestInspectionDateEnd = DateTime.Now;      
        if(ins.Description == null)
            ins.Description = string.Empty;
               
        var newinspectionNotification = InspectionNotification.Create(ins.Name!,ins.NotificationNumberParts!,ins.NotificationNumber,ins.IssueDate!,ins.PersianIssueDate,ins.RequestInspectionDateStart!
          ,ins.RequestInspectionDateEnd!,ins.RequestInspectionPersianDateStart,ins.RequestInspectionPersianDateEnd!,
          ins.Order!,ins.Description!,ins.InspectionTimeComment!,ins.EquipmentTagNo,ins.RequestInspectionDueDate,ins.OrderName,ins.InspectionAction);

        //if(inspectionNotification.InspectionNotificationId != null && inspectionNotification.InspectionNotificationId.Value !=Guid.Empty)
        //    newinspectionNotification.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
        //else
            newinspectionNotification.InspectionNotificationId = InspectionNotificationId.CreateUnique();

        InspectionNotificationContact newInspectionNotificationContact;
        //Guid newContactId;
        try
        {
            if (inspectionNotification.Contacts != null && inspectionNotification.Contacts.Count > 0)
            {
                for (int i = 0; i < inspectionNotification.Contacts.Count; i++)
                {
                    newInspectionNotificationContact = new InspectionNotificationContact();
                    newInspectionNotificationContact.InspectionNotificationId = newinspectionNotification.InspectionNotificationId;
                    newInspectionNotificationContact.ContactId = inspectionNotification.Contacts[i].ContactId;
                    //newInspectionNotificationContact.Contact = inspectionNotification.Contacts![i].Contact;
                    newinspectionNotification.Contacts!.Add(newInspectionNotificationContact);
                }
            }
        }catch (Exception ex)
        {

        }
        InspectionNotificationSubject newInspectionNotificationSubject;
        if (inspectionNotification.Subjects != null && inspectionNotification.Subjects.Count > 0)
        {
            for (int i = 0; i < inspectionNotification.Subjects.Count; i++)
            {
                newInspectionNotificationSubject = new InspectionNotificationSubject();
                newInspectionNotificationSubject.InspectionNotificationId = newinspectionNotification.InspectionNotificationId;
                newInspectionNotificationSubject.InspectionSubjectId = inspectionNotification.Subjects[i].InspectionSubjectId;
                // newInspectionNotificationSubject.InspectionSubject = inspectionNotification.Subjects[i].InspectionSubject;
                newInspectionNotificationSubject.ITP_Row_No = inspectionNotification.Subjects[i].ITP_Row_No;
                newinspectionNotification.Subjects!.Add(newInspectionNotificationSubject);
            }
        }

        InspectionNotificationType newInspectionNotificationType;
        if (inspectionNotification.Types != null && inspectionNotification.Types.Count > 0)
        {
            for (int i = 0; i < inspectionNotification.Types.Count; i++)
            {
                newInspectionNotificationType = new InspectionNotificationType();
                newInspectionNotificationType.InspectionNotificationId = newinspectionNotification.InspectionNotificationId;
                newInspectionNotificationType.InspectionTypeId = inspectionNotification.Types[i].InspectionTypeId;
               // newInspectionNotificationType.InspectionType = inspectionNotification.Types[i].InspectionType;
                newinspectionNotification.Types!.Add(newInspectionNotificationType);
            }
        }

        InspectionNotificationLocation newInspectionNotificationLocation;
        Address newLocation;
        if (inspectionNotification.Locations != null && inspectionNotification.Locations.Count > 0)
        {
            for (int i = 0; i < inspectionNotification.Locations.Count; i++)
            {
                newInspectionNotificationLocation = new InspectionNotificationLocation();
                newInspectionNotificationLocation.InspectionNotificationId = newinspectionNotification.InspectionNotificationId;
                
                if (inspectionNotification.Locations[i].AddressId == Guid.Empty || inspectionNotification.Locations[i].AddressId == null)
                {
                    newLocation = new Address();
                    newLocation.Id = Guid.NewGuid();
                    newLocation.ZipCode = inspectionNotification.Locations[i].Location.ZipCode;
                    newLocation.StreetLine2 = inspectionNotification.Locations[i].Location.StreetLine2;
                    newLocation.StreetLine1 = inspectionNotification.Locations[i].Location.StreetLine1;
                    newLocation.City = inspectionNotification.Locations[i].Location.City;
                    newLocation.State = inspectionNotification.Locations[i].Location.State;
                    newLocation.Country = inspectionNotification.Locations[i].Location.Country;
                    newInspectionNotificationLocation.Location = newLocation;
                    newInspectionNotificationLocation.AddressId = newLocation.Id;
                }
                else
                    newInspectionNotificationLocation.AddressId = inspectionNotification.Locations[i].AddressId;               
                
                newinspectionNotification.Locations!.Add(newInspectionNotificationLocation);
            }
        }


        newinspectionNotification.RaiseEvent(new InspectionNotificationCreatedEvent(newinspectionNotification.InspectionNotificationId));

        return newinspectionNotification;
    }

    public static NotificationNumber GenerateNewInspectionNumber(NotificationNumber LastInspectionNumber)
    {
        NotificationNumber newinspectionNotification = new NotificationNumber();
        return newinspectionNotification;
    }

    public void SetInspectionNotificationId(InspectionNotificationId id)
    {
        this.InspectionNotificationId = id;
    }

    public void SetOrder(Order order)
    {
        this.Order = order;
    }

   

    public void AddContact(string name,Address address,string phone,string mobile,string fax, string email,string homepage,string attention,string cc,ContactType contactType,bool isVisible)
    {
        var contact = Contact.Create(name, address, phone, mobile, fax, email, homepage, attention, cc,contactType.GetHashCode(),isVisible);
        _contacts.Add(InspectionNotificationContact.Create(contact,this,contactType));
    }

    public void AddContact(Contact contact,ContactType contactType)
    {
        contact.SetContactType(contactType);
        InspectionNotificationContact inspectionNotificationContact = InspectionNotificationContact.Create(contact, this, contactType);
        this._contacts.Add(inspectionNotificationContact);
    }

    public InspectionNotification DeepClone()
    {

        try {

            string currentDatePersian = "";
            DateTime currentDate = DateTime.Now;
            //PersianDate persianDate = PersianCelendar.GregorianToJalali(currentDate);
            currentDatePersian = "1404/03/11";//persianDate.ToString();

            var clonedInspectionNotification = new InspectionNotification();

            InspectionNotificationId clonedInspectionNotificationId = InspectionNotificationId.CreateUnique();
            clonedInspectionNotification.InspectionNotificationId = clonedInspectionNotificationId;
            clonedInspectionNotification.Id = clonedInspectionNotificationId;
            clonedInspectionNotification.Name = this.Name;     
            clonedInspectionNotification.Description = this.Description;
            clonedInspectionNotification.CreatedBy = this.CreatedBy;
            clonedInspectionNotification.CreatedDate = currentDate;
            clonedInspectionNotification.IssueDate = currentDate;//this.IssueDate;
            clonedInspectionNotification.ModifiedDate = currentDate;
            clonedInspectionNotification.PersianIssueDate = this.PersianIssueDate;
            clonedInspectionNotification.NotificationNumber = this.NotificationNumber;
            clonedInspectionNotification.NotificationNumberParts = this.NotificationNumberParts;
            clonedInspectionNotification.Order = this.Order;
            clonedInspectionNotification.RequestInspectionDueDate = this.RequestInspectionDueDate;
            clonedInspectionNotification.RequestInspectionDateEnd = this.RequestInspectionDateEnd;
            clonedInspectionNotification.RequestInspectionPersianDateEnd = this.RequestInspectionPersianDateEnd;
            clonedInspectionNotification.RequestInspectionDateStart = this.RequestInspectionDateStart;         
            clonedInspectionNotification.RequestInspectionPersianDateStart = this.RequestInspectionPersianDateStart;
            clonedInspectionNotification.InspectionAction = this.InspectionAction;
            clonedInspectionNotification.InspectionActionComment = this.InspectionActionComment;
            clonedInspectionNotification.InspectionTimeComment = this.InspectionTimeComment;
            clonedInspectionNotification.EquipmentTagNo = this.EquipmentTagNo;
            clonedInspectionNotification.OrderName = this.OrderName;
            //clonedInspectionNotification.Revision = this.Revision;
            //clonedInspectionNotification.RevisionId = this.RevisionId;
            
            clonedInspectionNotification.Subjects =  this.Subjects.Select(subject =>new InspectionNotificationSubject
            {
                CreatedBy = subject.CreatedBy,
               // InspectionSubject = subject.InspectionSubject,
                CreatedDate = subject.CreatedDate,
                Id = Guid.NewGuid(),
                //InspectionNotification = clonedInspectionNotification,
                InspectionNotificationId = clonedInspectionNotificationId,
                InspectionSubjectId = subject.InspectionSubjectId,
                ITP_Row_No = subject.ITP_Row_No,
                ModifiedBy = subject.ModifiedBy,
                ModifiedDate = subject.ModifiedDate
            }).ToList();


            clonedInspectionNotification.Types = this.Types.Select(type => new InspectionNotificationType
            {
                CreatedBy = type.CreatedBy,
                //InspectionType = type.InspectionType,
                CreatedDate = type.CreatedDate,
                Id = Guid.NewGuid(),
               // InspectionNotification = clonedInspectionNotification,
                InspectionNotificationId = clonedInspectionNotificationId,
                InspectionTypeId = type.InspectionTypeId,
                ModifiedBy = type.ModifiedBy,
                ModifiedDate = type.ModifiedDate
            }).ToList();

            clonedInspectionNotification.Locations = this.Locations.Select(location => new InspectionNotificationLocation
            {
                CreatedBy = location.CreatedBy,                
                CreatedDate = location.CreatedDate,                               
                InspectionNotificationId = clonedInspectionNotificationId,
                AddressId = location.AddressId,
                Id = Guid.NewGuid(),
                ModifiedBy = location.ModifiedBy,
                ModifiedDate = location.ModifiedDate
            }).ToList();

            clonedInspectionNotification.Contacts = this.Contacts.Select(contact=>new InspectionNotificationContact
            {                
                 //Contact = contact.Contact,
                 ContactId = contact.ContactId,
                 CreatedBy= contact.CreatedBy,
                 CreatedDate= currentDate,
                 //InspectionNotification= clonedInspectionNotification,
                 InspectionNotificationId= clonedInspectionNotificationId,
                 ModifiedBy = contact.ModifiedBy,
                 Id = Guid.NewGuid(),
                 ModifiedDate = currentDate
            }).ToList();

              return clonedInspectionNotification;

        }
        catch(Exception ex) 
        {
            return null;
        }
    
    }
}
