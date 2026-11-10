using Inspection.Domain.Entities.Inspector.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Inspector
{
    public class SkillOptionDto
    {
        public Guid SkillId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class ContactOptionDto
    {
        public Guid ContactId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ContactTypeCode { get; set; }
    }

    public class InspectorSaveDto
    {
        [Required]
        public Guid InspectionCompanyContactId { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(200)] public string? Name { get; set; }
        [Required, StringLength(100)] public string? FirstName { get; set; }
        [Required, StringLength(100)] public string? LastName { get; set; }
        [StringLength(2000)] public string? Description { get; set; }
        [StringLength(200)] public string? Position { get; set; }
        [StringLength(30)] public string? Phone { get; set; }
        [EmailAddress, StringLength(200)] public string? Email { get; set; }

        [Range(0, 2)]
        public int QCApprovalState { get; set; } = 2;

        public DateTime ExpireDate { get; set; }

        [MinLength(1, ErrorMessage = "An inspector needs at least one skill.")]
        public List<InspectorSkillSaveDto> Skills { get; set; } = new();
    }

    public class InspectorSkillSaveDto
    {
        [Required] public Guid SkillId { get; set; }
        [EnumDataType(typeof(InspectorSkillLevel))]
        public InspectorSkillLevel Level { get; set; } = InspectorSkillLevel.Experienced;
    }
}
