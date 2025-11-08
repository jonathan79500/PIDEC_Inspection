using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Orders
{
    public class OrderInspectionSubject//:BaseDomainEntity<Guid>
    {
        public OrderInspectionSubject() { }

        public Guid? Id { get; set; }
        public InspectionSubjectId? InspectionSubjectId { get; set; }
        public InspectionSubject? InspectionSubject { get; set; }
        public Order? Order { get; set; }
        public OrderId? OrderId {get;set;}
    }
}
