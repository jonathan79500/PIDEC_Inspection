using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspection.Domain.Entities
{
    public sealed class Project : BaseDomainEntity<Guid>
    {

        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Number { get;private set; } = string.Empty;
        public string FSId { get; private set; } = string.Empty;

        public List<ProjectVendor> vendors { get; private set; }


        public Project() { }    
        public Project(Guid id):base(id) {
        
        }

        public static Project Create(Guid id,string title,string description,string number, string fsId)
        {
            return new Project(id)
            {
                Title = title,
                Description = description,
                Number = number,
                FSId = fsId
            };
        }
        
    }
}
