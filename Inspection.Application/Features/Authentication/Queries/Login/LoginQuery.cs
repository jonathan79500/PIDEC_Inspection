using Inspection.Application.DTOs.User;
using Inspection.Application.Features.Authentication.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Authentication.Queries.Login
{
    public class LoginQuery :IRequest<UserDto>
    {
        public UserLoginRequestDto User { get; set; }
    }
}
