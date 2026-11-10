using Inspection.Application.DTOs;
using Inspection.Application.Features.Documents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inspection.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentTypesController(IMediator mediator) => _mediator = mediator;

    // GET api/document-types?contactTypeCode=9
    // Consumer side: active types visible to that contact type, extensions parsed into an array.
    // Used by the "attach a document" form.
    [HttpGet]
    public async Task<ActionResult<List<DocumentTypeDto>>> Get([FromQuery] int? contactTypeCode, CancellationToken ct)
        => Ok(await _mediator.Send(new GetDocumentTypesQuery(contactTypeCode), ct));

    // ===================== admin: manage the catalogue of document types =====================

    // GET api/document-types/admin
    [HttpGet("admin")]
    public async Task<ActionResult<List<DocumentTypeAdminDto>>> GetAllForAdmin(CancellationToken ct)
        => Ok(await _mediator.Send(new GetDocumentTypesAdminQuery(), ct));

    // GET api/document-types/admin/{id}
    [HttpGet("admin/{id:guid}")]
    public async Task<ActionResult<DocumentTypeAdminDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetDocumentTypeByIdQuery(id), ct));

    // POST api/document-types/admin
    [HttpPost("admin")]
    public async Task<ActionResult<DocumentTypeAdminDto>> Create([FromBody] DocumentTypeSaveDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateDocumentTypeCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.DocumentTypeId }, result);
    }

    // PUT api/document-types/admin/{id}
    [HttpPut("admin/{id:guid}")]
    public async Task<ActionResult<DocumentTypeAdminDto>> Update(
        Guid id, [FromBody] DocumentTypeSaveDto dto, CancellationToken ct)
        => Ok(await _mediator.Send(new UpdateDocumentTypeCommand(id, dto), ct));

    // DELETE api/document-types/admin/{id}
    [HttpDelete("admin/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteDocumentTypeCommand(id), ct);
        return NoContent();
    }
}