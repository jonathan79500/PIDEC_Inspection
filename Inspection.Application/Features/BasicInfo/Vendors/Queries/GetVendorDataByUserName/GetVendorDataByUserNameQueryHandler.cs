using AutoMapper;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Vendors.Queries.GetVendorDataByUserName
{
    public class GetVendorDataByUserNameQueryHandler : IRequestHandler<GetVendorDataByUserNameQuery, VendorDto>
    {

        private readonly IMapper _mapper;
        private readonly IEDMSService _eDMSService;

        public GetVendorDataByUserNameQueryHandler(IMapper mapper,IEDMSService eDMSService)
        {
            _mapper = mapper;
            _eDMSService = eDMSService;
        }

        public Task<VendorDto> Handle(GetVendorDataByUserNameQuery request, CancellationToken cancellationToken)
        {
            var result = _eDMSService.GetVendorInformation(request.vendorUserName,request.domainId).Result
                .FirstOrDefault();  
            return Task.FromResult(_mapper.Map<VendorDto>(result));
            
        }
    }
}
