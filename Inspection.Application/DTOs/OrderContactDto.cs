using Inspection.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs
{
    public class OrderContactDto
    {
        public OrderContactDto() { }
        public Guid? Id { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ContactId { get; set; }
        public OrderDto? Order { get; set; }
        public ContactDto? Contact{ get; set; }
    }
}
