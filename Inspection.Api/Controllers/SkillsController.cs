using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Features.Skills;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inspection.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkillsController(IMediator mediator) => _mediator = mediator;

    // GET api/skills
    // Full list, for a skills-management screen (code, name, active, how many inspectors use it)
    [HttpGet]
    public async Task<ActionResult<List<InspectionSkillDto>>> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetSkillsQuery(), ct));

    // GET api/skills/active
    // Lightweight list, for the inspector form's skill dropdown
    [HttpGet("active")]
    public async Task<ActionResult<List<SkillOptionDto>>> GetActive(CancellationToken ct)
        => Ok(await _mediator.Send(new GetActiveSkillsQuery(), ct));

    // GET api/skills/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InspectionSkillDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetSkillByIdQuery(id), ct));

    // POST api/skills
    [HttpPost]
    public async Task<ActionResult<InspectionSkillDto>> Create([FromBody] SkillSaveDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSkillCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.SkillId }, result);
    }

    // PUT api/skills/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InspectionSkillDto>> Update(Guid id, [FromBody] SkillSaveDto dto, CancellationToken ct)
        => Ok(await _mediator.Send(new UpdateSkillCommand(id, dto), ct));

    // DELETE api/skills/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteSkillCommand(id), ct);
        return NoContent();
    }
}