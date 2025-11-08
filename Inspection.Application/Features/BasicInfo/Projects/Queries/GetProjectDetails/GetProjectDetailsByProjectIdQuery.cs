using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Projects.Queries.GetProjectDetailsByProjectId
{
    public class GetProjectDetailsByProjectIdQuery : IRequest<ProjectDto>
    {
        public string ProjectId { get; set; }
    }
}
