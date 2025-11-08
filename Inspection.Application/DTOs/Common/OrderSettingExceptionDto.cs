using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class OrderSettingExceptionDto
    {
        public OrderSettingExceptionDto() { }

        public Guid? Id { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ProjectContactId { get; set; }
        public OrderDto? Order { get; set; }
        public ProjectContactDto? ProjectContact { get; set; }
    }
}
