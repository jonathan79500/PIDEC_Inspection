using Inspection.Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Authentication.Commands.SetPassword
{
    public class SetPasswordCommand:IRequest<bool>
    {
        public UserDto User { get; set; }
        public string newPassword { get; set; }
        public string oldPassword { get; set; }
        public string ConfirmedPassword { get; set; }
    }
}
