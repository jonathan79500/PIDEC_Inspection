using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Common;
using Mapster;
using Inspection.Domain.Entities.Vendor;
using Inspection.Domain.Entities.Contacts;

namespace Inspection.Application.Features.BasicInfo.Vendors.Queries.BasicVendorInfos
{
    public class GetBasicVendorInfoQueryHandler : IRequestHandler<GetBasicVendorInfoQuery, BasicVendorInfoDto>
    {
      
        private readonly IBasicInformationRepository _basicInformationRepository;
        
        public GetBasicVendorInfoQueryHandler( IBasicInformationRepository basicInformationRepository)
        {          
            _basicInformationRepository = basicInformationRepository;
        }

        public Task<BasicVendorInfoDto> Handle(GetBasicVendorInfoQuery request, CancellationToken cancellationToken)
        {
            Contact logginedVendor = request.LogginedVendor.Adapt<Contact>();

            Task<BasicVendorInfo> basicVendorInfoTask = _basicInformationRepository.GetBasicVendorInfo(logginedVendor.UserName,logginedVendor!.DomainId!.Value);           
            BasicVendorInfoDto basicVendorInfoDto = basicVendorInfoTask.Result.Adapt<BasicVendorInfoDto>();
            try
            {
                return Task.FromResult(basicVendorInfoDto);            
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }
    }
}
