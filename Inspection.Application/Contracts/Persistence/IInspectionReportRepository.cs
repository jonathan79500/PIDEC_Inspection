using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Inspection.Application.Contracts.Persistence;

public interface IInspectionReportRepository : IGenericRepository<InspectionReport, InspectionReportId>
{
    Task<InspectionReport> GetInspectionReportById(InspectionReportId inspectionReportId);

    Task<List<InspectionReport>> GetInspectionReportsByNotificationId(InspectionNotificationId inspectionNotificationId);

    Task<(InspectionReport,Circulation)> CreateNewInspectionReport(InspectionReport inspectionReport,InspectionReportNumber newInspectionReportNumber, List<Attachment> attachments);   

    Task<InspectionReportNumber> GenerateNewInspectionReportNumber(string NotificationNumber, int ReportType/*pcking List, General Report*/);

    Task<bool> UpdateInspectionReportNumber(InspectionReportNumber inspectionReportNumber);

    Task<InspectionReport> UpdateInspectionReport(InspectionReport inspectionReport);

    Task<List<InspectionReport>> GetInspectionReportsByInspectorUserId(string InspectorUserId);

    Task<List<InspectionReport>> GetDraftInspectionReportsByUserName(string userName);

    Task<List<InspectionReport>> GetIssuedInspectionReportsByUserName(string userName);

    Task<List<InspectionReport>> GetNotConfirmedInspectionReportsByUserName(string userName);

}
