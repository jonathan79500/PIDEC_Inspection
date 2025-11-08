using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.ValueObjects;


namespace Inspection.Domain.Entities.Contacts
{
    public class Contact : BaseDomainEntity<ContactId>
    {

        public Contact()
        {

        }
        private Contact(ContactId contactId) : base(contactId)
        {
            this.ContactId = contactId;
        }

        public ContactId? ContactId { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public int? DomainId { get; set; }
        public int? ContactType { get; set; }
        public Address? Address { get; set; }
        public string Phone { get; set; }
        public string? Fax { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? HomePage { get; set; } = string.Empty;
        public string Mobile { get; set; }
        public string? Attention { get; set; }
        public string? Cc { get; set; }
        public bool? IsVisible { get; set; }
        public string? UserId { get; set; }

        [NotMapped]
        public string? Password { get; set; }

        [NotMapped]
        public User.Entities.SystematicUser? User { get; set; }
        public List<OrderContact>? Orders { get; set; }
        

        public List<ProjectContact>? Projects{ get; set; }
        public List<InspectionNotificationContact>? InspectionNotifications { get; set; }


        private Contact(ContactId contactId, string name, Address address, string phone, string mobile, string fax, string email, string homepage, string attention, string cc, int contactType,bool isVisible)
        {
            ContactId = contactId;
            Name = name;
            Address = address;
            Phone = phone;
            Fax = fax;
            Email = email;
            HomePage = homepage;
            Attention = attention;
            Cc = cc;
            ContactType = contactType;
            Mobile = mobile;
            IsVisible = isVisible;
        }



        public static Contact Create(string name, Address address, string phone, string mobile, string fax, string email, string homepage, string attention, string cc, int contactType, bool isVisible)
        {
            return new Contact(ContactId.CreateUnique(), name, address, phone, mobile, fax, email, homepage, attention, cc, contactType,isVisible);
        }

        public void SetContactType(ContactType contactType)
        {
            ContactType = contactType.GetHashCode();
        }

    
    }

}
