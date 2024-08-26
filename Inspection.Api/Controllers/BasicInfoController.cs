using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Features.BasicInfo.Projects.Queries.GetPIDECProjects;
using Inspection.Application.Features.BasicInfo.Vendors.Queries.GetVendorDataByUserName;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotifications;
using Inspection.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Inspection.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicInfoController : ControllerBase
    {

        private readonly ISender _mediator;


        
        public BasicInfoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetVendorDataByVendorInfo")]
        public async Task<ActionResult<VendorDto>> GetVendorDataByVendorInfo([FromBody] vendor vendorData)
        {
           
            var vendorDataRes = await _mediator.Send(new GetVendorDataByUserNameQuery { vendorUserName = vendorData.vendorUserName,domainId= vendorData.domainId });
            try
            {
                return Ok(vendorDataRes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/<BasicInfoController>
        [HttpGet]
        [Route("GetProjectsByVendorId/{VendorId}")]
        public async Task<ActionResult<List<ProjectDto>>> GetProjectsByVendorId([FromRoute] string VendorId)
        {
            
            var projects = await _mediator.Send(new GetPIDECProjectsQuery { VendorId =new Guid(VendorId) });
            try
            {
                return Ok(projects);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

    public class vendor
    {
        public string vendorUserName { get; set; }
        public int domainId { get; set; }
    }
}
