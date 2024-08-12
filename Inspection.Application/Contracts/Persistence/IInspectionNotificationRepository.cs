using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Inspection.Application.Contracts.Persistence;

public interface IInspectionNotificationRepository : IGenericRepository<InspectionNotification, InspectionNotificationId>
{
    Task<InspectionNotification> CreateNewInspectionNotification(InspectionNotification inspectionNotification);  
}
