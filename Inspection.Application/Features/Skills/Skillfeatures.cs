using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Exceptions;
using Inspection.Domain.Entities.Common;   // InspectionSkill: adjust to your namespace
using Inspection.Domain.Entities.Inspector;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Skills;

// ===================== requests =====================
public record GetSkillsQuery : IRequest<List<InspectionSkillDto>>;
public record GetActiveSkillsQuery : IRequest<List<SkillOptionDto>>;     // used by the inspector form's dropdown
public record GetSkillByIdQuery(Guid SkillId) : IRequest<InspectionSkillDto>;
public record CreateSkillCommand(SkillSaveDto Data) : IRequest<InspectionSkillDto>;
public record UpdateSkillCommand(Guid SkillId, SkillSaveDto Data) : IRequest<InspectionSkillDto>;
public record DeleteSkillCommand(Guid SkillId) : IRequest;

// ===================== handlers =====================
public class GetSkillsHandler : IRequestHandler<GetSkillsQuery, List<InspectionSkillDto>>
{
    private readonly ISkillRepository _repository;
    public GetSkillsHandler(ISkillRepository repository) => _repository = repository;

    public async Task<List<InspectionSkillDto>> Handle(GetSkillsQuery request, CancellationToken ct)
    {
        var skills = await _repository.GetAllAsync(ct);
        var dtos = skills.Adapt<List<InspectionSkillDto>>();

        // one count call per skill is fine here: this list is small and rarely requested
        foreach (var dto in dtos)
            dto.InspectorCount = await _repository.CountInspectorsUsingAsync(dto.SkillId, ct);

        return dtos;
    }
}

public class GetActiveSkillsHandler : IRequestHandler<GetActiveSkillsQuery, List<SkillOptionDto>>
{
    private readonly ISkillRepository _repository;
    public GetActiveSkillsHandler(ISkillRepository repository) => _repository = repository;

    public async Task<List<SkillOptionDto>> Handle(GetActiveSkillsQuery request, CancellationToken ct)
    {
        var skills = await _repository.GetActiveAsync(ct);
        return skills.Adapt<List<SkillOptionDto>>();
    }
}

public class GetSkillByIdHandler : IRequestHandler<GetSkillByIdQuery, InspectionSkillDto>
{
    private readonly ISkillRepository _repository;
    public GetSkillByIdHandler(ISkillRepository repository) => _repository = repository;

    public async Task<InspectionSkillDto> Handle(GetSkillByIdQuery request, CancellationToken ct)
    {
        var skill = await _repository.GetByIdAsync(request.SkillId, ct)
                    ?? throw new NotFoundException("Skill not found.");

        var dto = skill.Adapt<InspectionSkillDto>();
        dto.InspectorCount = await _repository.CountInspectorsUsingAsync(skill.SkillId, ct);
        return dto;
    }
}

public class CreateSkillHandler : IRequestHandler<CreateSkillCommand, InspectionSkillDto>
{
    private readonly ISkillRepository _repository;
    public CreateSkillHandler(ISkillRepository repository) => _repository = repository;

    public async Task<InspectionSkillDto> Handle(CreateSkillCommand request, CancellationToken ct)
    {
        var dto = request.Data;
        var code = dto.Code.Trim().ToUpperInvariant();

        if (await _repository.CodeExistsAsync(code, excludingSkillId: null, ct))
            throw new BusinessRuleException($"A skill with code \"{code}\" already exists.");

        var skill = new InspectionSkill
        {
            SkillId = Guid.NewGuid(),
            Code = code,
            Name = dto.Name.Trim(),
            IsActive = dto.IsActive
        };

        await _repository.AddAsync(skill, ct);

        return skill.Adapt<InspectionSkillDto>();   // InspectorCount is 0 on a brand new skill
    }
}

public class UpdateSkillHandler : IRequestHandler<UpdateSkillCommand, InspectionSkillDto>
{
    private readonly ISkillRepository _repository;
    public UpdateSkillHandler(ISkillRepository repository) => _repository = repository;

    public async Task<InspectionSkillDto> Handle(UpdateSkillCommand request, CancellationToken ct)
    {
        var dto = request.Data;
        var code = dto.Code.Trim().ToUpperInvariant();

        var skill = await _repository.GetByIdAsync(request.SkillId, ct)
                    ?? throw new NotFoundException("Skill not found.");

        if (await _repository.CodeExistsAsync(code, excludingSkillId: skill.SkillId, ct))
            throw new BusinessRuleException($"A skill with code \"{code}\" already exists.");

        skill.Code = code;
        skill.Name = dto.Name.Trim();
        skill.IsActive = dto.IsActive;

        await _repository.SaveAsync(ct);

        var result = skill.Adapt<InspectionSkillDto>();
        result.InspectorCount = await _repository.CountInspectorsUsingAsync(skill.SkillId, ct);
        return result;
    }
}

// A skill in use by any inspector is deactivated instead of removed, since InspectorSkills has no cascade
// from InspectionSkills (only from Inspectors), so the database would otherwise reject the delete.
public class DeleteSkillHandler : IRequestHandler<DeleteSkillCommand>
{
    private readonly ISkillRepository _repository;
    public DeleteSkillHandler(ISkillRepository repository) => _repository = repository;

    public async Task Handle(DeleteSkillCommand request, CancellationToken ct)
    {
        var skill = await _repository.GetByIdAsync(request.SkillId, ct)
                    ?? throw new NotFoundException("Skill not found.");

        var inUse = await _repository.CountInspectorsUsingAsync(skill.SkillId, ct);
        if (inUse > 0)
            throw new BusinessRuleException(
                $"This skill is assigned to {inUse} inspector(s) and cannot be deleted. Deactivate it instead.");

        await _repository.DeleteAsync(skill, ct);
    }
}