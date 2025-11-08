using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Inspectors.Queries
{
    public class GetInspectorsDataByUserNameQueryHandler : IRequestHandler<GetInspectorsDataByUserNameQuery, BasicInspectorInfoDto>
    {
        private readonly IBasicInformationRepository _basicInformationRepository;
        
        public GetInspectorsDataByUserNameQueryHandler(IBasicInformationRepository basicInformationRepository)
        {
            _basicInformationRepository = basicInformationRepository;
        }

        public Task<BasicInspectorInfoDto> Handle(GetInspectorsDataByUserNameQuery request, CancellationToken cancellationToken)
        {
          

            Task<BasicInspectorInfo> basicInspectorInfoTask = _basicInformationRepository.GetBasicInspectorInfo(request.UserName, request!.DomainId);
            BasicInspectorInfoDto basicInspectorInfoDto = basicInspectorInfoTask.Result.Adapt<BasicInspectorInfoDto>();
            try
            {
                return Task.FromResult(basicInspectorInfoDto);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return null;
            }
        }
    }
}
