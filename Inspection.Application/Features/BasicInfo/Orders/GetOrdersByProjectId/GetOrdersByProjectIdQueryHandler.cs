using AutoMapper;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Orders.GetOrdersByProjectId;

public class GetOrdersByProjectIdQueryHandler : IRequestHandler<GetOrdersByProjectIdQuery, List<OrderDto>>
{


    private readonly IMapper _mapper;
    private readonly IBasicInformationRepository _basicInformationRepository;
    public GetOrdersByProjectIdQueryHandler(IMapper mapper, IBasicInformationRepository basicInformationRepository)
    {
        _basicInformationRepository = basicInformationRepository;
        _mapper = mapper;
    }

    Task<List<OrderDto>> IRequestHandler<GetOrdersByProjectIdQuery, List<OrderDto>>.Handle(GetOrdersByProjectIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
