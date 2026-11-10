using Inspection.Application.DTOs;
using Inspection.Application.Features.InspectionHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inspection.Api.Controllers;

[ApiController]
[Route("inspection-history")]
public class InspectionHistoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InspectionHistoryController(IMediator mediator) => _mediator = mediator;

    // GET api/inspection-history/{requestId}
    [HttpGet("{requestId:guid}")]
    public async Task<ActionResult<List<RevisionHistoryDto>>> GetByRequestId(Guid requestId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetInspectionHistoryQuery(requestId), ct));
}