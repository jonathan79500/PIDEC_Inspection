using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Inspection.Application.Features.BasicInfo.Orders.Queries.GetOrdersByProjectId;

public class GetEdmsOrdersByProjectIdQueryHandler : IRequestHandler<GetEdmsOrdersByProjectIdQuery, List<OrderDto>>
{
    private readonly IBasicInformationRepository _basicInformationRepository;
    public GetEdmsOrdersByProjectIdQueryHandler(IBasicInformationRepository basicInformationRepository)
    {
        _basicInformationRepository = basicInformationRepository;

    }

    Task<List<OrderDto>> IRequestHandler<GetEdmsOrdersByProjectIdQuery, List<OrderDto>>.Handle(GetEdmsOrdersByProjectIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
