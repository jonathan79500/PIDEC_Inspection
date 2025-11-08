
using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace Inspection.Application.Features.BasicInfo.Projects.Queries.GetPIDECProjects;

public class GetPIDECProjectsQuery:IRequest<List<ProjectDto>>
{
    public Guid VendorContactId { get; set; }
}
