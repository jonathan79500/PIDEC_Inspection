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
using Inspection.Domain.Entities.Enums;
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
using System.Net.Mail;
using System.Runtime.InteropServices.JavaScript;
using Attachment = Inspection.Domain.Entities.InspectionNotification.Entities.Attachment;
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

    public async Task<(InspectionReport, Circulation)> CreateNewInspectionReport(InspectionReport rawinspectionReport,InspectionReportNumber newInspectionReportNumber, List<Attachment> attachments)
    {

        InspectionReport inspectionReport = rawinspectionReport;

        try
        {

            foreach (InspectionReportPerson insrPerson in rawinspectionReport.InspectionPersons!.ToList())
            {                
                var existingPerson = await _dbContext.Persons.FirstOrDefaultAsync(c => c.Id == insrPerson.PersonId);

                Person personEntity;

                if (existingPerson != null)
                {
                    // If Table3 exists, use it
                    personEntity = existingPerson;
                    // Make sure EF Core treats it as an existing record:
                    _dbContext.Entry(personEntity).State = EntityState.Unchanged;

                    var existingInsContact = await _dbContext.InspectionReportPersons.FirstOrDefaultAsync(c => c.PersonId == insrPerson.PersonId && c.InspectionReportId == insrPerson.InspectionReportId);
                    if (existingInsContact != null)
                        _dbContext.Entry(insrPerson).State = EntityState.Unchanged;
                    else
                        _dbContext.Entry(insrPerson).State = EntityState.Added;
                }
                else if (insrPerson.Person != null)
                {
                    // inscontact.InspectionNotificationId = inspectionNotification.Id;
                    InspectionReportPerson findedInsrPerson = rawinspectionReport.InspectionPersons.Find((psn) => psn.PersonId == insrPerson.PersonId);

                    if (findedInsrPerson != null)
                    {
                        //_dbContext.InspectionSubjects.Add(findedSubject.InspectionSubject);
                        insrPerson.Person = findedInsrPerson.Person;
                    }

                    _dbContext.Entry(insrPerson.Person).State = EntityState.Added;
                    _dbContext.Entry(insrPerson).State = EntityState.Added;
                }
                else
                {
                    personEntity = null;
                    inspectionReport.InspectionPersons!.Remove(insrPerson);
                }
            }



            if (inspectionReport.Revision != null && inspectionReport.Revision.RevisionId != null)
            {
                //before this time , Request and circulation are created
            }
            else
            {

                Contact creator = inspectionReport.Creator;
                string currentDatePersian = "";
                DateTime currentTime = DateTime.Now;

                Domain.Entities.Common.Request newRequest = Domain.Entities.Common.Request.CreateNewRequestWithRevision(RequestTypes.InspectionNotification, creator);

                newRequest.Revisions[0].RevisionNumber = newInspectionReportNumber.Revision;
                newRequest.Revisions[0].VersionNumber = newInspectionReportNumber.Version;
                newRequest.Revisions[0].Comment = "First Time Create";
                newRequest.Revisions[0].IsFinal = false;
                newRequest.Revisions[0].SubmittedBy = creator;
                newRequest.Revisions[0].SubmittedDate = DateTime.Now;

                inspectionReport.Revision = newRequest.Revisions[0];
                InspectionReport savedNewInspectionReport = _dbContext.InspectionReports.Add(inspectionReport).Entity;

                Circulation circulation = Circulation.CreateNew(newRequest.Revisions[0], creator);

                circulation.PerformerContactType = creator.ContactType;
                circulation.CreatedDate = currentTime;
                circulation.PerformedActionDateGregorian = currentTime;
                circulation.PerformedActionDate = "";                
                newRequest.Revisions[0].Circulations!.Add(circulation);

                _dbContext.SaveChanges();

                CirculationAttachments addedCA;
                if (circulation.Attachments != null)
                    circulation.Attachments.Clear();

                foreach (Attachment attachment in attachments)//rawinspectionNotification.Attachments!.ToList())
                {

                    // var existingAttachment = await _dbContext.Attachments.FirstOrDefaultAsync(atc => atc.Id.Value == attachment.Id.Value);
                    var existingAttachment = await _dbContext.Attachments.FirstOrDefaultAsync(a => a.AttachmentId == attachment.Id);


                    CirculationAttachments newCirculationAttachment = new CirculationAttachments();
                    newCirculationAttachment.Id = Guid.NewGuid();
                    newCirculationAttachment.CirculationId = circulation.Id;
                    _dbContext.Entry(circulation).State = EntityState.Unchanged;

                    if (existingAttachment != null)
                    {
                        _dbContext.Entry(existingAttachment).State = EntityState.Unchanged;
                        newCirculationAttachment.AttachmentId = existingAttachment.AttachmentId;
                    }
                    else
                    {
                        Attachment newAttachment = Attachment.Create(attachment.Name, attachment.Description, attachment.Content, attachment.Version, "-1", "0", attachment.ContentType, attachment.DocNo, attachment.Status);
                        newAttachment.AttachmentId = attachment.Id;
                        _dbContext.Entry(newAttachment).State = EntityState.Added;
                        newCirculationAttachment.AttachmentId = newAttachment.AttachmentId;
                    }
                    _dbContext.Entry(newCirculationAttachment).State = EntityState.Added;
                    addedCA = _dbContext.CirculationAttachments.Add(newCirculationAttachment).Entity;
                    // circulation.Attachments.Add(addedCA);

                }


                _dbContext.SaveChanges();



                return await Task.FromResult((inspectionReport, circulation));
            }
        }
        catch (Exception ex)
        {

            string str = ex.Message;
        }


        return (null, null);
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
