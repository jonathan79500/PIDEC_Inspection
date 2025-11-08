
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities;
using Inspection.Domain.Entities.Common;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Vendors.Queries.GetVendorDataByUserName
{
    public class GetVendorDataByUserNameQueryHandler : IRequestHandler<GetVendorDataByUserNameQuery, BasicVendorInfoDto>
    {
        private readonly IEDMSService _eDMSService;
        private readonly IBasicInformationRepository _basicInformationRepository;

        public GetVendorDataByUserNameQueryHandler(IEDMSService eDMSService,IBasicInformationRepository basicInformationRepository)
        {          
            _eDMSService = eDMSService;
            _basicInformationRepository = basicInformationRepository;
        }

        public Task<BasicVendorInfoDto> Handle(GetVendorDataByUserNameQuery request, CancellationToken cancellationToken)
        {

            Task<BasicVendorInfo> QCMITask = _basicInformationRepository.GetBasicVendorInfo(request.vendorUserName, request.domainId);
            // var EDMSresult = _eDMSService.GetVendorInformation(request.vendorUserName, request.domainId).Result;

            // var vendorBasicInfo = QCMIResult.MergeWithASet(EDMSresult);
            //  return Task.FromResult(vendorBasicInfo);
            //  
            try
            {
                BasicVendorInfoDto basicVendorInfoDto = QCMITask.Result.Adapt<BasicVendorInfoDto>();
                return Task.FromResult(basicVendorInfoDto);
            }
            catch(Exception ex)
            {
                return null;
            }            

        }
    }
}
