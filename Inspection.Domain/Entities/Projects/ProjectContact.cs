using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Projects
{
    public class ProjectContact
    {

        public ProjectContact() { }

        public Guid Id { get; set; }

        public Project Project { get; set; }
        public ProjectId ProjectId { get; set; }

        public ContactId ContactId { get; set; }
        public Contact Contact { get; set; }

        public List<OrdersettingException>? OrderExceptions { get; set; }

    }
}
