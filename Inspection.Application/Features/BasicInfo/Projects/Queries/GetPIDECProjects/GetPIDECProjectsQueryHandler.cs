using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.Vendor;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Projects.Queries.GetPIDECProjects;

public class GetPIDECProjectsQueryHandler : IRequestHandler<GetPIDECProjectsQuery, List<ProjectDto>>
{

    private readonly IBasicInformationRepository _basicInformationRepository;
    public GetPIDECProjectsQueryHandler(IBasicInformationRepository basicInformationRepository)
    {
        _basicInformationRepository = basicInformationRepository;    
    }
    public Task<List<ProjectDto>> Handle(GetPIDECProjectsQuery request, CancellationToken cancellationToken)
    {
        ContactDto vendorcontactDto = new ContactDto();
        vendorcontactDto.Id = request.VendorContactId;

        Contact vendorcontact = vendorcontactDto.Adapt<Contact>();

        Task<List<Project>> projects = _basicInformationRepository.GetProjectsByVendor(vendorcontact);

        List<ProjectDto> projectDtos = projects.Result.Adapt<List<ProjectDto>>();

        return Task.FromResult(projectDtos);
    }
}


