using System;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;


namespace Inspection.Domain.Entities
{
    public class Contact: BaseDomainEntity<Guid>
    {

        public Contact()
        {
            
        }
        private Contact(Guid id) : base(id)
        {   
        }

        private Contact(string name, Address address, string phone, string fax, string email, string homepage)
        {
            Id = Guid.NewGuid();
            Name = name;
            Address = address;
            Phone = phone;
            Fax = fax;
            Email = email;
            HomePage = homepage;
        }


        public static Contact Create(string name, Address address, string phone, string fax, string email, string homepage)
        {
            return new Contact(name,address, phone, fax, email, homepage);
        }
        
        public string Name { get;private set; }
        public Address Address { get;private set; }      
        public string Phone { get;private set; }
        public string Fax { get;private set; }=string.Empty;
        public string Email { get;private set; }
        public string HomePage { get;private set; }
        = string.Empty;
    }
}
