using System;
using Inspection.Application.Services.Authentication;

namespace Inspection;

public class AuthenticationService : IAuthenticationService
{
    public AuthenticationResult Login( string email, string password)
    {
        return new AuthenticationResult(Guid.NewGuid(),  email, "token");  
    }
}
