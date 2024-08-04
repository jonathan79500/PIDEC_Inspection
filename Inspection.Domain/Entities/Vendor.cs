using Inspection.Domain.Entities.Common;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Inspection.Domain
{
    public sealed class Vendor : BaseDomainEntity<Guid>
    {

        public string Name { get; private set; }
        public Project Project { get; private set; }
        public IEnumerable<Contact> Contacts { get;private set; }

        public Vendor(Guid id):base(id)
        {
            
        }
    }
}
