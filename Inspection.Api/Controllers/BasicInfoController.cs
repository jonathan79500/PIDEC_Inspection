using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.DTOs.User;
using Inspection.Application.Features.Authentication.Queries.Login;
using Inspection.Application.Features.BasicInfo.Orders.Commands.ActivateEdmsOrderInInspectionSystem;
using Inspection.Application.Features.BasicInfo.Orders.Queries.GetEdmsOrderDataIfExist;
using Inspection.Application.Features.BasicInfo.Orders.Queries.GetEdmsOrdersByProjectId;
using Inspection.Application.Features.BasicInfo.Projects.Queries.GetProjectDetailsByProjectId;
using Inspection.Application.Features.BasicInfo.Projects.Commands.SaveProjectSettings;
using Inspection.Application.Features.BasicInfo.Projects.Queries.GetPIDECProjects;

using Inspection.Application.Features.BasicInfo.Vendors.Queries.BasicVendorInfos;
using Inspection.Application.Features.BasicInfo.Vendors.Queries.GetVendorDataByUserName;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotifications;
using Inspection.Domain.Constants;
using Inspection.Domain.Entities;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Inspection.Api.Controllers
{
    [Route("[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class BasicInfoController : ControllerBase
    {

        private readonly ISender _mediator;


        
        public BasicInfoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<TestController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //[HttpPost]
        //[Route("GetVendorDataByVendorInfo")]
        //public async Task<ActionResult<VendorDto>> GetVendorDataByVendorInfo([FromBody] VendorDto vendorData)
        //{

        //    var vendorDataRes = await _mediator.Send(new GetVendorDataByUserNameQuery { vendorUserName = vendorData.UserName,domainId= vendorData.DomainId });
        //    try
        //    {
        //        return Ok(vendorDataRes);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("IsValidUser")]
        //public async Task<ActionResult<bool>> IsValidUser([FromBody] UserDto userData)
        //{

        //    var userIsValid = await _mediator.Send(new LoginQuery { User = new UserDto() { UserName = userData.UserName, Password = userData.Password, domainId = userData.domainId } });
        //    try
        //    {
        //        return Ok(userIsValid);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        // GET: api/<BasicInfoController>

        [HttpGet]
        [Authorize(Roles = UserRoles.Vendor)]
        [Route("GetBasicInformationForVendors")]
        public async Task<ActionResult<BasicVendorInfo>> GetBasicInfoForVendors()
        {

            var basicInfos = await _mediator.Send(new GetBasicVendorInfoQuery());
            try
            {
                return Ok(basicInfos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = UserRoles.QC_Manager)]
        [HttpGet]
        [Route("GetEdmsOrdersByProjectId/{ProjectId}")]
        public async Task<ActionResult<List<Order>>> GetEdmsOrdersByProjectId([FromRoute] string ProjectId)
        {
            var EdmsOrders = await _mediator.Send(new GetEdmsOrdersByProjectIdQuery { ProjectId = ProjectId });
            return Ok(EdmsOrders);
        }


        [Authorize(Roles = UserRoles.QC_Manager)]
        [HttpGet]
        [Route("GetEdmsOrderDataIfExist/{EdmsOrderId}/{ProjectId}")]
        public async Task<ActionResult<OrderDto>> GetEdmsOrderDataIfExist([FromRoute] string EdmsOrderId, [FromRoute] string ProjectId)
        {
            var EdmsOrders = await _mediator.Send(new GetEdmsOrderDataIfExistQuery { EdmsOrderId = EdmsOrderId ,ProjectId=Guid.Parse(ProjectId)});
            return Ok(EdmsOrders);
        }

        [Authorize(Roles = UserRoles.QC_Manager)]
        [HttpGet]
        [Route("GetProjectDetailByProjectId/{ProjectId}")]
        public async Task<ActionResult<ProjectDto>> GetProjectDetailByProjectId([FromRoute] string ProjectId)
        {
            var ProjectData = await _mediator.Send(new GetProjectDetailsByProjectIdQuery { ProjectId = ProjectId });
            return Ok(ProjectData);
        }

        [HttpGet]
        [Route("GetProjectsByVendorId/{VendorId}")]
        public async Task<ActionResult<List<ProjectDto>>> GetProjectsByVendorId([FromRoute] string VendorId)
        {
            
            var projects = await _mediator.Send(new GetPIDECProjectsQuery { VendorContactId =new Guid(VendorId) });
            try
            {
                return Ok(projects);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //[Authorize(Roles = UserRoles.QC_Manager)]
        //
        //[Authorize]
        //[Route]  
        [AllowAnonymous]
        [HttpPost("ActivateOrderInInspectionSystem")]     
        public async void ActivateOrderInInspectionSystem([FromBody] OrderDto orderDto) {

            //OrderDto order = new OrderDto();
           bool result = await _mediator.Send(new ActivateEdmsOrderInInspectionSystemCommand { OrderPropForActivation=orderDto});
            //try {
            //    return  Ok(result);
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message);
            //}
        }

        [AllowAnonymous]
        [HttpPost("SaveProjectSettings")]
        public async Task<IActionResult> SaveProjectSettings([FromBody] ProjectDto projectDto)
        {
            bool result = await _mediator.Send(new SaveProjectSettingsCommand {  projectDto = projectDto });
            return Ok(result);
        }

    }

}
