using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification;   // Revision, Circulation: adjust to your namespace
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionHistory;

public record GetInspectionHistoryQuery(Guid RequestId) : IRequest<List<RevisionHistoryDto>>;

public class GetInspectionHistoryHandler : IRequestHandler<GetInspectionHistoryQuery, List<RevisionHistoryDto>>
{
    private readonly IInspectionHistoryRepository _repository;
    public GetInspectionHistoryHandler(IInspectionHistoryRepository repository) => _repository = repository;

    public async Task<List<RevisionHistoryDto>> Handle(GetInspectionHistoryQuery request, CancellationToken ct)
    {
        var revisions = await _repository.GetRevisionsByRequestIdAsync(request.RequestId, ct);

        return revisions
            .OrderBy(r => r.RevisionNumber)
            .Select(ToDto)
            .ToList();
    }

    private static RevisionHistoryDto ToDto(Revision revision) => new()
    {
        RevisionId = revision.RevisionId,
        VersionNumber = revision.VersionNumber,
        RevisionNumber = revision.RevisionNumber,
        SubmittedDate = revision.SubmittedDate,
        SubmittedByName = revision.SubmittedBy?.Name,
        Comment = revision.Comment,
        Status = revision.Status,
        IsFinal = revision.IsFinal,
        // Only the top-level circulations go in the tree root; BuildTree finds each one's
        // children by ParentId, however deep the chain actually goes.
        Circulations = BuildTree(revision.Circulations.ToList(), parentId: null),
    };

    private static List<CirculationHistoryDto> BuildTree(List<Circulation> all, Guid? parentId) =>
        all
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.SequenceNo)
            .Select(c => new CirculationHistoryDto
            {
                Id = c.Id,
                ParentId = c.ParentId,
                PerformerName = c.PerformerContactPerson?.Name,
                PerformerContactType = c.PerformerContactType,
                SequenceNo = c.SequenceNo,
                ActionType = c.ActionType,
                QCActionType = c.QCActionType,
                PreviousActionType = c.PreviousActionType,
                PerformedActionDate = c.PerformedActionDate,
                PerformedActionDateGregorian = c.PerformedActionDateGregorian,
                Comment = c.Comment,
                CreatorComment = c.CreatorComment,
                ChildCirculations = BuildTree(all, c.Id),   // recursive: supports any depth, not just one level
            })
            .ToList();
}