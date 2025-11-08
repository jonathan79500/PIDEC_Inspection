using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Orders.Queries.GetEdmsOrderDataIfExist
{
    public class GetEdmsOrderDataIfExistQueryHandler : IRequestHandler<GetEdmsOrderDataIfExistQuery, OrderDto>
    {
        private readonly IBasicInformationRepository _basicInformationRepository;
      
        public GetEdmsOrderDataIfExistQueryHandler(IBasicInformationRepository basicInformationRepository)
        {
            _basicInformationRepository = basicInformationRepository;            
        }
        public Task<OrderDto> Handle(GetEdmsOrderDataIfExistQuery request, CancellationToken cancellationToken)
        {
            OrderDto? orderDto = null;
            var Result = _basicInformationRepository.GetEdmsOrderDataInQCIMS(int.Parse(request.EdmsOrderId),request.ProjectId);
            try
            {
                if (Result != null)
                { 
                orderDto = Result.Result.Adapt<OrderDto>();                
                }
                return Task.FromResult(orderDto);
            }catch (Exception ex)
            {
                return null;
            }
        }
    }
}
