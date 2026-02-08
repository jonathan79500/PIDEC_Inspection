using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common.ValueObjects
{
    public sealed class Address : BaseDomainEntity<Guid>
    {

        public string? Country { get;  set; }
        public string? State { get;  set; }
        public string? City { get;  set; }
        public string? StreetLine1 { get;  set; }
        public string? StreetLine2 { get;  set; }
        public string? ZipCode { get;  set; }

        public List<InspectionNotificationLocation> InspectionNotifications { get; set; }

        //public Address address { get; private set; }

        public Address() { }

        public Address(string country, string state, string city, string streetL1, string streetL2, string zipCode)
        {
            Id = Guid.NewGuid();
            Country = country;
            City = city;
            State = state;
            StreetLine1 = streetL1;
            StreetLine2 = streetL2;
            ZipCode = zipCode;
        }

        public Address Create(string country, string state, string city, string streetL1, string streetL2, string zipCode)
        {
            return new Address(country, state, city, streetL1, streetL2, zipCode);
        }

        //public override IEnumerable<object> GetEqualityComponents()
        //{
        //    yield return address;
        //}
    }
}
