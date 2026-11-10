using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Infrastructure;

public record AttachmentInfo(string FileName, string ContentType, long SizeBytes);

// Where the file bytes live. Document only holds the AttachmentId, so this is the bridge.
public interface IAttachmentStore
{
    Task<AttachmentId> SaveAsync(IFormFile file, CancellationToken ct);
    Task<AttachmentInfo?> GetInfoAsync(Document doc, CancellationToken ct);
    Task<(Stream Content, AttachmentInfo Info)?> OpenAsync(Document doc, CancellationToken ct);
    Task DeleteAsync(Document doc, CancellationToken ct);
}