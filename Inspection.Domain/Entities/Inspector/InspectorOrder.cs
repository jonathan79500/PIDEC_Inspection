using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class InspectorOrder
    {
        public int Id { get; set; }
        public Guid InspectorId { get; set; }
        public OrderId OrderId { get; set; }

        public Inspector Inspector { get; set; }
        public Order Order { get; set; } 
    }
}
