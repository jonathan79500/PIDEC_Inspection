using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities;

namespace Inspection.Persistence.Repositories;

public class InspectionNotificationRepository : IGenericRepository<InspectionNotification,InspectionNotificationId>, IInspectionNotificationRepository
{

    private readonly InspectionDbContext _dbContext;

    public InspectionNotificationRepository(InspectionDbContext dbContext) : base()
    {
        _dbContext = dbContext;
    }

    public Task<InspectionNotification> CreateNewInspectionNotification(InspectionNotification inspectionNotification)
    {
        try
        {
            _dbContext.Entry(inspectionNotification.Owner).State = EntityState.Unchanged;
            _dbContext.Entry(inspectionNotification.InspectionType!).State = EntityState.Unchanged;
          
            _dbContext.Entry(inspectionNotification.Order!).State = EntityState.Unchanged;
            _dbContext.Entry(inspectionNotification.Location!).State = EntityState.Unchanged;

        
            _dbContext.InspectionNotifications.Add(inspectionNotification);

            
            _dbContext.SaveChanges();
            return Task.FromResult(inspectionNotification);

        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }

        
        return null;
    }

    Task<InspectionNotification> IGenericRepository<InspectionNotification, InspectionNotificationId>.Add(InspectionNotification entity)
    {

        try
        {
            Order  order = _dbContext.Orders.Find(entity.Order.Id);
            _dbContext.Entry(order).State = EntityState.Unchanged;

            _dbContext.InspectionNotifications.Add(entity);

            _dbContext.SaveChanges();

        }
        catch(Exception ex) {
            string str = ex.Message;
        }

        return null;
    }

    Task IGenericRepository<InspectionNotification, InspectionNotificationId>.Delete(InspectionNotification entity)
    {
        throw new NotImplementedException();
    }

    async Task<InspectionNotification> IGenericRepository<InspectionNotification, InspectionNotificationId>.Get(InspectionNotificationId id)
    {
        var inspectionNotification = await _dbContext.InspectionNotifications
         //.Include(n => n.InspectionType)
         //.Include(n => n.Vendor)
         .FirstOrDefaultAsync(n => n.InspectionNotificationId == id);

        return inspectionNotification!;
    }

    async Task<IReadOnlyList<InspectionNotification>> IGenericRepository<InspectionNotification, InspectionNotificationId>.GetAll()
    {
        try
        {
            var inspectionNotification = await _dbContext.InspectionNotifications
            .Include(n => n.InspectionType)
            .Include(n => n.Owner)
            .Include(n => n.Order.Project)
            .Include(n => n.Location)
           // .Include(n => n.Contacts)
            .ToListAsync();

            return inspectionNotification!;
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    Task<InspectionNotification> IGenericRepository<InspectionNotification, InspectionNotificationId>.Update(InspectionNotification entity)
    {
        throw new NotImplementedException();
    }
}
