

using AutoMapper;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Projects.Queries.GetPIDECProjects;

public class GetPIDECProjectsQueryHandler : IRequestHandler<GetPIDECProjectsQuery, List<ProjectDto>>
{

    private readonly IMapper _mapper;
    private readonly IBasicInformationRepository _basicInformationRepository;
    public GetPIDECProjectsQueryHandler(IMapper mapper, IBasicInformationRepository basicInformationRepository)
    {
        _basicInformationRepository = basicInformationRepository;
        _mapper = mapper;
    }
    public Task<List<ProjectDto>> Handle(GetPIDECProjectsQuery request, CancellationToken cancellationToken)
    {
        // throw new NotImplementedException();
        VendorDto vendorDto = new VendorDto();
        vendorDto.Id = request.VendorId;
        Vendor vendor = new Vendor();


        _mapper.Map(vendorDto, vendor);
        Task<List<Project>> projects = _basicInformationRepository.GetProjectsByVendor(vendor);

        List<ProjectDto> projectDtos = _mapper.Map<List<ProjectDto>>(projects.Result);

        return Task.FromResult(projectDtos);
    }
}


