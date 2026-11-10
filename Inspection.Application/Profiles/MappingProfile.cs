//using AutoMapper;
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionReport;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.DTOs.Inspector;
using Inspection.Application.Features.BasicInfo.Documents;
using Inspection.Application.Features.Documents;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.EDMS;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Enums;
using Inspection.Domain.Entities.ValueObjects;
using Inspection.Domain.Entities.Vendor;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Inspection.Application.Profiles;

public class MappingProfile : IRegister
{

    private static int GetMappedUserType(int domainId)
    {
        if (domainId == 0)
            return UserType.Vendor.GetHashCode();
        else if (domainId == 1)
            return UserType.PIDEC.GetHashCode();
        else
            return 0;
    }
    private static byte[] ReadFileBytes(IFormFile file)
    {
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        return ms.ToArray();
    }
    public void Register(TypeAdapterConfig config)
    {

        TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
        TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.Compile();


        TypeAdapterConfig<ActionType, ActionTypeDto>.NewConfig()
            .Map(dest => dest.ActionName, src => src.ActionName)
            .Map(dest => dest.ActionTypeId, src => src.ActionTypeId)
            .Map(dest => dest.Code, src => src.Code);

        TypeAdapterConfig<Attachment, AttachmentDto>.NewConfig()
            .Map(dest => dest.Id, src => src.AttachmentId == null ? Guid.NewGuid() : (src.AttachmentId.Value != Guid.Empty ? src.AttachmentId.Value : Guid.NewGuid()))
            // .Map(dest => dest.Id, src => src.AttachmentId.Value) // Map AttachmentId to Id
            .Map(dest => dest.Name, src => src.Name==null ? "":src.Name)
            .Map(dest => dest.Description, src => src.Description == null ? "":src.Description)
            .Map(dest => dest.ContentType, src => src.ContentType == null ? "": src.ContentType)
            .Map(dest => dest.DownloadUrl, src => src.DownloadUrl == null ? "" : src.DownloadUrl)
            .Map(dest => dest.Version, src => src.Version == null ? 0 : src.Version)            
            .Map(dest => dest.DocNo, src => src.DocNo == null ? "" : src.DocNo)
            .Map(dest => dest.Status, src => src.Status == null ? "": src.Status)
            .Map(dest => dest.PdfSize, src => src.PDFSize == null ? "" : src.PDFSize)
            .Map(dest => dest.EdmsId, src => src.edmsId == null ? "" : src.edmsId)
            .Map(dest => dest.Base64Content, src => src.Base64Content)          
            .IgnoreNonMapped(true);

        // Optional reverse mapping: Map AttachmentDto back to Attachment
        TypeAdapterConfig<AttachmentDto, Attachment>.NewConfig()
            .Map(dest => dest.AttachmentId, src => (src.Id == Guid.Empty || src.Id == null) ? new AttachmentId(Guid.NewGuid()) : new AttachmentId(src.Id)) // Map Id to AttachmentId

             .Map(dest => dest.Name, src => src.Name==null ? "":src.Name)
             .Map(dest => dest.Version, src => src.Version == null ? "0" : src.Version)
            .Map(dest => dest.Description, src => src.Description == null ? "":src.Description)
             .Map(dest => dest.ContentType, src => src.ContentType == null ? "": src.ContentType)
            .Map(dest => dest.DownloadUrl, src => src.DownloadUrl == null ? "":src.DownloadUrl)
            .Map(dest => dest.DocNo, src => src.DocNo==null ? "":src.DocNo)
            .Map(dest => dest.Status, src => src.Status == null ? "" : src.Status)
            .Map(dest => dest.edmsId, src => src.EdmsId == null ? "" : src.EdmsId)
            .Map(dest => dest.PDFSize, src => src.PdfSize == null ? "" : src.PdfSize)
            .Map(dest => dest.Base64Content, src => src.Base64Content)
           .Map(dest => dest.Content, src => src.Content != null
                ? ReadFileBytes(src.Content)
                : Array.Empty<byte>())
            .IgnoreNonMapped(true)
             .AfterMapping((src, dest) =>
             {
                 dest.Id = dest.AttachmentId;
             });




        // DocumentTypeName comes from DocumentView by name; everything else from the document itself
        TypeAdapterConfig<DocumentView, DocumentDto>.NewConfig()
            .Map(dest => dest, src => src.Document)
            .Map(dest => dest.FileName, src => src.Info != null ? src.Info.FileName : "(file missing)")
            .Map(dest => dest.SizeBytes, src => src.Info != null ? src.Info.SizeBytes : 0);
           

        TypeAdapterConfig<DocumentType, DocumentTypeDto>.NewConfig()
            .Map(dest => dest.DocumentTypeId, src => src.DocumentTypeId)
            .Map(dest => dest.DocumentTypeCode, src => src.DocumentTypeCode)
            .Map(dest => dest.DocumentTypeName, src => src.DocumentTypeName)
            .Map(dest => dest.RequiresName, src => src.RequiresName)
            .Map(dest => dest.RequiresNumber, src => src.RequiresNumber)
            .Map(dest => dest.RequiresReview, src => src.RequiresReview)
            .Map(dest => dest.RequiresApproval, src => src.RequiresApproval)                  
            .Map(dest => dest.MaxSizeMB, src => src.MaxSizeMB)
            .Map(dest => dest.MultipleAllowed, src => src.MultipleAllowed)
             .Map(dest => dest.AllowedExtensions, src => DocumentRules.ParseExtensions(src.AllowedExtensions))
            .IgnoreNonMapped(true);


        TypeAdapterConfig<DocumentTypeDto, DocumentType>.NewConfig()
            .Map(dest => dest.DocumentTypeId, src => src.DocumentTypeId)
            .Map(dest => dest.DocumentTypeCode, src => src.DocumentTypeCode)
            .Map(dest => dest.DocumentTypeName, src => src.DocumentTypeName)
            .Map(dest => dest.RequiresName, src => src.RequiresName)
            .Map(dest => dest.RequiresNumber, src => src.RequiresNumber)
            .Map(dest => dest.RequiresReview, src => src.RequiresReview)
            .Map(dest => dest.RequiresApproval, src => src.RequiresApproval)
            .Map(dest => dest.AllowedExtensions, src => string.Join(",", src.AllowedExtensions))
            
            .Map(dest => dest.MaxSizeMB, src => src.MaxSizeMB)
            .Map(dest => dest.MultipleAllowed, src => src.MultipleAllowed)
            .IgnoreNonMapped(true);


        TypeAdapterConfig<DocumentDto, Document>.NewConfig()
            .Map(dest => dest.DocumentTypeId, src => src.DocumentTypeId)
          
            .Map(dest => dest.ApprovedBy, src => src.ApprovedBy)
            .Map(dest => dest.ApprovedDate, src => src.ApprovedDate)
            .Map(dest => dest.ExpireDate, src => src.ExpireDate)           
            .Map(dest => dest.IssueDate, src => src.IssueDate)
            .Map(dest => dest.ExpireDate, src => src.ExpireDate)
            .Map(dest => dest.DocumentId, src => src.DocumentId)
      
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Version, src => src.Version)
            .IgnoreNonMapped(true);

        TypeAdapterConfig<Document, DocumentDto>.NewConfig()
        .Map(dest => dest.DocumentTypeId, src => src.DocumentTypeId)     
        .Map(dest => dest.ApprovedBy, src => src.ApprovedBy)
        .Map(dest => dest.ApprovedDate, src => src.ApprovedDate)
        .Map(dest => dest.ExpireDate, src => src.ExpireDate)
        
        .Map(dest => dest.IssueDate, src => src.IssueDate)
        .Map(dest => dest.ExpireDate, src => src.ExpireDate)
        .Map(dest => dest.DocumentId, src => src.DocumentId)         
        .Map(dest => dest.Status, src => src.Status)
        .Map(dest => dest.Version, src => src.Version)
        .Ignore(d => d.DocumentTypeName, d => d.FileName, d => d.SizeBytes)
        .IgnoreNonMapped(true);

        TypeAdapterConfig<DocumentTypeVisibility, DocumentTypeVisibilityDto>.NewConfig()
        .Map(dest => dest.DocumentTypeId, src => src.DocumentTypeId)
        .Map(dest => dest.ContactTypeCode, src => src.ContactTypeCode)
        .IgnoreNonMapped(true);

        TypeAdapterConfig<DocumentTypeVisibilityDto, DocumentTypeVisibility>.NewConfig()
      .Map(dest => dest.DocumentTypeId, src => src.DocumentTypeId)
      .Map(dest => dest.ContactTypeCode, src => src.ContactTypeCode)
      .IgnoreNonMapped(true);

        TypeAdapterConfig<DocumentLink, DocumentLinkDto>.NewConfig()
     .Map(dest => dest.OwnerType, src => src.OwnerType)
     .Map(dest => dest.OwnerId, src => src.OwnerId)
     .Map(dest => dest.DocumentId, src => src.DocumentId)     
     .IgnoreNonMapped(true);

        TypeAdapterConfig<DocumentLinkDto, DocumentLink>.NewConfig()
    .Map(dest => dest.OwnerType, src => src.OwnerType)
    .Map(dest => dest.OwnerId, src => src.OwnerId)
    .Map(dest => dest.DocumentId, src => src.DocumentId)
    .IgnoreNonMapped(true);

        // Map Circulation to CirculationDto
        TypeAdapterConfig<Circulation, CirculationDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                //.Map(dest => dest.ActionType, src => src.ActionType == null ? new ActionTypeDto() : src.ActionType.Adapt<ActionTypeDto>()) // Map complex type
                .Map(dest => dest.ActionType, src => src.ActionType==null ? 0: src.ActionType)
                //.Map(dest => dest.Attachments, src => src.Attachments == null ? new List<AttachmentDto>() : src.Attachments.Select(x=>x.Attachment).Adapt<List<AttachmentDto>>())
                .Map(dest => dest.Attachments,
    src => (src.Attachments ?? new List<CirculationAttachments>())
            .Where(x => x.Attachment != null)
            .Select(x => x.Attachment!)
            .Adapt<List<AttachmentDto>>())
                .Map(dest => dest.PerformerContactPerson,
                src => src.PerformerContactPerson == null ? null : src.PerformerContactPerson.Adapt<ContactDto>())
                .Map(dest => dest.SequenceNo, src => src.SequenceNo)
                .Map(dest => dest.PerformerPN, src => src.PerformerPN==null ? "": src.PerformerPN)
                .Map(dest => dest.PerformerContactType, src => src.PerformerContactType)
                .Map(dest => dest.PerformedActionDate, src => src.PerformedActionDate)
                .Map(dest => dest.PerformedActionDateGregorian, src => src.PerformedActionDateGregorian)  
                .Map(dest => dest.PreviousActionType , src=>src.PreviousActionType)
                .Map(dest => dest.CreatedDate, src => src.CreatedDate)
                .Map(dest => dest.ModifiedDate, src => src.ModifiedDate)
                .Map(dest => dest.Comment, src => src.Comment)
                .Map(dest => dest.CreatorComment, src => src.CreatorComment)
                .Map(dest => dest.ParentId, src => src.ParentId)
                .Map(dest => dest.RevisionId, src => src.RevisionId)
                .IgnoreNullValues(true)
                .IgnoreNonMapped(true);

        // Map Revision to RevisionDto
        TypeAdapterConfig<Revision, RevisionDto>.NewConfig()
           .Map(dest=>dest.Status,src=>src.Status)
           //.Map(dest => dest.RequestId, src => src.RequestId!.Value)
           .Map(dest => dest.IsFinal, src => src.IsFinal)
           .Map(dest => dest.Comment, src => src.Comment)
          // .Map(dest => dest.RevisionId, src => src.RevisionId)
           .Map(dest => dest.VersionNumber, src => src.VersionNumber)
           .Map(dest => dest.RevisionNumber, src => src.RevisionNumber)
           //.Map(dest => dest.SubmittedDate, src => src.SubmittedDate)
           .Map(dest => dest.SubmittedById, src => src.SubmittedBy.ContactId!)   // flatten Contact
           .Map(dest => dest.SubmittedBy, src => src.SubmittedBy == null ? null : src.SubmittedBy.Adapt<ContactDto>()) // flatten Contact
           .Map(dest => dest.Circulations, src => src.Circulations == null ? new List<CirculationDto>() : src.Circulations.Adapt<List<CirculationDto>>())
            //.Map(dest => dest.InspectionNotifications,
            //     src => src.InspectionNotifications != null
            //            ? src.InspectionNotifications.Adapt<List<InspectionNotificationDto>>()
            //            : new List<InspectionNotificationDto>())

            //.Map(dest => dest.InspectionReports,
            //     src => src.InspectionReports != null
            //            ? src.InspectionReports.Adapt<List<InspectionReportDto>>()
            //            : new List<InspectionReportDto>());
            .IgnoreNonMapped(true);



        // ===== Inspector -> InspectorDto (grid + edit form) =====
        config.NewConfig<InspectorSkill, InspectorSkillDto>()
            .Map(d => d.Code, s => s.Skill.Code)
            .Map(d => d.Name, s => s.Skill.Name);

        config.NewConfig<Inspector, InspectorDto>()
            .Map(d => d.InspectionCompanyContactName, s => s.InspectionCompanyContact.Name)
            .AfterMapping((s, d) => d.Skills = d.Skills.OrderBy(x => x.Code).ToList());

        // ===== InspectorSaveDto -> Inspector (used for both create and update) =====
        config.NewConfig<InspectorSaveDto, Inspector>()
            .Ignore(d => d.InspectorId,
                    d => d.CreatedDate,
                    d => d.VersionNumber,
                    d => d.Skills,
                    d => d.InspectionCompanyContact,
                    d => d.InspectionNotifications)
            .Map(d => d.ExpireDate, s => s.ExpireDate.ToUniversalTime())
            .AfterMapping((s, d) =>
            {
                d.FirstName = s.FirstName?.Trim();
                d.LastName = s.LastName?.Trim();
                d.Name = string.IsNullOrWhiteSpace(s.Name)
                    ? $"{d.FirstName} {d.LastName}".Trim()
                    : s.Name.Trim();
            });

        // ===== InspectorSkillSaveDto -> InspectorSkill (new link rows) =====
        config.NewConfig<InspectorSkillSaveDto, InspectorSkill>()
            .Ignore(d => d.InspectorSkillId, d => d.InspectorId, d => d.Inspector, d => d.Skill)
            .AfterMapping((s, d) => d.InspectorSkillId = Guid.NewGuid());

        // ===== documents =====
        config.NewConfig<DocumentType, DocumentTypeDto>()
            .Map(d => d.AllowedExtensions, s => ParseExtensions(s.AllowedExtensions));

        // File name, size and type name come from other sources and are set in DocumentService
        config.NewConfig<Document, DocumentDto>()
            .Ignore(d => d.DocumentTypeName, d => d.FileName, d => d.SizeBytes);




        //   TypeAdapterConfig<Inspector, InspectorDto>.NewConfig()
        //  .Map(dest => dest.Description, src => src.Description)
        //  .Map(dest => dest.Name, src => src.Name)
        //  .Map(dest => dest.LastName, src => src.LastName)
        //  .Map(dest => dest.FirstName, src => src.FirstName)
        //  .Map(dest => dest.Email, src => src.Email)
        //  .Map(dest => dest.Phone, src => src.Phone)
        //  .Map(dest => dest.Position, src => src.Position)
        //  //.Map(dest => dest.ContactId, src => src.ContactId)
        //  .Map(dest => dest.InspectorId, src => src.InspectorId)
        //  .Map(dest => dest.CreatedDate, src => src.CreatedDate)
        //  .Map(dest => dest.ExpireDate, src => src.ExpireDate)
        //  .Map(dest => dest.InspectionCompanyContactId, src => src.InspectionCompanyContactId)
        //  .Map(dest => dest.IsActive, src => src.IsActive)
        //  .Map(dest => dest.QCApprovalState, src => src.QCApprovalState)
        //  .IgnoreNonMapped(true);

        //   TypeAdapterConfig<InspectorDto, Inspector>.NewConfig()
        //     .Map(dest => dest.Description, src => src.Description)
        //     .Map(dest => dest.Name, src => src.Name)
        //     .Map(dest => dest.LastName, src => src.LastName)
        //     .Map(dest => dest.FirstName, src => src.FirstName)
        //     .Map(dest => dest.Email, src => src.Email)
        //     .Map(dest => dest.Phone, src => src.Phone)
        //     .Map(dest => dest.Position, src => src.Position)
        //     //.Map(dest => dest.ContactId, src => src.ContactId)
        //     .Map(dest => dest.InspectorId, src => src.InspectorId)
        //     .Map(dest=>dest.CreatedDate, src=> src.CreatedDate)
        //     .Map(dest=>dest.ExpireDate, src=> src.ExpireDate)
        //     .Map(dest => dest.InspectionCompanyContactId , src => src.InspectionCompanyContactId)
        //     .Map(dest => dest.IsActive, src => src.IsActive)
        //     .Map(dest => dest.QCApprovalState, src => src.QCApprovalState)
        //     .IgnoreNonMapped(true);


        TypeAdapterConfig<InspectionSkillDto, InspectionSkill>.NewConfig()
     .Map(dest => dest.SkillId, src => src.SkillId)
     .Map(dest => dest.Code, src => src.Code)
     .Map(dest => dest.Name, src => src.Name)
     .Map(dest => dest.IsActive, src => src.IsActive);


        TypeAdapterConfig<InspectionSkill, InspectionSkillDto>.NewConfig()
           .Map(dest => dest.SkillId, src => src.SkillId)
           .Map(dest => dest.Code, src => src.Code)
           .Map(dest => dest.Name, src => src.Name)
           .Map(dest => dest.IsActive, src => src.IsActive)
           .Ignore(d => d.InspectorCount); 


        //   TypeAdapterConfig<InspectorSkill, InspectorSkillDto>.NewConfig()
        //    .Map(dest => dest.SkillId, src => src.SkillId)
        //    .Map(dest => dest.Code, src => src.Skill.Code)
        //    .Map(dest => dest.Name, src => src.Skill.Name)
        //    .Map(dest => dest.Level, src => src.Level);

        //   TypeAdapterConfig<InspectorSkillDto, InspectorSkill>.NewConfig()
        //  .Map(dest => dest.SkillId, src => src.SkillId)
        //  .Map(dest => dest.Level, src => src.Level);

        TypeAdapterConfig<InspectionNotificationInspector, InspectionNotificationInspectorDto>.NewConfig()
    .Map(dest => dest.InspectionInspectorId, src => src.InspectionInspectorId)
    .Map(dest => dest.InspectorId, src => src.InspectorId)
    .Map(dest => dest.Remarks, src => src.Remarks)
    .Map(dest => dest.ApprovedDate, src => src.ApprovedDate)
    .Map(dest => dest.ApprovalStatus, src => src.ApprovalStatus)
    .Map(dest => dest.InspectionNotificationId, src => src.InspectionNotificationId)   
    .Map(dest => dest.Inspector,
                    src => src.Inspector == null ? null : src.Inspector.Adapt<Inspector>())
    .Map(dest => dest.ApprovedBy, src => src.ApprovedBy);


        TypeAdapterConfig<InspectionReport, InspectionReportDto>.NewConfig()
          .Map(dest => dest.CurrentStatus, src => (int)src.CurrentStatus)          
          .Map(dest => dest.RequestIdOfRelatedInspectionNotification, src => src.RequestIdOfRelatedInspectionNotification)
          .Map(dest => dest.ReportType, src => (int)src.ReportType)
          .Map(dest => dest.ReportNotes, src => src.ReportNotes)
          .Map(dest => dest.IssueDate, src => src.IssueDate)
          .Map(dest => dest.PersianIssueDate, src => src.PersianIssueDate)
          .Map(dest => dest.CreatedBy, src => src.CreatedBy)
          .Map(dest => dest.InspectionReportNumber, src=> src.InspectionReportNumber)
          .Map(dest => dest.InspectionReportId, src => src.InspectionReportId.Value)
          .Map(dest => dest.RelatedInspectionNotificationId, src => src.RelatedInspectionNotificationId)
          .Map(dest => dest.RequestIdOfRelatedInspectionNotification, src => src.RequestIdOfRelatedInspectionNotification)
          .Map(dest => dest.RevisionId, src => src.RevisionId)
          .Map(dest => dest.Inspectors, src => src.Inspectors.Adapt<List<InspectorDto>>())
         // .Map(dest => dest.InspectionPersons, src => src.InspectionPersons == null ? new List<PersonDto>() : src.InspectionPersons.Select(x => x.Person).Adapt<List<PersonDto>>())
          .IgnoreNonMapped(true);

        TypeAdapterConfig<InspectionReportDto, InspectionReport>.NewConfig()
        .Map(dest => dest.CurrentStatus, src => (int)src.CurrentStatus)
        .Map(dest => dest.InspectionReportId, src => (src.InspectionReportId == null ? Guid.NewGuid() : src.InspectionReportId))
        .Map(dest => dest.RequestIdOfRelatedInspectionNotification, src => src.RequestIdOfRelatedInspectionNotification)
        .Map(dest => dest.ReportType, src => (int)src.ReportType)
        .Map(dest => dest.ReportNotes, src => src.ReportNotes)
        .Map(dest => dest.IssueDate, src => (src.IssueDate == null ? DateTime.Now : src.IssueDate))
        .Map(dest => dest.PersianIssueDate, src => src.PersianIssueDate)
        .Map(dest => dest.CreatedBy, src => src.CreatedBy)
        .Map(dest => dest.InspectionReportNumber, src => src.InspectionReportNumber)
        .Map(dest => dest.InspectionReportId, src => src.InspectionReportId.Value)
        .Map(dest => dest.RelatedInspectionNotificationId, src => src.RelatedInspectionNotificationId)
        .Map(dest => dest.RequestIdOfRelatedInspectionNotification, src => src.RequestIdOfRelatedInspectionNotification)
        .Map(dest => dest.RevisionId, src => src.RevisionId)
        .Map(dest => dest.Inspectors, src => src.Inspectors.Adapt<List<InspectorDto>>())       
        .IgnoreNonMapped(true);
        
        // Map Circulation to CirculationDto
        TypeAdapterConfig<CirculationDto, Circulation>.NewConfig()
                    .Map(dest => dest.Id, src => src.Id)
                    //.Map(dest => dest.ActionType, src => src.ActionType == null ? new ActionTypeDto() : src.ActionType.Adapt<ActionTypeDto>()) // Map complex type
                    .Map(dest => dest.ActionType, src => src.ActionType)
                   // .Map(dest => dest.PerformerContactPerson, src => src.PerformerContactPerson.Adapt<Contact>()) // Map another complex type
                   .Map(dest => dest.PerformerContactPerson,
                    src => src.PerformerContactPerson == null ? null : src.PerformerContactPerson.Adapt<Contact>())
                    .Map(dest => dest.SequenceNo, src => src.SequenceNo)
                    .Map(dest => dest.PerformerPN, src => src.PerformerPN)
                    .Map(dest => dest.PerformerContactType, src => src.PerformerContactType)
                    .Map(dest => dest.PerformedActionDate, src => src.PerformedActionDate)
                    .Map(dest => dest.PerformedActionDateGregorian, src => src.PerformedActionDateGregorian)
                    //  .Map(dest => dest.Request, src => src.Request.Adapt<Request>())
                    .Map(dest => dest.CreatedDate, src => src.CreatedDate)
                    .Map(dest => dest.ModifiedDate, src => src.ModifiedDate)
                    .Map(dest => dest.Comment, src => src.Comment)
                    .Map(dest=>dest.PreviousActionType , src=>src.PreviousActionType)
                    .Map(dest => dest.CreatorComment, src => src.CreatorComment)
                    .Map(dest => dest.ParentId, src => src.ParentId)
                    .Map(dest => dest.RevisionId, src => src.RevisionId)
                    .IgnoreNullValues(true)
                    .IgnoreNonMapped(true);


        // Map BasicPIDECLeadsInfo to BasicPIDECLeadsInfoDto
        TypeAdapterConfig<BasicPIDECLeadsInfo, BasicPIDECLeadsInfoDto>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                  .Map(dest => dest.UserName, src => src.UserName)
                  .Map(dest => dest.DomainId, src => src.DomainId)
                  .Map(dest => dest.Projects, src => src.Projects.Adapt<List<ProjectDto>>())
                  .Map(dest => dest.Orders, src => src.Orders.Adapt<List<OrderDto>>())
                  .Map(dest => dest.EDMSOrders, src => src.EDMSOrders.Adapt<List<OrderDto>>())
                  .Map(dest => dest.OrderContacts, src => src.OrderContacts.Adapt<List<OrderContact>>())
                  .Map(dest => dest.InspectionTypes, src => src.InspectionTypes.Adapt<List<InspectionTypeDto>>())
                  .Map(dest => dest.InspectionSubjects, src => src.InspectionSubjects.Adapt<List<InspectionSubjectDto>>())
                  .Map(dest => dest.OrderInspectionSubjects, src => src.OrderInspectionSubjects.Adapt<List<OrderInspectionSubjectDto>>())
                  .Map(dest => dest.DocumentTypes, src => src.OrderInspectionSubjects.Adapt<List<OrderInspectionSubjectDto>>())
                  .Map(dest => dest.DocumentTypeVisibilities, src => src.DocumentTypesVisibilities.Adapt<List<DocumentTypeVisibilityDto>>())
                  .Map(dest => dest.OrderInspectionSubjects, src => src.OrderInspectionSubjects.Adapt<List<OrderInspectionSubjectDto>>())
                  .Map(dest => dest.ReceivedInspectionNotifications, src => src.ReceivedInspectionNotifications.Adapt<List<InspectionNotificationDto>>())
                .IgnoreNonMapped(true);

        // Map Request → RequestDto (null-safe)
        TypeAdapterConfig<Request, RequestDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.CreatorContactPerson,
                 src => src.CreatorContactPerson == null
                     ? null
                     : src.CreatorContactPerson.Adapt<ContactDto>())
            .Map(dest => dest.Revisions, src => src.Revisions != null ? src.Revisions.Adapt<List<RevisionDto>>() : new List<RevisionDto>())
            .Map(dest => dest.CreatedBy, src => src.CreatedBy)
            .Map(dest => dest.CreatedDate, src => src.CreatedDate)
            .Map(dest => dest.ModifiedDate, src => src.ModifiedDate)
            .Map(dest => dest.ModifiedBy, src => src.ModifiedBy)
            .IgnoreNonMapped(true);

        // Map RequestDto → Request (null-safe)
        TypeAdapterConfig<RequestDto, Request>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.CreatorContactPerson,
                 src => src.CreatorContactPerson == null
                     ? null
                     : src.CreatorContactPerson.Adapt<Contact>())

            .Map(dest => dest.CreatedBy, src => src.CreatedBy)
            .Map(dest => dest.CreatedDate, src => src.CreatedDate)
            .Map(dest => dest.ModifiedDate, src => src.ModifiedDate)
            .Map(dest => dest.ModifiedBy, src => src.ModifiedBy)
            .IgnoreNonMapped(true);

        TypeAdapterConfig<ProjectContact, ContactDto>.NewConfig()
    .Map(dest => dest, src => src.Contact != null ? src.Contact : new Contact
    {
        // fallback Contact construction from ContactId/Id fields if you have a Contact ctor or assign properties
        //  Id = src.ContactId != null && src.ContactId.Value != Guid.Empty ? src.ContactId.Value : src.Id,
        Name = string.Empty
    });
        //.Map(dest => dest.Id,
        //     src => src.Contact != null && src.Contact.Id != Guid.Empty
        //         ? (Guid?)src.Contact.Id
        //         : (src.ContactId != null && src.ContactId.Value != Guid.Empty ? (Guid?)src.ContactId.Value : (Guid?)src.Id));

        TypeAdapterConfig<Project, ProjectDto>.NewConfig()
         // .Map(dest => dest.Id, src => src.Id)
         //.Map(dest => dest.Id, src => src.ProjectId!.Value)
         .Map(dest => dest.Title, src => src.Title)
         .Map(dest => dest.Description, src => src.Description)
         .Map(dest => dest.Number, src => src.Number)
         .Map(dest => dest.FSId, src => src.FSId)
         .Map(dest => dest.Id, src => src.ProjectId)
          .Map(dest => dest.Contacts, src =>
        src.Contacts == null
            ? null
            : src.Contacts
                .Where(c => c != null && c.Contact != null)
                .Select(c => c.Contact.Adapt<ContactDto>())
                .ToList())
           .Map(dest => dest.PidecQCContacts, src =>
        src.PidecQCContacts == null
            ? null
            : src.PidecQCContacts
                .Where(pc => pc != null && pc.Contact != null)
                .Select(pc => pc.Contact.Adapt<ContactDto>())
                .ToList())
         .Map(dest => dest.MCContacts, src =>
        src.MCContacts == null
            ? null
            : src.MCContacts
                .Where(pc => pc != null && pc.Contact != null)
                .Select(pc => pc.Contact.Adapt<ContactDto>())
                .ToList())
          .Map(dest => dest.TPInspectors, src =>
        src.TPInspectors == null
            ? null
            : src.TPInspectors
                .Where(pc => pc != null && pc.Contact != null)
                .Select(pc => pc.Contact.Adapt<ContactDto>())
                .ToList())
          .Map(dest => dest.PidecInspectors, src =>
        src.PidecInspectors == null
            ? null
            : src.PidecInspectors
                .Where(pc => pc != null && pc.Contact != null)
                .Select(pc => pc.Contact.Adapt<ContactDto>())
                .ToList())
          .Map(dest => dest.OwnerContacts, src =>
        src.OwnerContacts == null
            ? null
            : src.OwnerContacts
                .Where(pc => pc != null && pc.Contact != null)
                .Select(pc => pc.Contact.Adapt<ContactDto>())
                .ToList())
          .AfterMapping((src, dest) =>
          {
              dest.Id = src.Id == null ? src.ProjectId?.Value : src.Id;
          })
         .IgnoreNullValues(true)
         .IgnoreNonMapped(true);

        config.NewConfig<ProjectDto, Project>()
   .Map(dest => dest.Id, src => src.Id)
   .Map(dest => dest.Title, src => src.Title)
   .Map(dest => dest.Description, src => src.Description)
   .Map(dest => dest.Number, src => src.Number)
   .Map(dest => dest.FSId, src => src.FSId)
   .Map(dest => dest.ProjectId, src => new ProjectId(src.Id.Value))
   .Map(dest => dest.PidecQCContacts, src => src.PidecQCContacts!.Select(x => new ProjectContact
   {
       Contact = new Contact
       {
           Email = x.Email,
           DomainId = x.DomainId,
           Fax = x.Fax,
           HomePage = x.HomePage,
           //Id = x.Id,
           Address = x.Address.Adapt<Address>(),
           Note = x.Note,
           Cc = x.Cc,
           ContactId = new ContactId(x.Id.Value),
           ContactType = x.ContactType,
           IsVisible = x.IsVisible,
           Mobile = x.Mobile,
           Name = x.Name,
           Phone = x.Phone,
           UserId = x.UserId,
           UserName = x.UserName
       },
       ContactId = x.Id.Value,
       Id = Guid.Empty,
       Project = null,
       ProjectId = src.Id
   }).ToList())
   .Map(dest => dest.MCContacts, src => src.MCContacts!.Select(x => new ProjectContact
   {
       Contact = new Contact
       {
           Email = x.Email,
           DomainId = x.DomainId,
           Fax = x.Fax,
           HomePage = x.HomePage,
           //Id = x.Id,
           Address = x.Address.Adapt<Address>(),
           Note = x.Note,
           Cc = x.Cc,
           ContactId = new ContactId(x.Id.Value),
           ContactType = x.ContactType,
           IsVisible = x.IsVisible,
           Mobile = x.Mobile,
           Name = x.Name,
           Phone = x.Phone,
           UserId = x.UserId,
           UserName = x.UserName
       },
       ContactId = x.Id.Value,
       Id = Guid.Empty,
       Project = null,
       ProjectId = src.Id
   }).ToList())
   .Map(dest => dest.TPInspectors, src => src.TPInspectors!.Select(x => new ProjectContact
   {
       Contact = x.Adapt<Contact>(),
       ContactId = x.Id.Value,
       Id = Guid.Empty,
       Project = null,
       ProjectId = src.Id
   }).ToList())
   .Map(dest => dest.PidecInspectors, src => src.PidecInspectors!.Select(x => new ProjectContact
   {
       Contact = x.Adapt<Contact>(),
       ContactId = x.Id.Value,
       Id = Guid.Empty,
       Project = null,
       ProjectId = src.Id
   }).ToList())
   .Map(dest => dest.OwnerContacts, src => src.OwnerContacts!.Select(x => new ProjectContact
   {
       Contact = x.Adapt<Contact>(),
       ContactId = x.Id.Value,
       Id = Guid.Empty,
       Project = null,
       ProjectId = src.Id
   }).ToList())
   .IgnoreNullValues(true)
   .IgnoreNonMapped(true)
    .AfterMapping((src, dest) =>
    {
        dest.ProjectId = new ProjectId(src.Id.Value);
    });


        TypeAdapterConfig<Address, AddressDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id.ToString().Trim() !="" ? src.Id: new Guid()) // Map Id
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.State, src => src.State)
            .Map(dest => dest.City, src => src.City)
            .Map(dest => dest.StreetLine1, src => src.StreetLine1)
            .Map(dest => dest.StreetLine2, src => src.StreetLine2)
            .Map(dest => dest.ZipCode, src => src.ZipCode)
            .IgnoreNonMapped(true);
        // .AfterMapping((src, dest) => dest.Id = Guid.NewGuid());

        TypeAdapterConfig<OrderInspectionSubjectDto, InspectionSubject>.NewConfig()
       .Map(dest => dest.Name, src => src.InspectionSubject.Name)
       .Map(dest => dest.InspectionSubjectId, src => src.InspectionSubject.Id)
       .IgnoreNonMapped(true);

        TypeAdapterConfig<OrderContactDto, Contact>.NewConfig()
            .Map(dest => dest.ContactId, src => src.Contact.Id)
            .Map(dest => dest.Note, src => src.Contact.Note)
            .Map(dest => dest.Fax, src => src.Contact.Fax)
            .Map(dest => dest.Email, src => src.Contact.Email)
            .Map(dest => dest.Name, src => src.Contact.Name)
            .Map(dest => dest.UserName, src => src.Contact.UserName)
            .Map(dest => dest.Cc, src => src.Contact.Cc)
            .Map(dest => dest.HomePage, src => src.Contact.HomePage)
            .Map(dest => dest.ContactType, src => src.Contact.ContactType)
            .Map(dest => dest.DomainId, src => src.Contact.DomainId)
            .Map(dest => dest.IsVisible, src => src.Contact.IsVisible)
            .Map(dest => dest.Mobile, src => src.Contact.Mobile)
            .Map(dest => dest.UserId, src => src.Contact.UserId)
            .IgnoreNonMapped(true);




        // The dropdown used by the inspector form: your column is ContactType, the DTO calls it ContactTypeCode
        TypeAdapterConfig<Contact, ContactOptionDto>.NewConfig()
            .Map(d => d.ContactTypeCode, s => s.ContactType.Value)
            .Map(d => d.Name, s => s.Name)
            .Map(d => d.ContactId, s => s.ContactId);

        TypeAdapterConfig<RevisionDto, InspectionNotificationRevision>.NewConfig()
         .Map(dest => dest.RevisionId, src => src.RevisionId)
         .Map(dest => dest.Revision, src =>
         new Revision
         {
             RevisionId = src.RevisionId == Guid.Empty ? Guid.NewGuid() : src.RevisionId.Value,
             RequestId = src.RequestId,
             Comment = src.Comment,
             IsFinal = src.IsFinal,
             Status = src.Status,
             RevisionNumber = src.RevisionNumber,
             VersionNumber = src.VersionNumber,
             SubmittedDate = src.SubmittedDate
             
         })        
         .IgnoreNonMapped(true);


        TypeAdapterConfig<InspectionNotificationRevision, RevisionDto>.NewConfig()
         .Map(dest => dest.RevisionId, src => src.RevisionId != Guid.Empty ? src.RevisionId : src.Revision.RevisionId)
         .Map(dest => dest.RequestId , src => src.Revision.RequestId)
         .Map(dest => dest.Comment, src => src.Revision.Comment)
         .Map(dest => dest.IsFinal, src => src.Revision.IsFinal)
         .Map(dest => dest.Status, src => src.Revision.Status)
         .Map(dest => dest.RevisionNumber, src => src.Revision.RevisionNumber)
         .Map(dest => dest.VersionNumber, src => src.Revision.VersionNumber)
         .Map(dest => dest.SubmittedDate, src => src.Revision.SubmittedDate)
         .Map(dest => dest.CreatedByCirculationId, src => src.Revision.CreatedByCirculationId)
          .AfterMapping((src, dest) => {
              if (src.Revision.Circulations == null)
                  dest.Circulations = new List<CirculationDto>();
              else
              {
                  
                  dest.Circulations = src.Revision.Circulations.Adapt<List<CirculationDto>>();
                  foreach (var circ in dest.Circulations)
                  {
                      circ.RevisionId = src.RevisionId != Guid.Empty ? src.RevisionId : src.Revision.RevisionId;
                  }
              }
          })
         .Map(dest => dest.Circulations, src => src.Revision.Circulations == null ? new List<CirculationDto>() : src.Revision.Circulations.Adapt<List<CirculationDto>>())
         .IgnoreNonMapped(true);


        TypeAdapterConfig<InspectionNotificationType, InspectionNotificationTypeDto>.NewConfig()
            .Map(dest => dest.InspectionNotificationId, src => src.InspectionNotificationId.Value.ToString())
            .Map(dest => dest.Name, src => src.InspectionType.Name)
            .Map(dest => dest.Id, src => src.InspectionType.InspectionTypeId.Value.ToString());


        TypeAdapterConfig<InspectionTypeDto, InspectionNotificationType>.NewConfig()
            .Map(dest => dest.InspectionTypeId, src => src.Id)
             //.Map(dest => dest.InspectionType, src => src)
             .Map(dest => dest.InspectionType, src =>
         new InspectionType
         {
             InspectionTypeId = src.Id == Guid.Empty ? InspectionTypeId.CreateUnique() : src.Id,
             Name = src.Name
         })
        .AfterMapping((src, dest) => {
            dest.InspectionTypeId = InspectionTypeId.Create(dest.InspectionType.InspectionTypeId.Value);
        })
            .IgnoreNonMapped(true);


        TypeAdapterConfig<InspectionSubjectDto, InspectionNotificationSubject>.NewConfig()
         .Map(dest => dest.InspectionSubjectId, src => src.Id)
        // .Map(dest => dest.InspectionSubject, src => src)
         .Map(dest=>dest.ITP_Row_No ,src=>src.ItpRowNo)       
          .Map(dest => dest.InspectionSubject, src =>
         new InspectionSubject
         {
             InspectionSubjectId = src.Id == Guid.Empty ? InspectionSubjectId.CreateUnique() : src.Id,
             Name = src.Name
         })
        .AfterMapping((src, dest) => {
            dest.InspectionSubjectId = InspectionSubjectId.Create(dest.InspectionSubject.InspectionSubjectId.Value);
        })
         .IgnoreNonMapped(true);


        TypeAdapterConfig<AddressDto, InspectionNotificationLocation>.NewConfig()
       .Map(dest => dest.AddressId, src => src.Id)              
        .Map(dest => dest.Location, src =>
       new Address
       {
           Id = src.Id == Guid.Empty ? Guid.Empty : src.Id,
           Country = src.Country,
           City = src.City,
           State = src.State,
           StreetLine1 = src.StreetLine1,
           StreetLine2 = src.StreetLine2,
           ZipCode = src.ZipCode
       })
      .AfterMapping((src, dest) => {
          dest.AddressId = src.Id;
      })
       .IgnoreNonMapped(true);

        TypeAdapterConfig<InspectionNotificationSubject, InspectionNotificationSubjectDto>.NewConfig()
            .Map(dest => dest.InspectionNotificationId, src => src.InspectionNotificationId.Value.ToString())
            .Map(dest => dest.Name, src => src.InspectionSubject.Name)
            .Map(dest =>dest.ITP_Row_No,src =>src.ITP_Row_No)
            .Map(dest => dest.Id, src => src.InspectionSubject.InspectionSubjectId.Value.ToString());


        TypeAdapterConfig<InspectionNotificationContact, InspectionNotificationContactDto>.NewConfig()
           .Map(dest => dest.InspectionNotificationId, src => src.InspectionNotificationId.Value.ToString())
           .Map(dest => dest.Name, src => src.Contact.Name)
           .Map(dest => dest.ContactType, src => src.Contact.ContactType)
           .Map(dest => dest.Note, src => src.Contact.Note)
           .Map(dest => dest.Cc, src => src.Contact.Cc)
           .Map(dest => dest.DomainId, src => src.Contact.DomainId)
           .Map(dest => dest.UserName, src => src.Contact.UserName)
           .Map(dest => dest.Phone, src => src.Contact.Phone)
           .Map(dest => dest.Mobile, src => src.Contact.Mobile)
           .Map(dest => dest.Email, src => src.Contact.Email)
           .Map(dest => dest.Fax, src => src.Contact.Fax)
           .Map(dest => dest.HomePage, src => src.Contact.HomePage)
           //.Map(dest => dest.Address, src => src.Contact.Address)
           .Map(dest => dest.IsVisible, src => src.Contact.IsVisible)
            .Map(dest => dest.UserId, src => src.Contact.UserId)
           .Map(dest => dest.Id, src => src.Id)
           .IgnoreNonMapped(true);

        TypeAdapterConfig<InspectionNotificationFlow, InspectionNotificationFlowDto>.NewConfig()
             .Map(dest => dest.InspectionNotifications, src => src.InspectionNotifications!.Select(x => x.Adapt<InspectionNotificationDto>()).ToList())
            .Map(dest => dest.Types, src => src.Types!.Select(x => x.Adapt<InspectionNotificationTypeDto>()).ToList())
            .Map(dest => dest.Subjects, src => src.Subjects!.Select(x => x.Adapt<InspectionNotificationSubjectDto>()).ToList())
             .Map(dest => dest.Contacts, src => src.Contacts!.Select(x => x.Adapt<InspectionNotificationContactDto>()).ToList())
              .Map(dest => dest.Circulations, src => src.Circulations!.Select(x => x.Adapt<CirculationDto>()).ToList())
               .IgnoreNonMapped(true);



        TypeAdapterConfig<InspectionNotificationContact, ContactDto>.NewConfig()
          .Map(dest => dest.Id, src => src.Contact.ContactId)
          .Map(dest => dest.Note, src => src.Contact.Note)
          .Map(dest => dest.Fax, src => src.Contact.Fax)
          .Map(dest => dest.Email, src => src.Contact.Email)
          .Map(dest => dest.Name, src => src.Contact.Name)
          .Map(dest => dest.UserName, src => src.Contact.UserName != "" ? src.Contact.UserName : src.Contact.Name)
          .Map(dest => dest.Cc, src => src.Contact.Cc)
          .Map(dest => dest.HomePage, src => src.Contact.HomePage)
          .Map(dest => dest.ContactType, src => src.Contact.ContactType)
          .Map(dest => dest.DomainId, src => src.Contact.DomainId)
          .Map(dest => dest.IsVisible, src => src.Contact.IsVisible)
          .Map(dest => dest.Mobile, src => src.Contact.Mobile)
          .Map(dest => dest.UserId, src => src.Contact.UserId)
          .IgnoreNonMapped(true);

        TypeAdapterConfig<InspectionNotificationSubject, InspectionSubjectDto>.NewConfig()
             .Map(dest => dest.Id, src => src.InspectionSubject!=null ? (src.InspectionSubject.InspectionSubjectId != null ? src.InspectionSubject.InspectionSubjectId.Value : src.InspectionSubjectId!.Value) : src.InspectionSubjectId!.Value)
             //.Map(dest => dest.Id, src => src.InspectionSubjectId!.Value)
            .Map(dest => dest.Name, src => src.InspectionSubject!.Name)
            .Map(dest=>dest.ItpRowNo,src => src.ITP_Row_No)
            //.Map(dest=>dest.Id,src=>src.InspectionSubject!.InspectionSubjectId!.Value)
            .IgnoreNonMapped(true);


        TypeAdapterConfig<InspectionNotificationType, InspectionTypeDto>.NewConfig()
            .Map(dest => dest.Id, src => src.InspectionType.InspectionTypeId!.Value)
            .Map(dest => dest.Name, src => src.InspectionType.Name)
            .IgnoreNonMapped(true);

        TypeAdapterConfig<InspectionNotificationLocation, AddressDto>.NewConfig()
         .Map(dest => dest.Id, src => src.Location.Id != Guid.Empty ? src.Location.Id : Guid.Empty)
         .Map(dest => dest.StreetLine2, src => src.Location.StreetLine2)
         .Map(dest => dest.StreetLine1, src => src.Location.StreetLine1)
         .Map(dest => dest.ZipCode, src => src.Location.ZipCode)
         .Map(dest => dest.Country, src => src.Location.Country)
         .Map(dest => dest.State, src => src.Location.State)
         .Map(dest => dest.City, src => src.Location.City)

         .IgnoreNonMapped(true);

        TypeAdapterConfig<CirculationAttachments, AttachmentDto>.NewConfig()
       .Map(dest => dest.Id, src => src.AttachmentId == null ? Guid.NewGuid() : (src.AttachmentId.Value != Guid.Empty ? src.AttachmentId.Value : Guid.NewGuid()))   
       .Map(dest => dest.Name, src => src.Attachment ==null ? "" : (src.Attachment!.Name == null ? "": src.Attachment!.Name))
       .Map(dest => dest.Version, src => src.Attachment == null ? 0 : (src.Attachment!.Version == null ? 0 : src.Attachment!.Version))
       .Map(dest => dest.DownloadUrl, src =>  src.Attachment == null ? "" : (src.Attachment!.DownloadUrl == null ? "" : src.Attachment!.DownloadUrl))
       .Map(dest => dest.ContentType, src => src.Attachment == null ? "" : (src.Attachment!.ContentType == null ? "" : src.Attachment!.ContentType))
       .Map(dest => dest.Description, src => src.Attachment!.Description == null ? "" : src.Attachment!.Description)
       .Map(dest => dest.DocNo, src => src.Attachment == null ? "" : (src.Attachment!.DocNo == null ? "" : src.Attachment!.DocNo))
       .Map(dest => dest.Status, src => src.Attachment == null ? "" : (src.Attachment!.Status == null ? "" : src.Attachment!.Status))
       
       .IgnoreNonMapped(true);

        TypeAdapterConfig<ContactDto, Contact>.NewConfig()
    .Map(dest => dest.ContactId, src => src.Id ?? Guid.Empty)
    .Map(dest => dest.Note, src => src.Note)
    .Map(dest => dest.Fax, src => src.Fax)
    .Map(dest => dest.Email, src => src.Email ?? "")
    .Map(dest => dest.Name, src => src.Name ?? "")
    .Map(dest => dest.UserName,
         src => !string.IsNullOrWhiteSpace(src.UserName)
             ? src.UserName
             : src.Name)
    .Map(dest => dest.Cc, src => src.Cc)
    .Map(dest => dest.HomePage, src => src.HomePage ?? "")
    .Map(dest => dest.ContactType, src => src.ContactType)
    .Map(dest => dest.DomainId, src => src.DomainId)
    .Map(dest => dest.IsVisible, src => src.IsVisible ?? false)
    .Map(dest => dest.Mobile, src => src.Mobile ?? "")
    .Map(dest => dest.UserId, src => src.UserId ?? null)
    .Map(dest => dest.Password, src => src.Password)
    .Map(dest => dest.Phone, src => src.Phone ?? "")
    .IgnoreNonMapped(true);


        TypeAdapterConfig<ContactDto, InspectionNotificationContact>.NewConfig()

         
         // .Map(dest => dest.Contact, src => src.Adapt<Contact>())
         //.IgnoreNonMapped(true);
         //.Map(dest => dest.Contact, src =>
         //)
        .AfterMapping((src, dest) => {

            dest.Contact = new Contact
            {
                Cc = src.Cc,
                UserName = src.UserName != "" ? src.UserName : src.Name,
                Note = src.Note,
                ContactType = src.ContactType,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now,
                DomainId = src.DomainId,
                Email = src.Email,
                Fax = src.Fax,
                HomePage = src.HomePage,
                Mobile = src.Mobile,
                Name = src.Name,
                ContactId = src.Id == Guid.Empty ? Guid.NewGuid() : src.Id.Value,
                //  Id = src.Id == Guid.Empty ? ContactId.CreateUnique() : src.Id,
                Phone = src.Phone,
                UserId = src.UserId
            };
            dest.ContactId = dest.Contact.ContactId;//ContactId.Create(dest.Contact.ContactId);
            
            dest.Id = Guid.NewGuid();//dest.Contact.ContactId;
        })
        .IgnoreNonMapped(true);


        TypeAdapterConfig<Contact, ContactDto>.NewConfig()
    .Map(dest => dest.Id,
         src => (src.ContactId != null && src.ContactId != Guid.Empty)
             ? src.ContactId : Guid.NewGuid())
    .Map(dest => dest.Name, src => src.Name ?? string.Empty)
    .Map(dest => dest.UserName,
         src => !string.IsNullOrWhiteSpace(src.UserName)
             ? src.UserName
             : (src.Name ?? string.Empty))
    .Map(dest => dest.DomainId, src => src.DomainId ?? 0)
    //.Map(dest => dest.Address,
    //     src => src.Address == null ? null : src.Address.Adapt<AddressDto>())
    .Map(dest => dest.Phone, src => src.Phone ?? string.Empty)
    .Map(dest => dest.Fax, src => src.Fax ?? string.Empty)
    .Map(dest => dest.Email, src => src.Email ?? string.Empty)
    .Map(dest => dest.HomePage, src => src.HomePage ?? string.Empty)
    .Map(dest => dest.Note, src => src.Note ?? string.Empty)
    .Map(dest => dest.Cc, src => src.Cc ?? string.Empty)
    .Map(dest => dest.Mobile, src => src.Mobile ?? string.Empty)
    .Map(dest => dest.ContactType, src => src.ContactType ?? 0)
    .Map(dest => dest.UserId, src => src.UserId ?? string.Empty)
    .Map(d => d.ContactTypeName,
                 s => s.ContactType == null ? null : ((ContactType)s.ContactType.Value).ToString())
    .IgnoreNonMapped(true);


        TypeAdapterConfig<InspectionSubject, InspectionSubjectDto>.NewConfig()
          .Map(dest => dest.Id, src => src.InspectionSubjectId.Value) // Map ContactId.Value to Id
          .Map(dest => dest.Name, src => src.Name)          
          .IgnoreNonMapped(true); // Ignore any unmapped properties


        TypeAdapterConfig<EDMSOrder, OrderDto>.NewConfig()

           .Map(dest => dest.Name, src => src.No)
           //.Map(dest=>dest.Title,src=>src.No)
           .Map(dest => dest.EdmsId, src => src.Dot_Id.ToString())
           .Map(dest => dest.Title, src => src.ContactPersonUserName)
           .Map(dest => dest.IsAccessibleByEmployer, src => src.IsAccessibleByEmployer)
           .Map(dest => dest.HasReadPermission, src => src.HasReadPermission)
           .Map(dest => dest.VendorCompanyName, src => src.VendorCompanyName)

           .IgnoreNonMapped(true)
            .AfterMapping((src, dest) => dest.Id = Guid.NewGuid())
             .AfterMapping((src, dest) =>
             {
                 if (!string.IsNullOrEmpty(src.ContactPersonUserName))
                 {
                     var newContact = new ContactDto
                     {
                         Name = src.ContactPersonUserName,
                         UserName = src.ContactPersonUserName,
                         ContactType = (int)ContactType.Vendor,
                         IsVisible = true,
                         DomainId = 1,
                         HomePage = src.Dot_Id.ToString(),
                         Email = src.ContactPersonUserName

                     };
                     dest.Contacts ??= new List<ContactDto>(); // Ensure the list is initialized
                     dest.Contacts.Add(newContact);
                 }
             });

        TypeAdapterConfig<Order, OrderDto>.NewConfig()
           .Map(dest => dest.VendorDocuments, src => src.VendorDocuments.Adapt<List<VendorDocumentDto>>())
           .Map(dest => dest.Contacts, src => src.Contacts == null ? new List<ContactDto>() : src.Contacts.Select(x => x.Contact).Adapt<List<ContactDto>>())
           .Map(dest => dest.Version, src => src.Version)
           .Map(dest => dest.InspectionSubjects, src => src.Subjects == null ? new List<InspectionSubjectDto>() : src.Subjects.Select(x => x.InspectionSubject).Adapt<List<InspectionSubjectDto>>())
           .Map(dest => dest.EdmsId, src => src.EdmsId)
           .Map(dest => dest.EquipmentTagNo, src => src.EquipmentTagNo)
           .Map(dest => dest.ITP_NO, src => src.ITP_NO)
           .Map(dest => dest.Project, src => src.Project.Adapt<ProjectDto>())
           .Map(dest => dest.Name, src => src.Name)
           .Map(dest => dest.No, src => src.No)
           .Map(dest => dest.Title, src => src.Title)
           .Map(dest => dest.Id, src => src.OrderId.Value) // Assuming OrderId has a Value property
           .Map(dest => dest.ExceptionContacts, src => src.ExceptionContacts.Select(ex => ex.ProjectContact.Contact).Adapt<List<ContactDto>>())
           .IgnoreNonMapped(true);

        TypeAdapterConfig<OrderDto, Order>.NewConfig()
           .Map(dest => dest.VendorDocuments, src => src.VendorDocuments!= null ? src.VendorDocuments.Adapt<List<VendorDocumentDto>>(): new List<VendorDocumentDto>())
           .Map(dest => dest.Contacts, src => src.Contacts == null ? new List<OrderContact>() : src.Contacts.Adapt<List<OrderContact>>())
           //  .Map(dest => dest.ExceptionContacts, src => src.ExceptionContacts == null ? new List<OrdersettingException>() : src.Contacts.Adapt<List<OrdersettingException>>())
           .Map(dest => dest.Version, src => src.Version!=null ? src.Version : "0")
           .Map(dest => dest.Subjects, src => src.InspectionSubjects == null ? new List<OrderInspectionSubject>() : src.InspectionSubjects.Adapt<List<OrderInspectionSubject>>())
           .Map(dest => dest.EdmsId, src => src.EdmsId!= null ? src.EdmsId: "")
           .Map(dest => dest.EquipmentTagNo, src => src.EquipmentTagNo!= null ? src.EquipmentTagNo: "")
           .Map(dest => dest.ITP_NO, src => src.ITP_NO != null ? src.ITP_NO :"")
           .Map(dest => dest.Project, src => src.Project!= null ? src.Project.Adapt<Project>() : new Project())
           .Map(dest => dest.Name, src => src.Name != null ? src.Name : "")
           .Map(dest => dest.No, src => src.No != null ? src.No :"")
           .Map(dest => dest.Title, src => src.Title != null ? src.Title: "")
           .Map(dest => dest.OrderId, src => src.Id != null ? src.Id : Guid.Empty   ) // Assuming OrderId has a Value property

           .IgnoreNonMapped(true)
           .AfterMapping((src, dest) => {
               if(src.Project != null && src.Project.Id != null)
                    dest.Project.ProjectId = new ProjectId(src.Project.Id.Value);
               dest.Id = new OrderId(src.Id);
           });


        TypeAdapterConfig<VendorDocument, VendorDocumentDto>.NewConfig()
            .Map(dest => dest.Id, src => src.VendorDocumentId.Value) // Map VendorDocumentId.Value to Id
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Content, src => src.Content)
            .IgnoreNonMapped(true);


        TypeAdapterConfig<InspectionNotification, CreateInspectionNotificationDto>.NewConfig()
          .Map(dest => dest.Id, src => src.InspectionNotificationId.Value) // Map InspectionNotificationId.Value to Id
          .Map(dest => dest.Name, src => src.Name == null ? "" : src.Name)
          .Map(dest => dest.NotificationNumber, src => src.NotificationNumber == null ? "" : src.NotificationNumber)
          .Map(dest => dest.Order, src => src.Order.Adapt<OrderDto>()) // Map nested Order to OrderDto
          .Map(dest => dest.Contacts, src => src.Contacts == null ? new List<ContactDto>() : src.Contacts.Adapt<List<ContactDto>>())
          .Map(dest => dest.OrderName, src => src.OrderName == null ? "" : src.OrderName)
          .Map(dest => dest.EquipmentTagNo, src => src.EquipmentTagNo == null ? "" : src.EquipmentTagNo)
          .Map(dest => dest.RequestInspectionDateEnd, src => src.RequestInspectionDateEnd == null ? DateTime.Now : src.RequestInspectionDateEnd)
          .Map(dest => dest.RequestInspectionDateStart, src => src.RequestInspectionDateStart == null ? DateTime.Now : src.RequestInspectionDateStart)
          .Map(dest => dest.RequestInspectionPersianDateEnd, src => src.RequestInspectionPersianDateEnd == null ? "" : src.RequestInspectionPersianDateEnd)
          .Map(dest => dest.RequestInspectionPersianDateStart, src => src.RequestInspectionPersianDateStart == null ? "" : src.RequestInspectionPersianDateStart)
          .Map(dest => dest.RequestInspectionDueDate, src => src.RequestInspectionDueDate == null ? 0 : src.RequestInspectionDueDate)
          .Map(dest => dest.InspectionTimeComment, src => src.InspectionTimeComment == null ? "" : src.InspectionTimeComment)
          .Map(dest => dest.InspectionAction, src => src.InspectionAction == null ? "" : src.InspectionAction)
          .Map(dest => dest.InspectionActionComment, src => src.InspectionActionComment == null ? "" : src.InspectionActionComment)
          .Map(dest => dest.Description, src => src.Description == null ? "" : src.Description)
          .Map(dest => dest.IssueDate, src => src.IssueDate == null ? DateTime.Now : src.IssueDate)
          .Map(dest => dest.CreatedDate, src => src.CreatedDate == null ? DateTime.Now : src.IssueDate)
          .Map(dest => dest.ModifiedDate, src => src.ModifiedDate == null ? DateTime.Now : src.IssueDate)
          .Map(dest => dest.CreatedBy, src => src.CreatedBy == null ? "":src.CreatedBy)
          .Map(dest => dest.ModifiedBy, src => src.ModifiedBy==null ? "":src.ModifiedBy)
          .Map(dest => dest.PersianIssueDate, src => src.PersianIssueDate==null ? "":src.PersianIssueDate)
          // .Map(dest => dest.Attachments, src => src.Attachments.Adapt<List<AttachmentDto>>()) // Map Attachments to AttachmentDto list
          .Map(dest => dest.InspectionSubjects, src => src.Subjects == null ? new List<InspectionSubjectDto>() : src.Subjects.Adapt<List<InspectionSubjectDto>>()) // Map Subjects
          .Map(dest => dest.InspectionTypes, src => src.Types == null ? new List<InspectionTypeDto>() : src.Types.Adapt<List<InspectionTypeDto>>()) // Map Types          
          .Map(dest => dest.Locations, src => src.Locations == null ? new List<AddressDto>() : src.Locations.Adapt<List<AddressDto>>()) // Map Locations          
          .IgnoreNullValues(true)
          .IgnoreNonMapped(true); // Ignore any unmapped properti


        TypeAdapterConfig<CreateInspectionNotificationDto, InspectionNotification>.NewConfig()
       .Map(dest => dest.InspectionNotificationId,
            src => (src.Id.HasValue && src.Id.Value != Guid.Empty) ? new InspectionNotificationId(src.Id.Value)
                                   : InspectionNotificationId.CreateUnique())
       .Map(dest => dest.Name, src => src.Name != null ? src.Name : "")
       .Map(dest => dest.NotificationNumber, src => src.NotificationNumber)
       .Map(dest => dest.Order, src => src.Order == null ? new Order() : src.Order.Adapt<Order>())
       .Map(dest => dest.Revisions, src => src.Revisions == null ? new List<InspectionNotificationRevision>() : src.Revisions.Adapt<List<InspectionNotificationRevision>>())
       .Map(dest => dest.OrderName, src => src.OrderName == null ? "" : src.OrderName)
       .Map(dest => dest.EquipmentTagNo, src => src.EquipmentTagNo== null ? "" : src.EquipmentTagNo)
       .Map(dest => dest.Contacts, src => src.Contacts == null ? new List<InspectionNotificationContact>() : src.Contacts.Adapt<List<InspectionNotificationContact>>())       
       .Map(dest => dest.RequestInspectionDateEnd, src => src.RequestInspectionDateEnd)
       .Map(dest => dest.RequestInspectionDateStart, src => src.RequestInspectionDateStart)
       .Map(dest => dest.RequestInspectionPersianDateEnd, src => src.RequestInspectionPersianDateEnd)
       .Map(dest => dest.RequestInspectionPersianDateStart, src => src.RequestInspectionPersianDateStart)
       .Map(dest => dest.RequestInspectionDueDate, src => src.RequestInspectionDueDate)      
       .Map(dest => dest.InspectionAction, src => src.InspectionAction)
       .Map(dest => dest.InspectionActionComment, src => src.InspectionActionComment)
       .Map(dest=> dest.InspectionTimeComment,src=>src.InspectionTimeComment)
       .Map(dest => dest.Description, src => src.Description == null ? "" : src.Description)
       .Map(dest => dest.IssueDate, src => src.IssueDate == null ? DateTime.Now : src.IssueDate)
       .Map(dest => dest.CreatedDate, src => src.CreatedDate == null ? DateTime.Now : src.CreatedDate)
       .Map(dest => dest.ModifiedDate, src => src.ModifiedDate == null ? DateTime.Now : src.ModifiedDate)
       .Map(dest => dest.CreatedBy, src => src.CreatedBy == null ? "" : src.CreatedBy)
       .Map(dest => dest.ModifiedBy, src => src.ModifiedBy == null ? "" : src.ModifiedBy)
       .Map(dest => dest.PersianIssueDate, src => src.PersianIssueDate)
       .Map(dest => dest.Subjects, src => src.InspectionSubjects == null ? new List<InspectionNotificationSubject>() : src.InspectionSubjects.Adapt<List<InspectionNotificationSubject>>())
       .Map(dest => dest.Types, src => src.InspectionTypes == null ? new List<InspectionNotificationType>() : src.InspectionTypes.Adapt<List<InspectionNotificationType>>())
       .Map(dest => dest.Locations, src => src.Locations == null ? new List<InspectionNotificationLocation>() : src.Locations.Adapt<List<InspectionNotificationLocation>>()) // Map Types       
       .IgnoreNonMapped(true)
       .AfterMapping((src, dest) =>
       {
           if (dest.Contacts == null || dest.Contacts.Count == 0)
           {
               dest.Contacts = new List<InspectionNotificationContact>();
               InspectionNotificationContact inscontact;//= new InspectionNotificationContact();
               if (src.Contacts != null)
               {
                   foreach (var contact in src.Contacts)
                   {
                       inscontact = new InspectionNotificationContact();
                       inscontact.Contact = contact.Adapt<Contact>();
                       inscontact.ContactId = contact.Id;
                       inscontact.InspectionNotificationId = dest.InspectionNotificationId;
                       // assign the parent InspectionNotificationId
                       //contact.InspectionNotificationId = src.InspectionNotificationId;

                       // optionally also set navigation property
                       // contact.InspectionNotification = dest;
                       dest.Contacts.Add(inscontact);
                   }
               }
           }
           else {
               foreach (var contact in dest.Contacts)
               {
                   
                   contact.InspectionNotificationId = dest.InspectionNotificationId;                  
               }
           }
       });


        TypeAdapterConfig<OrderContact, OrderContactDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id) // Map Id directly
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.ContactId, src => src.ContactId)
            .Map(dest => dest.Order, src => src.Order.Adapt<OrderDto>()) // Map Order to OrderDto
            .Map(dest => dest.Contact, src => src.Contact.Adapt<ContactDto>()); // Map MCContactPerson

        // Map OrderInspectionSubject to OrderInspectionSubjectDto
        TypeAdapterConfig<OrderInspectionSubject, OrderInspectionSubjectDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id.ToString()) // Map Id to string
            .Map(dest => dest.Order, src => src.Order.Adapt<OrderDto>()) // Map Order to OrderDto
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.InspectionSubjectId, src => src.InspectionSubjectId)
            .Map(dest => dest.InspectionSubject, src => src.InspectionSubject.Adapt<InspectionSubjectDto>()); // Map InspectionSubject to InspectionSubjectDto

        // Add mapping for custom value objects (if needed)
        TypeAdapterConfig<InspectionSubjectId, string>.NewConfig()
            .MapWith(src => src.Value.ToString()); // Assuming InspectionSubjectId has a Value property of type Guid





        TypeAdapterConfig<InspectionNotification, InspectionNotificationDto>.NewConfig()
          .Map(dest => dest.Id, src => src.InspectionNotificationId.Value) // Map InspectionNotificationId.Value to Id
          .Map(dest => dest.Name, src => src.Name == null ? "" : src.Name)
          .Map(dest => dest.NotificationNumber, src => src.NotificationNumber)
          .Map(dest => dest.Order, src => src.Order.Adapt<OrderDto>()) // Map nested Order to OrderDto
          .Map(dest => dest.OrderName, src => src.OrderName == null ? "" : src.OrderName)
          .Map(dest => dest.EquipmentTagNo, src => src.EquipmentTagNo == null ? "" : src.EquipmentTagNo)
          .Map(dest => dest.Revisions, src => src.Revisions != null ? src.Revisions.Adapt<List<RevisionDto>>() : new List<RevisionDto>())
          .Map(dest => dest.Contacts, src => src.Contacts == null ? new List<ContactDto>() : src.Contacts.Select(x=>x.Contact).Adapt<List<ContactDto>>())
          .Map(dest => dest.RequestInspectionDateEnd, src => src.RequestInspectionDateEnd)
          .Map(dest => dest.RequestInspectionDateStart, src => src.RequestInspectionDateStart)
          .Map(dest => dest.RequestInspectionPersianDateEnd, src => src.RequestInspectionPersianDateEnd)
          .Map(dest => dest.RequestInspectionPersianDateStart, src => src.RequestInspectionPersianDateStart)
          .Map(dest => dest.RequestInspectionDueDate, src => src.RequestInspectionDueDate == null ? 0 : src.RequestInspectionDueDate)
          .Map(dest => dest.InspectionTimeComment, src => src.InspectionTimeComment)
          .Map(dest => dest.InspectionAction, src => src.InspectionAction)
          .Map(dest => dest.InspectionActionComment, src => src.InspectionActionComment)
          .Map(dest => dest.Description, src => src.Description)
          .Map(dest => dest.IssueDate, src => src.IssueDate)
          .Map(dest => dest.CreatedDate, src => src.CreatedDate)
          .Map(dest => dest.ModifiedDate, src => src.ModifiedDate)
          .Map(dest => dest.CreatedBy, src => src.CreatedBy)
          .Map(dest => dest.ModifiedBy, src => src.ModifiedBy)
          .Map(dest => dest.PersianIssueDate, src => src.PersianIssueDate == null ? "" : src.PersianIssueDate)
          //   .Map(dest => dest.Attachments, src => src.Attachments.Adapt<List<AttachmentDto>>()) // Map Attachments to AttachmentDto list
          .Map(dest => dest.InspectionSubjects, src => src.Subjects == null ? new List<InspectionSubjectDto>() : src.Subjects.Adapt<List<InspectionSubjectDto>>()) // Map Subjects          
          .Map(dest => dest.InspectionTypes, src => src.Types == null ? new List<InspectionTypeDto>() : src.Types.Adapt<List<InspectionTypeDto>>()) // Map Types
          .Map(dest => dest.Locations, src => src.Locations == null ? new List<AddressDto>() : src.Locations.Adapt<List<AddressDto>>()) // Map Types       

          .IgnoreNonMapped(true); // Ignore any unmapped properti


        TypeAdapterConfig<InspectionType, InspectionTypeDto>.NewConfig()
           .Map(dest => dest.Id, src => src.InspectionTypeId.Value.ToString())
           .Map(dest => dest.Name, src => src.Name)
           .IgnoreNonMapped(true);

        TypeAdapterConfig<OrdersettingException, OrderSettingExceptionDto>.NewConfig()
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.ProjectContactId, src => src.ProjectContactId)
            .Map(dest => dest.Order, src => src.Order.Adapt<OrderDto>()) // Map Order to OrderDto
            .Map(dest => dest.ProjectContact, src => src.ProjectContact.Adapt<ProjectContactDto>()); // Map ProjectContact


        config.NewConfig<BasicQCMemberInfo, BasicQCMemberInfoDto>()
                  .Map(dest => dest.Name, src => src.Name)
                  .Map(dest => dest.UserName, src => src.UserName)
                  .Map(dest => dest.DomainId, src => src.DomainId)
                  .Map(dest => dest.Projects, src => src.Projects.Adapt<List<ProjectDto>>())
                  .Map(dest => dest.Orders, src => src.Orders.Adapt<List<OrderDto>>())
                  .Map(dest => dest.EDMSOrders, src => src.EDMSOrders.Adapt<List<OrderDto>>())
                  .Map(dest => dest.OrderContacts, src => src.OrderContacts.Adapt<List<OrderContact>>())
                  .Map(dest => dest.InspectionTypes, src => src.InspectionTypes.Adapt<List<InspectionTypeDto>>())
                  .Map(dest => dest.InspectionSubjects, src => src.InspectionSubjects.Adapt<List<InspectionSubjectDto>>())
                  .Map(dest => dest.OrderInspectionSubjects, src => src.OrderInspectionSubjects.Adapt<List<OrderInspectionSubjectDto>>())
                  .Map(dest => dest.ReceivedInspectionNotifications, src => src.ReceivedInspectionNotifications.Adapt<List<InspectionNotificationDto>>())
                  .IgnoreNonMapped(true);


        config.NewConfig<BasicVendorInfo, BasicVendorInfoDto>()
                      .Map(dest => dest.Name, src => src.Name)
                      .Map(dest => dest.UserName, src => src.UserName)
                      .Map(dest => dest.DomainId, src => src.DomainId)
                      .Map(dest => dest.Projects, src => src.Projects == null ? new List<ProjectDto>() : src.Projects.Adapt<List<ProjectDto>>(config))
                      .Map(dest => dest.Orders, src => src.Orders == null ? new List<OrderDto>() : src.Orders.Adapt<List<OrderDto>>())
                      .Map(dest => dest.Participated_Circulations, src => src.Participated_Circulations == null ? new List<CirculationDto>() : src.Participated_Circulations.Adapt<List<CirculationDto>>())
                      .Map(dest => dest.OrderContacts, src => src.OrderContacts == null ? new List<OrderContactDto>() : src.OrderContacts.Adapt<List<OrderContactDto>>())
                      .Map(dest => dest.ProjectContacts, src => src.ProjectContacts == null ? new List<ProjectContactDto>() : src.ProjectContacts.Adapt<List<ProjectContactDto>>())
                      .Map(dest => dest.OrderSettingsExceptions, src => src.OrderSettingsExceptions == null ? new List<OrderSettingExceptionDto>() : src.OrderSettingsExceptions.Adapt<List<OrderSettingExceptionDto>>())
                      .Map(dest => dest.InspectionTypes, src => src.InspectionTypes == null ? new List<InspectionTypeDto>() : src.InspectionTypes.Adapt<List<InspectionTypeDto>>())
                      .Map(dest => dest.InspectionSubjects, src => src.InspectionSubjects == null ? new List<InspectionSubjectDto>() : src.InspectionTypes.Adapt<List<InspectionSubjectDto>>())
                      .Map(dest => dest.OrderInspectionSubjects, src => src.OrderInspectionSubjects == null ? new List<OrderInspectionSubjectDto>() : src.OrderInspectionSubjects.Adapt<List<OrderInspectionSubjectDto>>())
                      .Map(dest => dest.ReceivedInspectionNotifications, src => src.ReceivedInspectionNotifications == null ? new List<InspectionNotificationDto>() : src.ReceivedInspectionNotifications.Adapt<List<InspectionNotificationDto>>())
                      //.AfterMapping((src, dest) => {
                      //    for (var i = 0; i < src.Projects.Count; i++)
                      //        Console.Write("src project", src.Projects[i].ProjectId);
                      //})
                      .IgnoreNonMapped(true);


    }

    public static string[] ParseExtensions(string? allowed)
    {
        if (string.IsNullOrWhiteSpace(allowed)) return Array.Empty<string>();

        var parts = allowed
            .Split(new[] { ',', ';', ' ', '|' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(p => p.ToLowerInvariant())
            .ToList();

        if (parts.Contains("*") || parts.Contains("*.*")) return Array.Empty<string>();

        return parts
            .Select(p => p.TrimStart('*'))
            .Select(p => p.StartsWith('.') ? p : "." + p)
            .Distinct()
            .ToArray();
    }
}