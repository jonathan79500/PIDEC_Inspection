using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Exceptions;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.Inspector;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Inspectors;


// ===================== requests =====================
public record GetInspectorsQuery(Guid? InspectionCompanyContactId = null, int? QCApprovalState = null): IRequest<List<InspectorDto>>;
public record GetInspectorByIdQuery(Guid InspectorId) : IRequest<InspectorDto>;
    public record CreateInspectorCommand(InspectorSaveDto Data) : IRequest<InspectorDto>;
    public record UpdateInspectorCommand(Guid InspectorId, InspectorSaveDto Data) : IRequest<InspectorDto>;
    public record DeleteInspectorCommand(Guid InspectorId) : IRequest;

// ===================== handlers =====================
public class GetInspectorsHandler : IRequestHandler<GetInspectorsQuery, List<InspectorDto>>
{
    private readonly IInspectorRepository _repository;
    public GetInspectorsHandler(IInspectorRepository repository) => _repository = repository;

    public async Task<List<InspectorDto>> Handle(GetInspectorsQuery request, CancellationToken ct)
    {
        var inspectors = request.InspectionCompanyContactId is null && request.QCApprovalState is null
            ? await _repository.GetAllAsync(ct)
            : await _repository.GetFilteredAsync(request.InspectionCompanyContactId, request.QCApprovalState, ct);
        return inspectors.Adapt<List<InspectorDto>>();
    }
}

public class GetInspectorByIdHandler : IRequestHandler<GetInspectorByIdQuery, InspectorDto>
    {
        private readonly IInspectorRepository _repository;
        public GetInspectorByIdHandler(IInspectorRepository repository) => _repository = repository;

        public async Task<InspectorDto> Handle(GetInspectorByIdQuery request, CancellationToken ct)
        {
            var inspector = await _repository.GetByIdAsync(request.InspectorId, ct)
                            ?? throw new NotFoundException("Inspector not found.");
            return inspector.Adapt<InspectorDto>();
        }
    }

    public class CreateInspectorHandler : IRequestHandler<CreateInspectorCommand, InspectorDto>
    {
        private readonly IInspectorRepository _repository;
        public CreateInspectorHandler(IInspectorRepository repository) => _repository = repository;

        public async Task<InspectorDto> Handle(CreateInspectorCommand request, CancellationToken ct)
        {
            var dto = request.Data;

            if (!await _repository.ContactExistsAsync(dto.InspectionCompanyContactId, ct))
                throw new BusinessRuleException("The selected contact does not exist.");

            var skillError = await InspectorRules.ValidateSkillsAsync(_repository, dto.Skills, new HashSet<Guid>(), ct);
            if (skillError is not null) throw new BusinessRuleException(skillError);

            var inspector = dto.Adapt<Inspector>();          // see MappingConfig
            inspector.InspectorId = Guid.NewGuid();
            inspector.CreatedDate = DateTime.UtcNow;
            inspector.VersionNumber = 1;
            _repository.ReplaceSkills(inspector, dto.Skills.Adapt<List<InspectorSkill>>());

            await _repository.AddAsync(inspector, ct);

            var saved = await _repository.GetByIdAsync(inspector.InspectorId, ct);
            return saved!.Adapt<InspectorDto>();
        }
    }

    public class UpdateInspectorHandler : IRequestHandler<UpdateInspectorCommand, InspectorDto>
    {
        private readonly IInspectorRepository _inspectorRepository;
        public UpdateInspectorHandler(IInspectorRepository inspectorRepository) => _inspectorRepository = inspectorRepository;

        public async Task<InspectorDto> Handle(UpdateInspectorCommand request, CancellationToken ct)
        {
            var dto = request.Data;

            var inspector = await _inspectorRepository.GetForUpdateAsync(request.InspectorId, ct)
                            ?? throw new NotFoundException("Inspector not found.");

            if (!await _inspectorRepository.ContactExistsAsync(dto.InspectionCompanyContactId, ct))
                throw new BusinessRuleException("The selected contact does not exist.");

            var existing = inspector.Skills.Select(s => s.SkillId).ToHashSet();
            var skillError = await InspectorRules.ValidateSkillsAsync(_inspectorRepository, dto.Skills, existing, ct);
            if (skillError is not null) throw new BusinessRuleException(skillError);

            dto.Adapt(inspector);                            // copies the form fields onto the tracked entity
        _inspectorRepository.ReplaceSkills(inspector, dto.Skills.Adapt<List<InspectorSkill>>());
            inspector.VersionNumber += 1;

            await _inspectorRepository.SaveAsync(ct);

            var saved = await _inspectorRepository.GetByIdAsync(inspector.InspectorId, ct);
            return saved!.Adapt<InspectorDto>();
        }
    }

// Also removes the inspector's skills (cascade) and documents
public class DeleteInspectorHandler : IRequestHandler<DeleteInspectorCommand>
{
    private readonly IInspectorRepository _inspectorRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IAttachmentStore _attachmentStore;

    public DeleteInspectorHandler(
        IInspectorRepository inspectorRepository,
        IDocumentRepository documentRepository,
        IAttachmentStore attachmentStore)
    {
        _inspectorRepository = inspectorRepository;
        _documentRepository = documentRepository;
        _attachmentStore = attachmentStore;
    }

    public async Task Handle(DeleteInspectorCommand request, CancellationToken ct)
    {
        var inspector = await _inspectorRepository.GetForUpdateAsync(request.InspectorId, ct)
                        ?? throw new NotFoundException("Inspector not found.");

        // Stages the document removals on the same scoped DbContext, so the single
        // SaveChanges inside DeleteAsync removes the inspector and its document links together.
        var orphaned = await _documentRepository.StageRemoveAllForOwnerAsync(
            DocumentOwnerType.Inspector, request.InspectorId, ct);

        await _inspectorRepository.DeleteAsync(inspector, ct);

        // Remove the files only after the database change succeeded
        foreach (var document in orphaned)
            await _attachmentStore.DeleteAsync(document, ct);
    }
}

// ===================== rules =====================
public static class InspectorRules
    {
        public static async Task<string?> ValidateSkillsAsync(
            IInspectorRepository repository, List<InspectorSkillSaveDto> skills,
            HashSet<Guid> existingSkillIds, CancellationToken ct)
        {
            if (skills.Count == 0) return "An inspector needs at least one skill.";
            if (skills.GroupBy(s => s.SkillId).Any(g => g.Count() > 1)) return "A skill can only be added once.";

            // Skills the inspector already has stay valid even if they were deactivated later
            var newIds = skills.Select(s => s.SkillId).Where(id => !existingSkillIds.Contains(id)).ToList();
            if (newIds.Count == 0) return null;

            var okCount = await repository.CountActiveSkillsAsync(newIds, ct);
            return okCount == newIds.Count ? null : "One or more selected skills do not exist or are inactive.";
        }
    }
