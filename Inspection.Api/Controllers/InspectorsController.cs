using Inspection.Api.Services;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Features.Inspectors;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.Inspector;
using Inspection.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Inspection.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class InspectorsController : ControllerBase
{
    private readonly ISender _mediator;
    private IWebHostEnvironment webHostEnvironment;
    private IFileService _fileService;

    public InspectorsController(IMediator mediator, IWebHostEnvironment _webHostEnvironment, IFileService fileService)
    {
        _mediator = mediator;
        this.webHostEnvironment = _webHostEnvironment;
        _fileService = fileService;
    }

    // GET api/inspectors
    [HttpGet]
    public async Task<ActionResult<List<InspectorDto>>> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetInspectorsQuery(), ct));

    // GET api/inspectors/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InspectorDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetInspectorByIdQuery(id), ct));

    // POST api/inspectors
    [HttpPost]
    [Route("Create")]
    public async Task<ActionResult<InspectorDto>> Create([FromBody] InspectorSaveDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateInspectorCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.InspectorId }, result);
    }

    // PUT api/inspectors/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InspectorDto>> Update(Guid id, [FromBody] InspectorSaveDto dto, CancellationToken ct)
        => Ok(await _mediator.Send(new UpdateInspectorCommand(id, dto), ct));

    // DELETE api/inspectors/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteInspectorCommand(id), ct);
        return NoContent();
    }



    // GET api/inspectors/pending
    // The QC review queue: every inspector still awaiting a decision
    [HttpGet("pending")]
    public async Task<ActionResult<List<InspectorDto>>> GetPending(CancellationToken ct)
        => Ok(await _mediator.Send(new GetInspectorsByApprovalStateQuery((int)InspectorApprovalState.NoAction), ct));

    // POST api/inspectors/{id}/approve
    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "QC")]   // adjust to your actual role/policy name
    public async Task<ActionResult<InspectorDto>> Approve(Guid id, [FromBody] InspectorReviewDto dto, CancellationToken ct)
    {
        var reviewedBy = User.Identity?.Name ?? "unknown";
        return Ok(await _mediator.Send(new ApproveInspectorCommand(id, dto.Comment, reviewedBy), ct));
    }

    // POST api/inspectors/{id}/reject
    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "QC")]   // adjust to your actual role/policy name
    public async Task<ActionResult<InspectorDto>> Reject(Guid id, [FromBody] InspectorReviewDto dto, CancellationToken ct)
    {
        var reviewedBy = User.Identity?.Name ?? "unknown";
        return Ok(await _mediator.Send(new RejectInspectorCommand(id, dto.Comment ?? "", reviewedBy), ct));
    }

}

//[ApiController]
//[Route("api/[controller]")]
//public class InspectorsController : ControllerBase
//{
//    private readonly InspectionDbContext _db;
//    private readonly DocumentService _documents;

//    public InspectorsController(InspectionDbContext db, DocumentService documents)
//    {
//        _db = db;
//        _documents = documents;
//    }

//    // Must be an Expression (not a normal method) so EF Core can translate it to SQL
//    private static readonly Expression<Func<Inspector, InspectorDto>> ToDto = i => new InspectorDto
//    {
//        InspectorId = i.InspectorId,
//        InspectionCompanyContactId = i.InspectionCompanyContactId,
//        InspectionCompanyContactName = i.InspectionCompanyContact.Name,
//        IsActive = i.IsActive,
//        Name = i.Name,
//        FirstName = i.FirstName,
//        LastName = i.LastName,
//        Description = i.Description,
//        Position = i.Position,
//        Phone = i.Phone,
//        Email = i.Email,
//        QCApprovalState = i.QCApprovalState,
//        VersionNumber = i.VersionNumber,
//        CreatedDate = i.CreatedDate,
//        ExpireDate = i.ExpireDate,
//        Skills = i.Skills
//            .OrderBy(s => s.Skill.Code)
//            .Select(s => new InspectorSkillDto
//            {
//                SkillId = s.SkillId,
//                Code = s.Skill.Code,
//                Name = s.Skill.Name,
//                Level = s.Level
//            })
//            .ToList()
//    };

//    // GET api/inspectors
//    [HttpGet]
//    public async Task<ActionResult<List<InspectorDto>>> GetAll(CancellationToken ct)
//    {
//        var items = await _db.Inspectors
//            .AsNoTracking()
//            .OrderBy(i => i.LastName).ThenBy(i => i.FirstName)
//            .Select(ToDto)
//            .ToListAsync(ct);

//        return Ok(items);
//    }

//    // GET api/inspectors/{id}
//    [HttpGet("{id:guid}")]
//    public async Task<ActionResult<InspectorDto>> GetById(Guid id, CancellationToken ct)
//    {
//        var item = await _db.Inspectors
//            .AsNoTracking()
//            .Where(i => i.InspectorId == id)
//            .Select(ToDto)
//            .FirstOrDefaultAsync(ct);

//        return item is null ? NotFound() : Ok(item);
//    }

//    // POST api/inspectors
//    [HttpPost]
//    public async Task<ActionResult<InspectorDto>> Create(InspectorSaveDto dto, CancellationToken ct)
//    {
//        if (!await _db.Contacts.AnyAsync(c => c.ContactId == dto.InspectionCompanyContactId, ct))
//            return BadRequest(new { error = "The selected contact does not exist." });

//        var skillError = await ValidateSkillsAsync(dto.Skills, existingSkillIds: new HashSet<Guid>(), ct);
//        if (skillError is not null) return BadRequest(new { error = skillError });

//        var entity = new Inspector
//        {
//            InspectorId = Guid.NewGuid(),
//            CreatedDate = DateTime.UtcNow,
//            VersionNumber = 1
//        };
//        Apply(entity, dto);
//        SyncSkills(entity, dto.Skills);

//        _db.Inspectors.Add(entity);
//        await _db.SaveChangesAsync(ct);

//        var result = await _db.Inspectors.AsNoTracking()
//            .Where(i => i.InspectorId == entity.InspectorId)
//            .Select(ToDto)
//            .FirstAsync(ct);

//        return CreatedAtAction(nameof(GetById), new { id = entity.InspectorId }, result);
//    }

//    // PUT api/inspectors/{id}
//    [HttpPut("{id:guid}")]
//    public async Task<ActionResult<InspectorDto>> Update(Guid id, InspectorSaveDto dto, CancellationToken ct)
//    {
//        var entity = await _db.Inspectors
//            .Include(i => i.Skills)
//            .FirstOrDefaultAsync(i => i.InspectorId == id, ct);
//        if (entity is null) return NotFound();

//        if (!await _db.Contacts.AnyAsync(c => c.ContactId == dto.InspectionCompanyContactId, ct))
//            return BadRequest(new { error = "The selected contact does not exist." });

//        var existing = entity.Skills.Select(s => s.SkillId).ToHashSet();
//        var skillError = await ValidateSkillsAsync(dto.Skills, existing, ct);
//        if (skillError is not null) return BadRequest(new { error = skillError });

//        Apply(entity, dto);
//        SyncSkills(entity, dto.Skills);
//        entity.VersionNumber += 1;

//        await _db.SaveChangesAsync(ct);

//        var result = await _db.Inspectors.AsNoTracking()
//            .Where(i => i.InspectorId == id)
//            .Select(ToDto)
//            .FirstAsync(ct);

//        return Ok(result);
//    }

//    // DELETE api/inspectors/{id}  (also removes the inspector's skills and attachments)
//    [HttpDelete("{id:guid}")]
//    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
//    {
//        var entity = await _db.Inspectors.FirstOrDefaultAsync(i => i.InspectorId == id, ct);
//        if (entity is null) return NotFound();

//        // Unlink the inspector's documents (documents nobody else uses are deleted too)
//        var orphaned = await _documents.StageRemoveAllForOwnerAsync(DocumentOwnerType.Inspector, id, ct);

//        _db.Inspectors.Remove(entity);      // InspectorSkill rows are removed by cascade
//        await _db.SaveChangesAsync(ct);

//        // Remove the files only after the database change succeeded
//        await _documents.DeleteFilesAsync(orphaned, ct);

//        return NoContent();
//    }

//    // ---------- helpers ----------
//    private static void Apply(Inspector entity, InspectorSaveDto dto)
//    {
//        var first = dto.FirstName?.Trim();
//        var last = dto.LastName?.Trim();

//        entity.InspectionCompanyContactId = dto.InspectionCompanyContactId;
//        entity.IsActive = dto.IsActive;
//        entity.FirstName = first;
//        entity.LastName = last;
//        entity.Name = string.IsNullOrWhiteSpace(dto.Name) ? $"{first} {last}".Trim() : dto.Name.Trim();
//        entity.Description = dto.Description;
//        entity.Position = dto.Position;
//        entity.Phone = dto.Phone;
//        entity.Email = dto.Email;
//        entity.QCApprovalState = dto.QCApprovalState;
//        entity.ExpireDate = dto.ExpireDate.ToUniversalTime();
//    }

//    private async Task<string?> ValidateSkillsAsync(
//        List<InspectorSkillSaveDto> skills, HashSet<Guid> existingSkillIds, CancellationToken ct)
//    {
//        if (skills.Count == 0) return "An inspector needs at least one skill.";
//        if (skills.GroupBy(s => s.SkillId).Any(g => g.Count() > 1)) return "A skill can only be added once.";

//        // Skills the inspector already has stay valid even if they were deactivated later
//        var newIds = skills.Select(s => s.SkillId).Where(id => !existingSkillIds.Contains(id)).ToList();
//        if (newIds.Count == 0) return null;

//        var okCount = await _db.InspectionSkills.CountAsync(s => newIds.Contains(s.SkillId) && s.IsActive, ct);
//        return okCount == newIds.Count ? null : "One or more selected skills do not exist or are inactive.";
//    }

//    // Makes entity.Skills match the incoming list: remove, update level, add
//    private void SyncSkills(Inspector entity, List<InspectorSkillSaveDto> incoming)
//    {
//        var wanted = incoming.ToDictionary(s => s.SkillId, s => s.Level);

//        var toRemove = entity.Skills.Where(s => !wanted.ContainsKey(s.SkillId)).ToList();
//        foreach (var r in toRemove)
//        {
//            entity.Skills.Remove(r);
//            _db.InspectorSkills.Remove(r);
//        }

//        foreach (var s in entity.Skills)
//            s.Level = wanted[s.SkillId];

//        var have = entity.Skills.Select(s => s.SkillId).ToHashSet();
//        foreach (var (skillId, level) in wanted)
//        {
//            if (have.Contains(skillId)) continue;
//            entity.Skills.Add(new InspectorSkill
//            {
//                InspectorSkillId = Guid.NewGuid(),
//                SkillId = skillId,
//                Level = level
//            });
//        }
//    }
//}