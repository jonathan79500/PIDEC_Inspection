using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspection.Domain
{
    public sealed class Project : BaseDomainEntity<Guid>
    {

        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Number { get;private set; } = string.Empty;
        public string FSId { get; private set; } = string.Empty;


        public Project() { }    
        public Project(Guid id):base(id) { }
        
    }
}
