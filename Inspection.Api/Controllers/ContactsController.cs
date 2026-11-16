using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Features.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inspection.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class ContactsController : ControllerBase
{

    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator) => _mediator = mediator;

    // GET api/contacts
    // Lightweight list for the dropdown in the inspector form (id, name, contact type code)
    [HttpGet]
    public async Task<ActionResult<List<ContactOptionDto>>> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetContactOptionsQuery(), ct));


    [HttpPost("GetByProject_Types")]
    public async Task<ActionResult<List<ContactOptionDto>>> GetByProject_Types([FromBody] queryParams param, CancellationToken ct)
        => Ok(await _mediator.Send(new GetContactOptionsQueryByProject_ContactTypes{ProjectId = new Guid(param.projectId),  ContactTypes=param.requestedContactTypes}, ct));
    
    // GET api/contacts/by-project/{projectId}
    // GET api/contacts/by-project/{projectId}?contactType=9     (only one contact type)
    [HttpPost("GetByProject")]
    public async Task<ActionResult<List<ContactDto>>> GetByProject([FromBody] queryProjectParams param, CancellationToken ct)
        => Ok(await _mediator.Send(new GetContactsByProjectQuery(new Guid(param.projectId)), ct));

    [HttpPost("GetByTypes")]
    public async Task<ActionResult<List<ContactDto>>> GetByTypes([FromBody] queryContactTypesParams param, CancellationToken ct)
       => Ok(await _mediator.Send(new GetContactOptionsQueryByContactTypes { ContactTypes = param.requestedContactTypes }, ct));

    // GET api/contacts/inspectors
    // Every contact of type PidecInspector (8) or TPInspector (9)
    [HttpGet("inspectors")]
    public async Task<ActionResult<List<ContactDto>>> GetInspectors(CancellationToken ct)
        => Ok(await _mediator.Send(new GetInspectorContactsQuery(), ct));
}

public class queryParams
{
    public string projectId { get; set; }
    public IReadOnlyCollection<int> requestedContactTypes { get; set; }
}

public class queryContactTypesParams
{    
    public IReadOnlyCollection<int> requestedContactTypes { get; set; }
}

public class queryProjectParams
{
    public string projectId { get; set; }
   
}


