using Inspection.Application.DTOs;
using Inspection.Domain.Entities.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Orders.Commands.ActivateEdmsOrderInInspectionSystem
{
    public class ActivateEdmsOrderInInspectionSystemCommand:IRequest<bool>
    {
        public OrderDto OrderPropForActivation { get; set; }
    }
}
