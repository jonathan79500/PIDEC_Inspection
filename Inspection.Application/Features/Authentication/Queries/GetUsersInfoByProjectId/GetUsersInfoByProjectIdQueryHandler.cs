using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.User;
using Inspection.Domain.Entities.User.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Authentication.Queries.GetUsersInfoByProjectId
{
    public class GetUsersInfoByProjectIdQueryHandler : IRequestHandler<GetUsersInfoByProjectIdQuery, List<UserDto>>
    {
        private IUserManagerRepository  userManagerRepository;

        public GetUsersInfoByProjectIdQueryHandler(IUserManagerRepository _userManagerRepository)
        {
            userManagerRepository = _userManagerRepository;
        }
        public Task<List<UserDto>> Handle(GetUsersInfoByProjectIdQuery request, CancellationToken cancellationToken)
        {
            List<SystematicUser> ReturnedUsers = userManagerRepository.GetUsersInfoByProjectId(request.ProjectId).Result;
            List<UserDto> userDtos = ReturnedUsers.Adapt<List<UserDto>>();

            return Task.FromResult(userDtos);
        }
    }
}
