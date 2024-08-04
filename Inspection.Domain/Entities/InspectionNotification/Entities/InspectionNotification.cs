
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;

using System;
using System.Collections.Generic;


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

        public readonly List<Contact>? Contacts = new List<Contact>();


        public readonly List<Attachment>? Attachments = new List<Attachment>();


        public static InspectionNotification Create(string name,NotificationNumber notificationNumber,string inspectionSubject,            
            DateTime contractShippingDueDate, DateTime actualShippingDueDate,DateTime proposedDate, Vendor owner, Order order,
            Address location, InspectionType inspectionType, string description)
        { 
            var inspectionNotification =  new InspectionNotification( InspectionNotificationId.CreateUnique() ,name, notificationNumber, inspectionSubject, contractShippingDueDate, actualShippingDueDate, 
                                                proposedDate, owner, order, location, inspectionType, description);

            inspectionNotification.RaiseEvent(new InspectionNotificationCreatedEvent(inspectionNotification.InspectionNotificationId));

            return inspectionNotification;
        }

   
        public void AddAttachment(string name, string description, byte[] content)
        {
            var attachment = Attachment.Create(name, description, content);          
            Attachments.Add(attachment);
        }

        public void AddContact(string name,Address address,string phone,string fax, string email,string homepage)
        {
            var contact = Contact.Create(name, address, phone, fax, email, homepage);
            Contacts.Add(contact);
        }

    }
}
