
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace Inspection.Domain.Entities.InspectionNotification.Entities
{
    public sealed class InspectionNotification:AggregateRoot<InspectionNotificationId>
    {
        private InspectionNotification()
        {
            
        }

        private InspectionNotification(InspectionNotificationId inspectionNotificationId) : base(inspectionNotificationId)
        {
            InspectionNotificationId = inspectionNotificationId;
        }

        private InspectionNotification (InspectionNotificationId inspectionNotificationId,string name, NotificationNumber notificationNumber, string inspectionSubject,
            DateTime contractShippingDueDate, DateTime actualShippingDueDate, DateTime proposedDate, Vendor owner, Order order,
            Address location, InspectionType inspectionType, string description):base(inspectionNotificationId)
        { 
            InspectionNotificationId = inspectionNotificationId;
            Name = name;
            NotificationNumber = notificationNumber;
            InspectionSubject = inspectionSubject;          
            ContractShippingDueDate = contractShippingDueDate;
            ActualShippingDueDate = actualShippingDueDate;
            ProposedDate = proposedDate;
            Owner = owner;
            Order = order;
            Location = location;
            InspectionType = inspectionType;
            Description = description;

        }


        public InspectionNotificationId InspectionNotificationId { get; private set; }
        public string Name { get; private set; }
        public NotificationNumber NotificationNumber { get; private set; }
        public string InspectionSubject { get; private set; }
        public DateTime? ContractShippingDueDate { get; private set; }
        public DateTime? ActualShippingDueDate { get; private set; }
        public DateTime? ProposedDate { get; private set; }
        public Vendor Owner { get; private set; }
        public Order? Order { get; private set; }
        public Address? Location { get; private set; }
        public InspectionType? InspectionType { get; private set; }
        public string? Description { get; private set; }
        private readonly List<Attachment>? _attachments= new();

        public readonly List<Contact>? _contacts = new();


        public IReadOnlyList<Attachment>? Attachments => _attachments!.AsReadOnly();
        public IReadOnlyList<Contact>? Contacts => _contacts!.AsReadOnly();

        public static InspectionNotification Create(string name,NotificationNumber notificationNumber,string inspectionSubject,            
            DateTime contractShippingDueDate, DateTime actualShippingDueDate,DateTime proposedDate, Vendor owner, Order order,
            Address location, InspectionType inspectionType, string description)
        { 
            var inspectionNotification =  new InspectionNotification( InspectionNotificationId.CreateUnique() ,name, notificationNumber, inspectionSubject, contractShippingDueDate, actualShippingDueDate, 
                                                proposedDate, owner, order, location, inspectionType, description);

            inspectionNotification.RaiseEvent(new InspectionNotificationCreatedEvent(inspectionNotification.InspectionNotificationId));

            return inspectionNotification;
        }

        public static InspectionNotification CreateByUniqueId(InspectionNotification inspectionNotification)
        {
            var ins = inspectionNotification;
            var newinspectionNotification = InspectionNotification.Create(ins.Name,ins.NotificationNumber,ins.InspectionSubject,ins.ContractShippingDueDate!.Value,
                ins.ActualShippingDueDate!.Value,ins.ProposedDate!.Value,ins.Owner,ins.Order!,ins.Location!,ins.InspectionType!,ins.Description!);
            newinspectionNotification.InspectionNotificationId = InspectionNotificationId.CreateUnique();


            newinspectionNotification.RaiseEvent(new InspectionNotificationCreatedEvent(newinspectionNotification.InspectionNotificationId));

            return newinspectionNotification;
        }

        public void AddAttachment(string name, string description, byte[] content)
        {
            var attachment = Attachment.Create(name, description, content);          
            _attachments.Add(attachment);
        }

        public void AddContact(string name,Address address,string phone,string fax, string email,string homepage)
        {
            var contact = Contact.Create(name, address, phone, fax, email, homepage);
            _contacts.Add(contact);
        }

    }
}
