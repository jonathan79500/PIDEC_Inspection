using Inspection.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Orders.Queries.GetEdmsOrdersByProjectId
{
    public class GetEdmsOrdersByProjectIdQuery : IRequest<List<OrderDto>>
    {
        public string ProjectId { get; set; }
    }
}
