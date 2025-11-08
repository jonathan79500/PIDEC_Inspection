using Inspection.Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Authentication.Queries.GetUsersInfoByProjectId
{
    public class GetUsersInfoByProjectIdQuery:IRequest<List<UserDto>>
    {
        public string ProjectId { get; set; }
    }
}
