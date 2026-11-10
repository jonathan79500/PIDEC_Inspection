using Inspection.Application.Contracts.Infrastructure;   // AttachmentInfo: adjust to your namespace
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Features.BasicInfo.Documents;
using Inspection.Application.Features.Documents;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.Inspector.Enums;
using Mapster;

namespace Inspection.Application.Tests.Mapping;

public class MappingTests
{
    [Fact]
    public void All_mapping_configurations_compile()
    {
        // Fails here (instead of at the first request) when a mapping is misconfigured
        TypeAdapterConfig.GlobalSettings.Compile();
    }

    // ---------- InspectorSaveDto -> Inspector ----------
    private static InspectorSaveDto SaveDto() => new()
    {
        InspectionCompanyContactId = Guid.NewGuid(),
        FirstName = "  Ada ",
        LastName = " Lovelace  ",
        ExpireDate = new DateTime(2027, 3, 1, 0, 0, 0, DateTimeKind.Unspecified),
        Skills = new List<InspectorSkillSaveDto>
        {
            new() { SkillId = Guid.NewGuid(), Level = InspectorSkillLevel.Professional }
        }
    };

    [Fact]
    public void SaveDto_trims_the_names_and_builds_the_display_name_when_it_is_empty()
    {
        var inspector = SaveDto().Adapt<Inspector>();

        Assert.Equal("Ada", inspector.FirstName);
        Assert.Equal("Lovelace", inspector.LastName);
        Assert.Equal("Ada Lovelace", inspector.Name);
    }

    [Fact]
    public void SaveDto_keeps_a_given_display_name_trimmed()
    {
        var dto = SaveDto();
        dto.Name = "  Countess ";

        var inspector = dto.Adapt<Inspector>();

        Assert.Equal("Countess", inspector.Name);
    }

    [Fact]
    public void SaveDto_stores_the_expiry_date_as_utc()
    {
        var inspector = SaveDto().Adapt<Inspector>();

        Assert.Equal(DateTimeKind.Utc, inspector.ExpireDate.Kind);
    }

    [Fact]
    public void SaveDto_does_not_touch_the_fields_the_server_controls()
    {
        var inspector = SaveDto().Adapt<Inspector>();

        Assert.Equal(Guid.Empty, inspector.InspectorId);
        Assert.Equal(0, inspector.VersionNumber);
        Assert.Empty(inspector.Skills);       // skills are synchronised by the repository, not mapped here
    }

    [Fact]
    public void SaveDto_mapped_onto_an_existing_inspector_keeps_its_id_and_version()
    {
        var id = Guid.NewGuid();
        var existing = new Inspector { InspectorId = id, VersionNumber = 7, CreatedDate = new DateTime(2020, 1, 1) };

        SaveDto().Adapt(existing);

        Assert.Equal(id, existing.InspectorId);
        Assert.Equal(7, existing.VersionNumber);
        Assert.Equal(new DateTime(2020, 1, 1), existing.CreatedDate);
        Assert.Equal("Ada", existing.FirstName);
    }

    // ---------- InspectorSkillSaveDto -> InspectorSkill ----------
    [Fact]
    public void SkillSaveDto_creates_a_link_row_with_a_new_id()
    {
        var skillId = Guid.NewGuid();
        var dto = new InspectorSkillSaveDto { SkillId = skillId, Level = InspectorSkillLevel.Internship };

        var first = dto.Adapt<InspectorSkill>();
        var second = dto.Adapt<InspectorSkill>();

        Assert.Equal(skillId, first.SkillId);
        Assert.Equal(InspectorSkillLevel.Internship, first.Level);
        Assert.NotEqual(Guid.Empty, first.InspectorSkillId);
        Assert.NotEqual(first.InspectorSkillId, second.InspectorSkillId);
    }

    // ---------- Inspector -> InspectorDto ----------
    [Fact]
    public void Inspector_dto_lists_skills_with_code_and_name_ordered_by_code()
    {
        var inspector = new Inspector
        {
            InspectorId = Guid.NewGuid(),
            FirstName = "Ada",
            Skills = new List<InspectorSkill>
            {
                new() { SkillId = Guid.NewGuid(), Level = InspectorSkillLevel.Professional,
                        Skill = new InspectionSkill { Code = "UT", Name = "Ultrasonic Testing" } },
                new() { SkillId = Guid.NewGuid(), Level = InspectorSkillLevel.Internship,
                        Skill = new InspectionSkill { Code = "MT", Name = "Magnetic Particle Testing" } }
            }
        };

        var dto = inspector.Adapt<InspectorDto>();

        Assert.Equal(new[] { "MT", "UT" }, dto.Skills.Select(s => s.Code));
        Assert.Equal("Magnetic Particle Testing", dto.Skills[0].Name);
        Assert.Equal(InspectorSkillLevel.Internship, dto.Skills[0].Level);
    }

    // ---------- documents ----------
    [Fact]
    public void DocumentType_dto_parses_the_allowed_extensions()
    {
        var dto = TestData.DocType(extensions: "*.PDF; docx").Adapt<DocumentTypeDto>();

        Assert.Equal(new[] { ".pdf", ".docx" }, dto.AllowedExtensions);
    }

    [Fact]
    public void DocumentView_maps_document_fields_type_name_and_file_info()
    {
        var document = new Document
        {
            DocumentId = Guid.NewGuid(),
            DocumentTypeId = Guid.NewGuid(),
            Name = "Contract",
            Number = "42",
            Status = DocumentStatuses.Active,
            Version = "2",
            IssueDate = new DateTime(2026, 1, 1),
            ExpireDate = new DateTime(2027, 1, 1)
        };

        var dto = new DocumentView(document, "Resume", new AttachmentInfo("cv.pdf", "application/pdf", 99))
            .Adapt<DocumentDto>();

        Assert.Equal(document.DocumentId, dto.DocumentId);
        Assert.Equal(document.DocumentTypeId, dto.DocumentTypeId);
        Assert.Equal("Contract", dto.Name);
        Assert.Equal("42", dto.Number);
        Assert.Equal(DocumentStatuses.Active, dto.Status);
        Assert.Equal("2", dto.Version);
        Assert.Equal(new DateTime(2026, 1, 1), dto.IssueDate);
        Assert.Equal(new DateTime(2027, 1, 1), dto.ExpireDate);
        Assert.Equal("Resume", dto.DocumentTypeName);
        Assert.Equal("cv.pdf", dto.FileName);
        Assert.Equal(99, dto.SizeBytes);
    }

    [Fact]
    public void DocumentView_without_file_info_reports_a_missing_file()
    {
        var document = new Document { DocumentId = Guid.NewGuid(), Version = "1" };

        var dto = new DocumentView(document, "Resume", null).Adapt<DocumentDto>();

        Assert.Equal("(file missing)", dto.FileName);
        Assert.Equal(0, dto.SizeBytes);
        Assert.Equal("Resume", dto.DocumentTypeName);
    }
}