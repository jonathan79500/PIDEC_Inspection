
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
        Address location, string description):base(inspectionNotificationId)
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
        
        Order = order;
        Location = location;       
        Description = description;

    }


    public InspectionNotificationId InspectionNotificationId { get; set; }
    public Request Request { get; set; }
   // public InspectionNotificationId MainInspectionNotificationId { get; set; }
    public int Version { get; set; }
    public string? Name { get;  set; }= string.Empty;
    public NotificationNumber NotificationNumberParts { get; set; }
    public string NotificationNumber { get;  set; }  
  //  public DateTime? ContractShippingDueDate { get;  set; } = DateTime.Now;
  //  public DateTime? ActualShippingDueDate { get;  set; } = DateTime.Now;
    public DateTime? IssueDate { get;  set; } = DateTime.Now;
    public string? PersianIssueDate { get; set; } = string.Empty;
    public DateTime? RequestInspectionDateStart { get;  set; } = DateTime.Now;
    public DateTime? RequestInspectionDateEnd { get;  set; } = DateTime.Now;
    public string? RequestInspectionPersianDateStart { get;  set; } = string.Empty;
    public string? RequestInspectionPersianDateEnd { get;  set; } = string.Empty;
    public int RequestInspectionDueDate { get;  set; }
    public int RequestInspectionTimesInPeriod { get;  set; }
    public int RequestInspectionPeriodDays { get;  set; } //for emaple we want to do inspection twice each 7 days , so RequestInspectionTimesInPeriod will be 2 and RequestInspectionPeriodDays will be 7 
    //public DateTime? ProposedDate { get;  set; }= DateTime.Now; 
    public Order? Order { get;  set; }
    public Address? Location { get;  set; }
    public string? Description { get;  set; } = string.Empty;

    private List<InspectionNotificationType> _types = new List<InspectionNotificationType>() ;
    private List<InspectionNotificationSubject> _subjects = new List<InspectionNotificationSubject>();  
    private List<Attachment>? _attachments = new List<Attachment>();
    private List<InspectionNotificationContact>? _contacts = new List<InspectionNotificationContact>();
    private List<Circulation>? _circulations = new List<Circulation>();


    public List<Attachment> Attachments
    {
        get => _attachments;
        set => _attachments = value;
    }
    public List<InspectionNotificationContact>? Contacts { get => _contacts; set => _contacts = value; }
    public List<InspectionNotificationSubject>? Subjects { get => _subjects; set => _subjects = value; }
    public List<InspectionNotificationType>? Types { get => _types; set => _types = value; }
    public List<Circulation>? Circulations { get => _circulations; set => _circulations = value; }

    public static InspectionNotification Create(string ?name,NotificationNumber ?notificationNumberParts,string? notificationNumber
        , DateTime? IssueDate,string? PersianIssueDate,DateTime? RequestInspectionDateStart,DateTime? RequestInspectionDateEnd,string? RequestInspectionPersianDateStart,
        string RequestInspectionPersianDateEnd, Order ?order,
        Address ?location, string ?description)
    {
        var inspectionNotification =  new InspectionNotification( InspectionNotificationId.CreateUnique() ,name!, notificationNumberParts!,notificationNumber, IssueDate!.Value, PersianIssueDate, RequestInspectionDateStart!.Value
        ,RequestInspectionDateEnd!.Value, RequestInspectionPersianDateStart!, RequestInspectionPersianDateEnd!, order!, location!,  description!);
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
          ins.Order!,ins.Location!,ins.Description!);

        if(inspectionNotification.InspectionNotificationId != null && inspectionNotification.InspectionNotificationId.Value !=Guid.Empty)
            newinspectionNotification.InspectionNotificationId = inspectionNotification.InspectionNotificationId;
        else
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
                   // newInspectionNotificationContact.Contact = inspectionNotification.Contacts![i].Contact;
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

        Attachment newAttachment;
        if (inspectionNotification.Attachments != null && inspectionNotification.Attachments.Count > 0)
        {
            
            for (int i = 0; i < inspectionNotification.Attachments.Count; i++)
            {
                if (inspectionNotification.Attachments[i].Content == null || inspectionNotification.Attachments[i].Content.Length == 0)
                    inspectionNotification.Attachments.RemoveAt(i);
                else
                {
                    newAttachment = new Attachment();
                    newAttachment.AttachmentId = inspectionNotification.Attachments[i].AttachmentId;
                    newAttachment.Content = inspectionNotification.Attachments[i].Content;
                    newAttachment.Version = inspectionNotification.Attachments[i].Version;
                    newAttachment.Name = inspectionNotification.Attachments[i].Name;
                    newAttachment.PDFSize = inspectionNotification.Attachments[i].PDFSize;
                }
                
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

    public void SetLocation(Address location)
    {
        this.Location = location;
    }

  
   
    public  void AddAttachment(string name, string description, byte[] content,int version,string edmsId,string pdfSize)
    {
        var attachment = Attachment.Create(name, description, content,version,edmsId,pdfSize);          
        _attachments.Add(attachment);
    }

    public void AddAttachment1(Attachment attachment) {
        _attachments.Add(attachment);
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
            clonedInspectionNotification.Version = this.Version;
            clonedInspectionNotification.Description = this.Description;
            clonedInspectionNotification.CreatedBy = this.CreatedBy;
            clonedInspectionNotification.CreatedDate = currentDate;
            clonedInspectionNotification.Version = this.Version;
            clonedInspectionNotification.IssueDate = this.IssueDate;
            clonedInspectionNotification.NotificationNumber = this.NotificationNumber;
            clonedInspectionNotification.NotificationNumberParts = this.NotificationNumberParts;
            clonedInspectionNotification.Order = this.Order;
            clonedInspectionNotification.RequestInspectionDueDate = this.RequestInspectionDueDate;
            clonedInspectionNotification.RequestInspectionDateEnd = this.RequestInspectionDateEnd;
            clonedInspectionNotification.RequestInspectionPersianDateEnd = this.RequestInspectionPersianDateEnd;
            clonedInspectionNotification.RequestInspectionDateStart = this.RequestInspectionDateStart;
            clonedInspectionNotification.RequestInspectionPeriodDays = this.RequestInspectionPeriodDays;
            clonedInspectionNotification.RequestInspectionPersianDateStart = this.RequestInspectionPersianDateStart;
            clonedInspectionNotification.RequestInspectionTimesInPeriod = this.RequestInspectionTimesInPeriod;


            clonedInspectionNotification.Subjects =  this.Subjects.Select(subject =>new InspectionNotificationSubject
            {
                CreatedBy = subject.CreatedBy,
               // InspectionSubject = subject.InspectionSubject,
                CreatedDate = subject.CreatedDate,
                Id = Guid.NewGuid(),
                //InspectionNotification = clonedInspectionNotification,
                InspectionNotificationId = clonedInspectionNotificationId,
                InspectionSubjectId = subject.InspectionSubjectId,
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

            clonedInspectionNotification.Location = this.Location;
          // clonedInspectionNotification.Location.Id = Guid.NewGuid();

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

         
            clonedInspectionNotification.Attachments = this.Attachments.Select(attachment=>new Attachment { 
                AttachmentId = AttachmentId.CreateUnique(),
                Content = attachment.Content,   
                CreatedBy  = attachment.CreatedBy,
                CreatedDate= currentDate,
                Description = attachment.Description,
                edmsId = attachment.edmsId,
                ModifiedBy = attachment.ModifiedBy,
                ModifiedDate = currentDate,
                Name = attachment.Name,
                PDFSize = attachment.PDFSize,
                Version = attachment.Version
            }).ToList();


            return clonedInspectionNotification;

        }
        catch(Exception ex) 
        {
            return null;
        }
    //{
    //    try
    //    {
    //        var json = JsonSerializer.Serialize(this);
    //        return JsonSerializer.Deserialize<InspectionNotification>(json);
    //    }catch (Exception ex)
    //    {
    //        return new InspectionNotification();
    //    }
    }
}
