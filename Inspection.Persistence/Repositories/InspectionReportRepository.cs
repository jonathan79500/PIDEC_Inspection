using Azure.Core;
using Dapper;
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Models;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.EDMS;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.User.Enums;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
//using Pidec.Utils;

namespace Inspection.Persistence.Repositories;

public class InspectionReportRepository : IGenericRepository<InspectionReport, InspectionReportId>, IInspectionReportRepository
{

    private readonly InspectionDbContext _dbContext;
    private IServiceScopeFactory _serviceProvider;
    private IEDMSService _eDMSService;
    private IUserManagerRepository _userManagerRepository;

    public InspectionReportRepository(InspectionDbContext dbContext, IServiceScopeFactory serviceProvider,IEDMSService eDMSService, IUserManagerRepository userManagerRepository) : base()
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _eDMSService = eDMSService;
        _userManagerRepository = userManagerRepository;
    }

    public Task<InspectionReport> Add(InspectionReport entity)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionReport> CreateNewInspectionReport(InspectionReport inspectionReport)
    {
        throw new NotImplementedException();
    }

    public Task Delete(InspectionReport entity)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionReportNumber> GenerateNewInspectionReportNumber(string NotificationNumber, int ReportType)
    {
        throw new NotImplementedException();
        //InspectionReportNumber inspectionReportNumber = _dbContext.Inspec.Where(nn => nn.ProjectNumber == projectNumber && nn.OrderNumber == orderNumber).FirstOrDefault()!;

        //if (notificationNumber?.Id != null)
        //{
        //    // return Task.FromResult(notificationNumber.CreateNewVersion());
        //    return await Task.FromResult(notificationNumber.IncreaseVersion());
        //}
        //else
        //{
        //    return await Task.FromResult(NotificationNumber.CreateFromNull(projectNumber, orderNumber, "NTF"));
        //}
    }

    public Task<InspectionReport> Get(InspectionReportId id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<InspectionReport>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<InspectionReport>> GetDraftInspectionReportsByUserName(string userName)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionReport> GetInspectionReportById(InspectionReportId inspectionReportId)
    {
        throw new NotImplementedException();
    }

    public Task<List<InspectionReport>> GetInspectionReportsByInspectorUserId(string InspectorUserId)
    {
        throw new NotImplementedException();
    }

    public Task<List<InspectionReport>> GetInspectionReportsByNotificationId(InspectionNotificationId inspectionNotificationId)
    {
        throw new NotImplementedException();
    }

    public Task<List<InspectionReport>> GetIssuedInspectionReportsByUserName(string userName)
    {
        throw new NotImplementedException();
    }

    public Task<List<InspectionReport>> GetNotConfirmedInspectionReportsByUserName(string userName)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionNotification> SendNewInspectionNotification(InspectionNotification inspectionNotification, Project project)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionReport> Update(InspectionReport entity)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionReport> UpdateInspectionReport(InspectionReport inspectionReport)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateInspectionReportNumber(InspectionReportNumber inspectionReportNumber)
    {
        throw new NotImplementedException();
    }

}
