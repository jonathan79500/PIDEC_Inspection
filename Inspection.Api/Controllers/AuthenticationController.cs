using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.User;
using Inspection.Application.Features.Authentication.Commands.Register;
using Inspection.Application.Features.Authentication.Commands.SetPassword;
using Inspection.Application.Features.Authentication.Common;
using Inspection.Application.Features.Authentication.Queries.GetUsersInfoByProjectId;
using Inspection.Application.Features.Authentication.Queries.Login;
using Inspection.Application.Features.BasicInfo.Inspectors.Queries;
using Inspection.Application.Features.BasicInfo.Inspectors.Queries.GetInspectorsDataByUserName;
using Inspection.Application.Features.BasicInfo.Orders.Commands.ActivateEdmsOrderInInspectionSystem;
using Inspection.Application.Features.BasicInfo.Orders.Queries.GetEdmsOrdersByProjectId;
using Inspection.Application.Features.BasicInfo.PIDECLeads;
using Inspection.Application.Features.BasicInfo.Projects.Queries.GetPIDECProjects;
using Inspection.Application.Features.BasicInfo.QCMembers.Queries.GetQCMemberDataByUserName;
using Inspection.Application.Features.BasicInfo.Vendors.Queries.GetVendorDataByUserName;
using Inspection.Application.Services.Authentication;
using Inspection.Domain.Constants;
using Inspection.Domain.Entities;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.User.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using TestMissionService;

namespace Inspection.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
//[Authorize]
public class AuthenticationController : ControllerBase
{
    //private readonly SignInManager<SystematicUser> _signInManager;

    private readonly ISender _mediator;
    private readonly IAuthenticationService _authenticationService;
    //  private readonly UserManager<SystematicUser> _userManager;

    public AuthenticationController(IAuthenticationService authenticationService, IMediator mediator)//,SignInManager<SystematicUser> signInManager, UserManager<SystematicUser> userManager)
    {
        _authenticationService = authenticationService;
        _mediator = mediator;
        //_signInManager = signInManager;

        //_userManager = userManager;
    }
    // GET: api/<TestController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }
    [Authorize]
    [HttpGet]
    [Route("TestGet/{id}")]
    public string TestGet(int id)
    {
        return "value" + id;
    }


    [AllowAnonymous]
    [HttpPost("ActivateOrderInInspectionSystem")]
    public async void ActivateOrderInInspectionSystem([FromBody] UserDto userDto)
    {

        OrderDto order = new OrderDto();
        bool result = await _mediator.Send(new ActivateEdmsOrderInInspectionSystemCommand { OrderPropForActivation = order });
        //try {
        //    return  Ok(result);
        //}
        //catch (Exception ex)
        //{
        //    return BadRequest(ex.Message);
        //}
    }

    [Authorize]
    [HttpPost]
    [Route("GetVendorDataByVendorInfo_Remotely")]
    public async Task<ActionResult<BasicVendorInfo>> GetVendorDataByVendorInfo_Remotely([FromBody] UserDto vendorData)
    {        
        var a = User.IsInRole("Vendor");
        var vendorDataRes = await _mediator.Send(new GetVendorDataByUserNameQuery { vendorUserName = vendorData.UserName, domainId = vendorData.DomainId!.Value });
        try
        {
            return Ok(vendorDataRes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    [Route("GetQCDataByQCMemberInfo_Remotely")]
    public async Task<ActionResult<BasicQCMemberInfo>> GetQCDataByQCMemberInfo_Remotely([FromBody] UserDto QCMemberData)
    {             
        var qcMemberDataRes = await _mediator.Send(new GetQCMemberDataByUserNameQuery { UserName = QCMemberData.UserName, DomainId = QCMemberData.DomainId!.Value });
        try
        {
            return Ok(qcMemberDataRes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    [Route("GetPIDECLeadDataByPIDECLeadInfo_Remotely")]
    public async Task<ActionResult<BasicQCMemberInfo>> GetPIDECLeadDataByPIDECLeadInfo_Remotely([FromBody] UserDto PIDECLeadData)
    {
        var pidecLeadDataRes = await _mediator.Send(new GetPIDECLeadsDataByUserNameQuery { UserName = PIDECLeadData.UserName, DomainId = PIDECLeadData.DomainId!.Value });
        try
        {
            return Ok(pidecLeadDataRes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = UserRoles.QC_Manager)]
    [HttpGet]
    [Route("GetUserDataByProjectId/{ProjectId}")]
    public async Task<ActionResult<List<UserDto>>> GetUserDataByProjectId([FromRoute] string ProjectId)
    {
        var UsersData = await _mediator.Send(new GetUsersInfoByProjectIdQuery { ProjectId = ProjectId });
        return Ok(UsersData);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    { 
        var tokenHandler = new JwtSecurityTokenHandler();
        // Encoding.ASCII.GetBytes("your_secret_key");
         var key = Encoding.ASCII.GetBytes("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e");
       
        try { 
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters { ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(key), ValidateIssuer = false, ValidateAudience = false, ClockSkew = TimeSpan.Zero },
                out SecurityToken validatedToken); return principal;
        } 
        catch { 
            return null; }
    }


    //[Authorize]
    //[HttpPost]
    //[Route("GetVendorDataByVendorInfo")]
    private async Task<BasicVendorInfoDto> GetVendorDataByVendorInfo([FromBody] UserDto vendorData)
    {
        var vendorDataRes = await _mediator.Send(new GetVendorDataByUserNameQuery { vendorUserName = vendorData.UserName, domainId = vendorData.DomainId!.Value });
        return vendorDataRes!;
    }

    //[Authorize]
    //[HttpPost]
    //[Route("GetQCDataByQCMemberInfo")]
    //private async Task<ActionResult<IBasicUserInfo>> GetQCDataByQCMemberInfo([FromBody] UserDto QCMemberData)
    private async Task<BasicQCMemberInfoDto> GetQCDataByQCMemberInfo([FromBody] UserDto QCMemberData)
    {
        var qcMemberDataRes =await _mediator.Send(new GetQCMemberDataByUserNameQuery { UserName = QCMemberData.UserName, DomainId = QCMemberData.DomainId!.Value });
        return qcMemberDataRes;
    }

    //[Authorize]
    //[HttpPost]
    //[Route("GetQCDataByQCMemberInfo")]
    //private async Task<ActionResult<IBasicUserInfo>> GetQCDataByQCMemberInfo([FromBody] UserDto QCMemberData)
    private async Task<BasicPIDECLeadsInfoDto> GetPIDECLeadDataByPIDECLeadInfo([FromBody] UserDto PIDECLeadData)
    {
        var pidecLeadDataRes = await _mediator.Send(new GetPIDECLeadsDataByUserNameQuery { UserName = PIDECLeadData.UserName, DomainId = PIDECLeadData.DomainId!.Value });
        return pidecLeadDataRes;
    }

    private async Task<BasicInspectorInfoDto> GetInspectorDataByInspectorInfo([FromBody] UserDto InspectorData)
    {
        var inspectorDataRes = await _mediator.Send(new GetInspectorsDataByUserNameQuery { UserName = InspectorData.UserName, DomainId = InspectorData.DomainId!.Value });
        return inspectorDataRes;
    }

    [HttpPost]
    [Route("SetPassword")]
    public async Task<IActionResult> SetPassword([FromBody] UserDto userData) {
        var result = await _mediator.Send(new SetPasswordCommand { User = userData, newPassword = userData.NewPassword,oldPassword= userData.OldPassword, ConfirmedPassword = userData.ConfirmPassword });
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("IsValidUser")]
    public async Task<ActionResult<ValidUserResponse>> IsValidUser([FromBody] UserDto userDto)
    {
        try
        {
            var loggedInUser = await _mediator.Send(new LoginQuery { User = new UserLoginRequestDto { UserName = userDto.UserName, Password = userDto.Password, DomainId = userDto.DomainId!.Value } });


            if (loggedInUser == null || (loggedInUser != null && (loggedInUser.IsActive == false || loggedInUser.IsAuthenticated == false))) { return Unauthorized(); }

            var token = loggedInUser!.Token;

            var principal = GetPrincipalFromToken(token!);
            if (principal == null) { return Unauthorized(); } // Check user role and call the appropriate function
            if (principal.IsInRole("Vendor"))
            {
                var vendorDataRes = await GetVendorDataByVendorInfo(userDto);
                //return Ok(new { User = loggedInUser, Data = ((ObjectResult)vendorDataRes.Result!).Value });
                var response = new ValidUserResponse()
                {
                    User = loggedInUser,
                    QCMemberInfo = null,
                    VendorInfo = vendorDataRes,
                    PIDECLeadsInfo = null,
                    InspectorInfo = null
                };

                return Ok(response);
            }
            else if (principal.IsInRole("QC_Manager") || principal.IsInRole("QC_Lead"))
            {
                var qcMemberDataRes = await GetQCDataByQCMemberInfo(userDto);
                var response = new ValidUserResponse()
                {
                    User = loggedInUser,
                    QCMemberInfo = qcMemberDataRes,
                    VendorInfo = null,
                    PIDECLeadsInfo = null,
                    InspectorInfo = null
                };
                var text = System.Text.Json.JsonSerializer.Serialize(response);
                return Ok(response);
            }
            else if (principal.IsInRole("PIDECDepartment_Lead") || principal.IsInRole("PIDECDepartment_Originator"))
            {
                var pidecLeadDataRes = await GetPIDECLeadDataByPIDECLeadInfo(userDto);
                var response = new ValidUserResponse()
                {
                    User = loggedInUser,
                    QCMemberInfo = null,
                    VendorInfo = null,
                    InspectorInfo=null,
                    PIDECLeadsInfo = pidecLeadDataRes
                };
                var text = System.Text.Json.JsonSerializer.Serialize(response);
                return Ok(response);
            }
            else if (principal.IsInRole("MC") )
            {
                var pidecLeadDataRes = await GetPIDECLeadDataByPIDECLeadInfo(userDto);
                var response = new ValidUserResponse()
                {
                    User = loggedInUser,
                    QCMemberInfo = null,
                    VendorInfo = null,
                    InspectorInfo=null,
                    PIDECLeadsInfo = pidecLeadDataRes
                };
                var text = System.Text.Json.JsonSerializer.Serialize(response);
                return Ok(response);
            }
            else if (principal.IsInRole("ThirdPartyInspector") || principal.IsInRole("PidecInspector"))
            {
                var pidecLeadDataRes = await GetInspectorDataByInspectorInfo(userDto);
                var response = new ValidUserResponse()
                {
                    User = loggedInUser,
                    QCMemberInfo = null,
                    VendorInfo = null,
                    PIDECLeadsInfo =null,
                    InspectorInfo =  pidecLeadDataRes
                };
                var text = System.Text.Json.JsonSerializer.Serialize(response);
                return Ok(response);
            }
            else
            {
                return Forbid();
            }
        }catch (Exception ex)
        {

            return Forbid();
        }
            // return Ok(new { Token = loggedInUser.Token, User = loggedInUser });
       
    }

    public class ValidUserResponse
    {
        public UserDto? User { get; set; }
        public BasicQCMemberInfoDto? QCMemberInfo { get; set; } 
        public BasicVendorInfoDto? VendorInfo { get; set; }
        public BasicPIDECLeadsInfoDto PIDECLeadsInfo { get; set; }
        public BasicInspectorInfoDto InspectorInfo { get; set; }
    }

    //private async Task<ActionResult<IBasicUserInfo>> HandleRequestBasedOnUserRole(UserDto userDto)
    //{
    //    // Call IsValidUser function
    //    var validUserResult = await IsValidUser(userDto);
    //    if (!(validUserResult.Result is OkObjectResult okResult))
    //    {
    //        return validUserResult.Result;
    //    }

    //    var user = okResult.Value as UserDto;
    //    if (user == null)
    //    {
    //        return BadRequest("Invalid user data.");
    //    }

    //    // Check user role and call the appropriate function
    //    if (User.IsInRole("Vendor"))
    //    {
    //        return await GetVendorDataByVendorInfo(userDto);
    //    }
    //    else if (User.IsInRole("QCMember"))
    //    {
    //        return await GetQCDataByQCMemberInfo(userDto);
    //    }
    //    else
    //    {
    //        return Forbid();
    //    }
    //}


    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] UserDto model)
    //{
    //    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
    //    if (result.Succeeded)
    //    {
    //        var user = await _userManager.FindByNameAsync(model.UserName);
    //      //  var token = await _tokenService.GenerateToken(user);
    //        return Ok();
    //    }

    //    return Unauthorized();
    //}
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> register([FromBody] UserDto model)
    {
        UserRegistrationRequestDto userRegistrationRequestDto = new UserRegistrationRequestDto();


        if (model.Email != null && model.Email != "" && model.UserName != null && model.UserName != "")
        {
            userRegistrationRequestDto.Email = model.Email;
            userRegistrationRequestDto.Name = model.UserName;
            userRegistrationRequestDto.Password = model.Password;
            userRegistrationRequestDto.DomainId = model.DomainId!.Value;

            var addedUser = await _mediator.Send(new RegisterCommand() { userRegistrationRequestDto = userRegistrationRequestDto });
            if (addedUser != null)
            {
                RegistrationRequestResponse registrationRequestResponse = new RegistrationRequestResponse();
                registrationRequestResponse.Token = addedUser.Token;
                return Ok(registrationRequestResponse);
            }
            else
                return BadRequest("there is Some Errors");

        }

        return BadRequest("there is Some Errors");
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> login([FromBody] UserDto model)
    {
        UserLoginRequestDto userLoginRequestDto = new UserLoginRequestDto();


        if (model.Password != null && model.DomainId != 0 && model.UserName != null && model.UserName != "")
        {
            userLoginRequestDto.UserName = model.UserName;
            userLoginRequestDto.DomainId = model.DomainId!.Value;
            userLoginRequestDto.Password = model.Password;

            var loggedInUser = await _mediator.Send(new LoginQuery() { User = userLoginRequestDto });
            if (loggedInUser != null)
            {
                RegistrationRequestResponse registrationRequestResponse = new RegistrationRequestResponse();
                registrationRequestResponse.Token = loggedInUser.Token;
                return Ok(registrationRequestResponse);
            }
            else
                return BadRequest("there is Some Errors");

        }

        return BadRequest("there is Some Errors");
    }


    //  [HttpPost("Login")]
    //public IActionResult Login(LoginRequest request)
    //{
    //  var actionResult = _authenticationService.Login(request.Email, request.Password);

    //  return Ok(actionResult);
    //}

    // GET: api/<BasicInfoController>
    [HttpGet]
    [Route("GetProjectsByVendorId/{VendorId}")]
    public async Task<ActionResult<List<ProjectDto>>> GetProjectsByVendorId([FromRoute] string VendorId)
    {

        var projects = await _mediator.Send(new GetPIDECProjectsQuery { VendorContactId = new Guid(VendorId) });
        try
        {
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }




}
