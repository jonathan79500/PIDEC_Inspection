using Inspection.Application.Features.Authentication.Common;
using Inspection.Domain.Entities.User.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inspection.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommand:IRequest<SystematicUser>
    {
        public UserRegistrationRequestDto userRegistrationRequestDto { get; set;}
    }
}
