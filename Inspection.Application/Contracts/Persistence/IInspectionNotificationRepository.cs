using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Inspection.Application.Contracts.Persistence;

public interface IInspectionNotificationRepository : IGenericRepository<InspectionNotification, InspectionNotificationId>
{
    Task<InspectionNotification> GetInspectionNotificationById(InspectionNotificationId inspectionNotificationId);

    Task<(InspectionNotification,Circulation)> CreateNewInspectionNotification(InspectionNotification inspectionNotification,List<Attachment> attachments);

    //Task<InspectionNotification> SendNewInspectionNotification(InspectionNotification inspectionNotification,Project project);

    Task<NotificationNumber> GenerateNewNotificationNumber(string projectNumber, string orderNumber);

    Task<bool> UpdateNotificationNumber(NotificationNumber notificationNumber);

    Task<InspectionNotificationFlow> GetInspectionNotificationFlow(string notificationNumber);

    Task<InspectionNotification> UpdateInspectionNotification(InspectionNotification inspectionNotification);

    Task<List<InspectionNotification>> GetInspectionNotificationsByProjUsrIds(string ProjectId, string UserId);

    Task<List<InspectionNotificationDataSummaryDto>> GetDraftInspectionNotificationsByUserName(string userName);

    Task<List<InspectionNotificationDataSummaryDto>> GetSentInspectionNotificationsByUserName(string userName);

    Task<List<InspectionNotificationDataSummaryDto>> GetNotStartedInspectionNotificationsByUserName(string userName);

   // InspectionNotification SimulateInpectionDataForNextUsers(InspectionNotification inspectionNotification, Project project);

    Task<Contact> AddNewContact(Contact contact);

    Task<Contact> GetContactByUserId(string userId);

    Task<InspectionNotification> AddNewInspectionNotification(InspectionNotification inspectionNotification);

    Task<bool> SaveInspectionNotificationCirculations(List<Circulation> circulations);
    
    Task<Circulation> CompleteCirculation(Circulation circulation);
}
