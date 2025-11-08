using Arch.EntityFrameworkCore.UnitOfWork;
using Inspection.Application.Common.Interfaces.Authentication;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.User.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler:IRequestHandler<RegisterCommand,SystematicUser>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserManagerRepository _userManagerRepository;     
        private readonly IEmailSender _emailSender;
        private readonly IPublisher _publisher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IUserManagerRepository userManagerRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManagerRepository = userManagerRepository;         
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<SystematicUser> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            //check if user already exist

            //Create User (generate Unique Id)

            //Create Jwt Token


            Guid userId = Guid.NewGuid();
            SystematicUser newUser = new SystematicUser();
            newUser.Email = request.userRegistrationRequestDto.Email;
            newUser.PasswordHash = request.userRegistrationRequestDto.Password;
            newUser.UserName = request.userRegistrationRequestDto.Name;
            newUser.DomainId = request.userRegistrationRequestDto.DomainId;
            newUser.Id = userId.ToString();


            var token = _jwtTokenGenerator.GenerateToken(newUser, []);
            newUser.SetToken(token);


            var (resultUser,callbackResetPassUrl) = await _userManagerRepository.RegisterNewUser(newUser, []);
            if (resultUser != null)
            {
                return resultUser;
            }
            else
                return null;
           
        }
    }
}
