using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class AttachmentDto
    {
        public AttachmentDto() { }
        public Guid Id { get; set; }  = Guid.Empty;
        public string? Name { get;  set; } = string.Empty;
        public string? Version { get;  set; } = string.Empty;
        public string? Description { get;  set; } = string.Empty;
        public string? DocNo { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public IFormFile? Content { get; set; } = null;
        public string? ContentType { get; set; } = string.Empty;
        // For client consumption
        public string? DownloadUrl { get; set; } = string.Empty;
        // OR
        public string? Base64Content { get; set; } = string.Empty;
        public string? EdmsId { get; set; } = string.Empty;
        public string? PdfSize { get; set; } = string.Empty;
    }
}
