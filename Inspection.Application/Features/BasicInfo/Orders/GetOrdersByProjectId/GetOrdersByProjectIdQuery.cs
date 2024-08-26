using Inspection.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Orders.GetOrdersByProjectId
{
    public class GetOrdersByProjectIdQuery:IRequest<List<OrderDto>>
    {
        public Guid ProjectId { get; set; }
    }
}
