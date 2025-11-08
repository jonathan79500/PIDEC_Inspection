using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.User.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Authentication.Commands.SetPassword
{
    public class SetPasswordCommandHandler : IRequestHandler<SetPasswordCommand, bool>
    {
        private IUserManagerRepository _userManagerRepository;
        public SetPasswordCommandHandler(IUserManagerRepository userManagerRepository) { 
        _userManagerRepository = userManagerRepository;
        }
        public Task<bool> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.newPassword == request.ConfirmedPassword)
            {
                var user = request.User.Adapt<SystematicUser>();
                var result = _userManagerRepository.ResetPassword(user , request.newPassword,request.oldPassword);
                if (result != null)
                {
                    return result;
                }
            }
            return Task.FromResult(false);
        }
    }
}
