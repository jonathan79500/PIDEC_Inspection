using Inspection.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Persistence;

// Tells the document service whether an owner exists and which contact type it belongs to
// (the contact type decides which document types are visible).
public interface IDocumentOwnerResolver
{
    // ContactTypeCode == null means "no filtering: every active document type is allowed"
    Task<(bool Exists, int? ContactTypeCode)> ResolveAsync(DocumentOwnerType type, Guid ownerId, CancellationToken ct);
}
