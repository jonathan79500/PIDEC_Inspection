using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Features.InspectionNotifications.Commands.CreateInspectionNotification;
using Inspection.Application.Features.InspectionNotifications.Common;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;


namespace Inspection.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InspectionNotificationController : ControllerBase
{
    private readonly ISender _mediator;
    public InspectionNotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }
    // GET: api/<InspectionController>
    [HttpGet]
    [Route("GetAll")]
    public async Task<ActionResult<List<InspectionNotificationDto>>> Get()
    {
        var inspectionNotifications = await _mediator.Send(new GetInspectionNotificationListRequest());

        return Ok(inspectionNotifications);
    }

    // GET api/<InspectionController>/5 [Route("{id}/GetById")]
    //[HttpGet("{id}")]
    [HttpGet]
    [Route("GetById/{id}")]
    public async Task<ActionResult<InspectionNotificationDto>> GetById([FromRoute]Guid id)
    {
        var inspectionNotification = await _mediator.Send(new GetInspectionNotificationDetailRequest { Id = new Domain.Entities.InspectionNotification.ValueObjects.InspectionNotificationId(id)});
        return Ok(inspectionNotification);
    }


    [HttpPost]
    [Route("SaveNew")]
    public async Task<ActionResult> Post([FromBody] CreateInspectionNotificationDto inspectionNotification)
    {
        var command = new CreateInspectionNotificationCommand { createInspectionNotificationDto = inspectionNotification };

        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {

            return Ok("jdljqwl");
        }
        
    }


    // POST api/<InspectionController>
    [HttpPost]
    //[Route("SaveNew")]
    public async Task<ActionResult> Post(string str)//[FromBody] CreateInspectionNotificationDto inspectionNotification)
    {
        return Ok(str);
        //var command = new CreateInspectionNotificationCommand { createInspectionNotificationDto = inspectionNotification };

        //var response = await _mediator.Send(command);

        //return Ok(response);
    }

    // PUT api/<InspectionController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<InspectionController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
