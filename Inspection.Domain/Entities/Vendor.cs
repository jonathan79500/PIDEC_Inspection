using Inspection.Domain.Entities.Common;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Inspection.Domain.Entities
{
    public sealed class Vendor : BaseDomainEntity<Guid>
    {

        public string Name { get; private set; }
        public string UserName { get; private set; }
        public List<ProjectVendor> Projects { get; private set; }
        public IEnumerable<Contact> Contacts { get;private set; }
        private readonly List<Order> orders = new();

        public IReadOnlyList<Order> Orders => orders;

        public Vendor() { }

        public Vendor(Guid id):base(id)
        {
            
        }
    }
}
