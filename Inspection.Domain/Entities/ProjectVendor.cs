using Inspection.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities
{
    public class ProjectVendor:BaseDomainEntity<Guid>
    {
        public Guid ProjectId { get;private set; }
        public Guid VendorId { get; private set; }
        public Project Project { get; private set; }
        public Vendor Vendor { get; private set; }
    }
}
