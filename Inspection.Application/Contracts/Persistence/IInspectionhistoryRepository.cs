using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;   // Revision, Circulation: adjust to your namespace

namespace Inspection.Application.Contracts.Persistence;

public interface IInspectionHistoryRepository
{
    // Every revision submitted for this request, each with its own Circulations loaded
    // (including the Contact navigation needed for PerformerName / SubmittedByName),
    // ordered by RevisionNumber.
    Task<List<Revision>> GetRevisionsByRequestIdAsync(Guid requestId, CancellationToken ct);
}