using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Orders
{
    public class OrdersettingException
    {
        public OrdersettingException() { }

        public Guid? Id { get; set; }
        public OrderId? OrderId { get; set; }
        public Guid? ProjectContactId { get; set; }
        public Order? Order { get; set; }
        public ProjectContact? ProjectContact { get; set; }        
    }
}
