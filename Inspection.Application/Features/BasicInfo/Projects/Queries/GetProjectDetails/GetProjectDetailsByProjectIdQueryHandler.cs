using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Application.Features.BasicInfo.Projects.Queries.GetProjectDetailsByProjectId;
using Inspection.Domain.Entities.Projects;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Inspection.Application.Features.BasicInfo.Projects.Queries.GetProjectDetailsByProjectId;

public class GetProjectDetailsByProjectIdQueryHandler : IRequestHandler<GetProjectDetailsByProjectIdQuery, ProjectDto>
{
    private readonly IBasicInformationRepository _basicInformationRepository;
    private readonly IEDMSService _edmsService;
    public GetProjectDetailsByProjectIdQueryHandler(IBasicInformationRepository basicInformationRepository, IEDMSService eDMSService)
    {
        _basicInformationRepository = basicInformationRepository;      
    }

    Task<ProjectDto> IRequestHandler<GetProjectDetailsByProjectIdQuery, ProjectDto>.Handle(GetProjectDetailsByProjectIdQuery request, CancellationToken cancellationToken)
    {
        ProjectDto result = new ProjectDto();
        Guid projectId = new Guid(request.ProjectId);
        Task<Project> projectDetail = _basicInformationRepository.GetProjectDataInQCIMS(projectId);

        //Task<List<EDMSOrder>> eDMSOrders = _edmsService.GetEdmsOrders_ByProId(request.ProjectId.ToString());


        try
        {            
            var prjDto = projectDetail.Result.Adapt<ProjectDto>();
            return Task.FromResult(prjDto);
        }
        catch (Exception ex)
        {
            string exp = ex.Message;
        }
        return null;
    }
}
