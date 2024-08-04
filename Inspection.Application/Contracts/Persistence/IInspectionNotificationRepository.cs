using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;


namespace Inspection.Application.Contracts.Persistence;

public interface IInspectionNotificationRepository : IGenericRepository<InspectionNotification, InspectionNotificationId>
{
    
}
