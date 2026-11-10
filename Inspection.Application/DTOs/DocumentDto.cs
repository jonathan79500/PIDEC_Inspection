using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs
{
    public class DocumentDto
    {
        public Guid DocumentId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = "";
        public string? Name { get; set; }
        public string? Number { get; set; }
        public string? Status { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Version { get; set; } = "";
        public string FileName { get; set; } = "";
        public long SizeBytes { get; set; }
    }

    public class UploadDocumentRequest
    {
        [Required] public IFormFile File { get; set; } = null!;
        [Required] public DocumentOwnerType OwnerType { get; set; }
        [Required] public Guid OwnerId { get; set; }
        [Required] public Guid DocumentTypeId { get; set; }
        [StringLength(200)] public string? Name { get; set; }
        [StringLength(100)] public string? Number { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
