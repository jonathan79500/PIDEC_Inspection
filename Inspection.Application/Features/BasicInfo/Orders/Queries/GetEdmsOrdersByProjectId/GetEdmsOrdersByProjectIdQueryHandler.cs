using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Domain.Entities.EDMS;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Inspection.Application.Features.BasicInfo.Orders.Queries.GetEdmsOrdersByProjectId;

public class GetEdmsOrdersByProjectIdQueryHandler : IRequestHandler<GetEdmsOrdersByProjectIdQuery, List<OrderDto>>
{
    private readonly IBasicInformationRepository _basicInformationRepository;
    private readonly IEDMSService _edmsService;
    public GetEdmsOrdersByProjectIdQueryHandler(IBasicInformationRepository basicInformationRepository, IEDMSService eDMSService)
    {
        _basicInformationRepository = basicInformationRepository;
        _edmsService = eDMSService;
    }

    Task<List<OrderDto>> IRequestHandler<GetEdmsOrdersByProjectIdQuery, List<OrderDto>>.Handle(GetEdmsOrdersByProjectIdQuery request, CancellationToken cancellationToken)
    {
        List<OrderDto> result = new List<OrderDto>();
        Task<List<EDMSOrder>> eDMSOrders = _edmsService.GetEdmsOrders_ByProId(request.ProjectId.ToString());


        try
        {
            var order = new EDMSOrder();
            var ordDto = order.Adapt<OrderDto>();
        }
        catch (Exception ex)
        {
            string exp = ex.Message;
        }

        List<OrderDto> orderDtos = eDMSOrders.Result.Adapt<List<OrderDto>>();


        return Task.FromResult(orderDtos);
    }
}
