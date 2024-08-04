using Inspection.Application.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Inspection.Api.Controllers;


[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{

  private readonly IAuthenticationService _authenticationService;

  public AuthenticationController(IAuthenticationService authenticationService)
  {
    _authenticationService = authenticationService;
  }

  [HttpPost("Login")]
  public IActionResult Login(LoginRequest request)
  {
    var actionResult = _authenticationService.Login(request.Email, request.Password);

    return Ok(actionResult);
  }
}
