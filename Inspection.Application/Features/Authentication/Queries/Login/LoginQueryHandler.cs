using Inspection.Application.Contracts.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


using Inspection.Application.DTOs.User;
using Inspection.Application.Features.Authentication.Common;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.Common.Interfaces.Authentication;
using Mapster;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Constants;

namespace Inspection.Application.Features.Authentication.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, UserDto>
    {
      
        private readonly IEDMSService _eDMSService;
        private readonly IUserManagerRepository _userManagerRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;


        public LoginQueryHandler( IEDMSService eDMSService,IUserManagerRepository userManagerRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _eDMSService = eDMSService;
            _userManagerRepository= userManagerRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<UserDto> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            //At the First step User is validated based on Inspection System, if it is available so we generate the token and return user
            //if user is not available in inspection system, it probably shows that it is the first time that user login in this system, so 
            //we should validate it by EDMS system, and then return data from that
            UserDto loginnedUser = null;
           
            SystematicUser EdmsSystemUser = null;
            try
            {

                var (authenticatedUser, userRoles) = await _userManagerRepository.Login(request.User.UserName, request.User.Password, request.User.DomainId);
                if(authenticatedUser == null) {
                    return null;
                }

                if (userRoles != null && userRoles!.Contains("Vendor"))
                {
                    if (authenticatedUser.PasswordHash != null )
                    {
                        if (authenticatedUser.IsAuthenticated == false)
                            return null;
                        else
                            return authenticatedUser.Adapt<UserDto>();
                    }                    
                    else if(authenticatedUser.PasswordHash == null && authenticatedUser.IsAuthenticated == false) { 
                    
                    EdmsSystemUser = _eDMSService.UserIsValid(request.User.UserName, request.User.Password, request.User.DomainId).Result;
                        if (EdmsSystemUser != null && EdmsSystemUser.IsAuthenticated && EdmsSystemUser.IsActive)
                        {
                            var updatedVendor = await _userManagerRepository.UpdateUserPassword_Token(authenticatedUser, request.User.Password, userRoles);
                            if (updatedVendor != null)
                            {
                                return updatedVendor.Adapt<UserDto>();
                            }
                        }
                        else
                        {
                            EdmsSystemUser.IsActive = false;

                            return EdmsSystemUser.Adapt<UserDto>();

                        }
                    }
                }

                else if (userRoles != null && (userRoles!.Contains("QC_Manager") || userRoles!.Contains("QC_Leads")))
                {
                    return authenticatedUser.Adapt<UserDto>();
                }
                else if (userRoles != null && (userRoles!.Contains("PIDECDepartment_Lead") || userRoles!.Contains("PIDECDepartment_Originator")))
                {
                    return authenticatedUser.Adapt<UserDto>();
                }
                else if (userRoles != null && (userRoles!.Contains("MC") || userRoles!.Contains("MC")))
                {
                    return authenticatedUser.Adapt<UserDto>();
                }
                else
                    return null;

                return null;
            }
            catch (Exception exp)
            { return null; }

            
        }
    }
}
