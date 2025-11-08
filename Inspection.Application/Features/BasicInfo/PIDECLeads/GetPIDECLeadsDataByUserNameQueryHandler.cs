using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Common;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.PIDECLeads
{
    public class GetPIDECLeadsDataByUserNameQueryHandler : IRequestHandler<GetPIDECLeadsDataByUserNameQuery, BasicPIDECLeadsInfoDto>
    {

        private readonly IEDMSService _eDMSService;
        private IBasicInformationRepository _basicInformationRepository;

        public GetPIDECLeadsDataByUserNameQueryHandler(IEDMSService eDMSService, IBasicInformationRepository basicInformationRepository)
        {
            _eDMSService = eDMSService;
            _basicInformationRepository = basicInformationRepository;
        }
        public Task<BasicPIDECLeadsInfoDto> Handle(GetPIDECLeadsDataByUserNameQuery request, CancellationToken cancellationToken)
        {
            Task<BasicPIDECLeadsInfo> basicPIDECInfoTask = _basicInformationRepository.GetPIDECLeadInfo(request.UserName, request.DomainId);
            try
            {
                BasicPIDECLeadsInfoDto basicPIDECLeadsInfoDto = basicPIDECInfoTask.Result.Adapt<BasicPIDECLeadsInfoDto>();


                return Task.FromResult(basicPIDECLeadsInfoDto);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }
    }
}
