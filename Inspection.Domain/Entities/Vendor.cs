using Inspection.Domain.Entities.Common;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Inspection.Domain.Entities
{
    public sealed class Vendor : BaseDomainEntity<Guid>
    {

        public string Name { get; private set; }
        public Project Project { get; private set; }
        public IEnumerable<Contact> Contacts { get;private set; }

        public Vendor() { }

        public Vendor(Guid id):base(id)
        {
            
        }
    }
}
