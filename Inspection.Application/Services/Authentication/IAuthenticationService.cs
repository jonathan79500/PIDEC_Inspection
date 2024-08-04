namespace Inspection.Application.Services.Authentication;

public interface IAuthenticationService
{
  AuthenticationResult Login(string email, string password);

}

