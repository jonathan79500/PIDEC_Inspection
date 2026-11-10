using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Microsoft.AspNetCore.StaticFiles;
using Inspection.Domain.Entities.Common;
using Inspection.Application.Contracts.Infrastructure;

namespace Inspection.Api.Services;

 

    // Simple implementation: one folder per attachment under App_Data/attachments (or Attachments:RootPath)
    public class LocalDiskAttachmentStore : IAttachmentStore
    {
        private readonly string _root;
        private readonly FileExtensionContentTypeProvider _types = new();

        public LocalDiskAttachmentStore(IConfiguration config, IWebHostEnvironment env)
        {
            _root = Path.GetFullPath(
                config["Attachments:RootPath"] ?? Path.Combine(env.ContentRootPath, "App_Data", "attachments"));
            Directory.CreateDirectory(_root);
        }

        // ---- The only two spots that depend on the shape of your AttachmentId type. Adapt if needed. ----
        private static AttachmentId ToAttachmentId(Guid id) => new AttachmentId(id);
        private static Guid? ToGuid(Document doc) => doc.AttachmentId?.Value;
        // -------------------------------------------------------------------------------------------------

        public async Task<AttachmentId> SaveAsync(IFormFile file, CancellationToken ct)
        {
            var id = Guid.NewGuid();
            var dir = DirFor(id);
            Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, SafeName(file.FileName));
            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream, ct);

            return ToAttachmentId(id);
        }

        public Task<AttachmentInfo?> GetInfoAsync(Document doc, CancellationToken ct)
            => Task.FromResult(Find(doc)?.info);

        public Task<(Stream Content, AttachmentInfo Info)?> OpenAsync(Document doc, CancellationToken ct)
        {
            var found = Find(doc);
            if (found is null) return Task.FromResult<(Stream, AttachmentInfo)?>(null);

            Stream s = new FileStream(found.Value.path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<(Stream, AttachmentInfo)?>((s, found.Value.info));
        }

        public Task DeleteAsync(Document doc, CancellationToken ct)
        {
            var id = ToGuid(doc);
            if (id is not null && Directory.Exists(DirFor(id.Value)))
                Directory.Delete(DirFor(id.Value), recursive: true);
            return Task.CompletedTask;
        }

        private string DirFor(Guid id) => Path.Combine(_root, id.ToString("N"));

        private (string path, AttachmentInfo info)? Find(Document doc)
        {
            var id = ToGuid(doc);
            if (id is null) return null;

            var dir = DirFor(id.Value);
            if (!Directory.Exists(dir)) return null;

            var path = Directory.EnumerateFiles(dir).FirstOrDefault();
            if (path is null) return null;

            var name = Path.GetFileName(path);
            if (!_types.TryGetContentType(name, out var contentType)) contentType = "application/octet-stream";
            return (path, new AttachmentInfo(name, contentType, new FileInfo(path).Length));
        }

        private static string SafeName(string raw)
        {
            var name = Path.GetFileName(raw);
            foreach (var c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            return string.IsNullOrWhiteSpace(name) ? "file" : name;
        }
    }

