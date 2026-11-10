using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Exceptions;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Inspectors;

// ===================== requests =====================

// NoAction (2) is the normal "awaiting QC review" queue; pass null for every inspector regardless of state.
public record GetInspectorsByApprovalStateQuery(int? QCApprovalState) : IRequest<List<InspectorDto>>;

public record ApproveInspectorCommand(Guid InspectorId, string? Comment, string ReviewedBy) : IRequest<InspectorDto>;
public record RejectInspectorCommand(Guid InspectorId, string Comment, string ReviewedBy) : IRequest<InspectorDto>;
public record CreateAndApproveInspectorCommand(InspectorSaveDto Data, string ReviewedBy) : IRequest<InspectorDto>;



// ===================== handlers =====================
public class CreateAndApproveInspectorHandler : IRequestHandler<CreateAndApproveInspectorCommand, InspectorDto>
{
    private readonly IMediator _mediator;
    public CreateAndApproveInspectorHandler(IMediator mediator) => _mediator = mediator;

    public async Task<InspectorDto> Handle(CreateAndApproveInspectorCommand request, CancellationToken ct)
    {
        var created = await _mediator.Send(new CreateInspectorCommand(request.Data), ct);
        return await _mediator.Send(new ApproveInspectorCommand(created.InspectorId, null, request.ReviewedBy), ct);
    }
}


public class GetInspectorsByApprovalStateHandler : IRequestHandler<GetInspectorsByApprovalStateQuery, List<InspectorDto>>
{
    private readonly IInspectorRepository _repository;
    public GetInspectorsByApprovalStateHandler(IInspectorRepository repository) => _repository = repository;

    public async Task<List<InspectorDto>> Handle(GetInspectorsByApprovalStateQuery request, CancellationToken ct)
    {
        var inspectors = await _repository.GetByApprovalStateAsync(request.QCApprovalState, ct);
        return inspectors.Adapt<List<InspectorDto>>();
    }
}

public class ApproveInspectorHandler : IRequestHandler<ApproveInspectorCommand, InspectorDto>
{
    private readonly IInspectorRepository _repository;
    public ApproveInspectorHandler(IInspectorRepository repository) => _repository = repository;

    public async Task<InspectorDto> Handle(ApproveInspectorCommand request, CancellationToken ct)
    {
        var inspector = await _repository.GetForUpdateAsync(request.InspectorId, ct)
                        ?? throw new NotFoundException("Inspector not found.");

        inspector.QCApprovalState = (int)InspectorApprovalState.Approved;
        inspector.QCReviewedBy = request.ReviewedBy;
        inspector.QCReviewedDate = DateTime.UtcNow;
        inspector.QCComment = request.Comment;
        inspector.VersionNumber += 1;

        await _repository.SaveAsync(ct);

        var saved = await _repository.GetByIdAsync(inspector.InspectorId, ct);
        return saved!.Adapt<InspectorDto>();
    }
}

public class RejectInspectorHandler : IRequestHandler<RejectInspectorCommand, InspectorDto>
{
    private readonly IInspectorRepository _repository;
    public RejectInspectorHandler(IInspectorRepository repository) => _repository = repository;

    public async Task<InspectorDto> Handle(RejectInspectorCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Comment))
            throw new BusinessRuleException("A reason is required when rejecting an inspector.");

        var inspector = await _repository.GetForUpdateAsync(request.InspectorId, ct)
                        ?? throw new NotFoundException("Inspector not found.");

        inspector.QCApprovalState = (int)InspectorApprovalState.Rejected;
        inspector.QCReviewedBy = request.ReviewedBy;
        inspector.QCReviewedDate = DateTime.UtcNow;
        inspector.QCComment = request.Comment.Trim();
        inspector.VersionNumber += 1;

        await _repository.SaveAsync(ct);

        var saved = await _repository.GetByIdAsync(inspector.InspectorId, ct);
        return saved!.Adapt<InspectorDto>();
    }
}