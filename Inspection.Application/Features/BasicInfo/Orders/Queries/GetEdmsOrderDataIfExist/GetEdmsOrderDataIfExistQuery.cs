using Inspection.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Orders.Queries.GetEdmsOrderDataIfExist
{
    public class GetEdmsOrderDataIfExistQuery:IRequest<OrderDto>
    {
        public string EdmsOrderId { get; set;}
        public Guid ProjectId { get; set; }
    }
}
