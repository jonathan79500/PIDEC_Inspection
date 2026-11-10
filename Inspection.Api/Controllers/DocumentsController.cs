using Inspection.Api.Services;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.Features.BasicInfo.Documents;
using Inspection.Application.Features.Documents;
using Inspection.Domain.Entities.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inspection.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator) => _mediator = mediator;

    // GET api/documents?ownerType=Inspector&ownerId=...
    [HttpGet]
    public async Task<ActionResult<List<DocumentDto>>> List(
        [FromQuery] DocumentOwnerType ownerType, [FromQuery] Guid ownerId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetDocumentsQuery(ownerType, ownerId), ct));

    // POST api/documents   (multipart/form-data)
    // The size is checked against DocumentType.MaxSizeMB inside the handler's repository, so the global limits are lifted here.
    [HttpPost]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public async Task<ActionResult<DocumentDto>> Upload([FromForm] UploadDocumentRequest req, CancellationToken ct)
        => Ok(await _mediator.Send(new UploadDocumentCommand(req), ct));

    // GET api/documents/{id}/download?ownerType=Inspector&ownerId=...
    [HttpGet("{documentId:guid}/download")]
    public async Task<IActionResult> Download(
        Guid documentId, [FromQuery] DocumentOwnerType ownerType, [FromQuery] Guid ownerId, CancellationToken ct)
    {
        var file = await _mediator.Send(new DownloadDocumentQuery(documentId, ownerType, ownerId), ct);
        return File(file.Content, file.ContentType, file.FileName);
    }

    // DELETE api/documents/{id}?ownerType=Inspector&ownerId=...
    [HttpDelete("{documentId:guid}")]
    public async Task<IActionResult> Delete(
        Guid documentId, [FromQuery] DocumentOwnerType ownerType, [FromQuery] Guid ownerId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteDocumentCommand(documentId, ownerType, ownerId), ct);
        return NoContent();
    }
}


//[ApiController]
//[Route("api/documents")]
//public class DocumentsController : ControllerBase
//{
//    private readonly IDocumentRepository _documentRepository;

//    public DocumentsController(IDocumentRepository documentRepository) => _documentRepository = documentRepository;

//    // GET api/documents?ownerType=Inspector&ownerId=...
//    [HttpGet]
//    public async Task<ActionResult<List<DocumentDto>>> List(
//        [FromQuery] DocumentOwnerType ownerType, [FromQuery] Guid ownerId, CancellationToken ct)
//        => Ok(await _documentRepository.ListAsync(ownerType, ownerId, ct));

//    // POST api/documents   (multipart/form-data)
//    // The size is checked against DocumentType.MaxSizeMB inside the service, so the global limits are lifted here.
//    [HttpPost]
//    [DisableRequestSizeLimit]
//    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
//    public async Task<ActionResult<DocumentDto>> Upload([FromForm] UploadDocumentRequest req, CancellationToken ct)
//    {
//        try
//        {
//            return Ok(await _documentRepository.UploadAsync(req, ct));
//        }
//        catch (DocumentRuleException ex)
//        {
//            return Fail(ex);
//        }
//    }

//    // GET api/documents/{id}/download?ownerType=Inspector&ownerId=...
//    [HttpGet("{documentId:guid}/download")]
//    public async Task<IActionResult> Download(
//        Guid documentId, [FromQuery] DocumentOwnerType ownerType, [FromQuery] Guid ownerId, CancellationToken ct)
//    {
//        try
//        {
//            var (content, info) = await _documentRepository.OpenAsync(documentId, ownerType, ownerId, ct);
//            return File(content, info.ContentType, info.FileName);
//        }
//        catch (DocumentRuleException ex)
//        {
//            return Fail(ex);
//        }
//    }

//    // DELETE api/documents/{id}?ownerType=Inspector&ownerId=...
//    [HttpDelete("{documentId:guid}")]
//    public async Task<IActionResult> Delete(
//        Guid documentId, [FromQuery] DocumentOwnerType ownerType, [FromQuery] Guid ownerId, CancellationToken ct)
//    {
//        try
//        {
//            await _documentRepository.DeleteAsync(documentId, ownerType, ownerId, ct);
//            return NoContent();
//        }
//        catch (DocumentRuleException ex)
//        {
//            return Fail(ex);
//        }
//    }

//    private ActionResult Fail(DocumentRuleException ex) =>
//        ex.NotFound ? NotFound(new { error = ex.Message }) : BadRequest(new { error = ex.Message });
//}