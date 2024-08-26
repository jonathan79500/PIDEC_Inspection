
using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities;
using Inspection.Domain.Entities.InspectionNotification.Entities;

namespace Inspection.Persistence.Repositories;

public class BasicInformationRepository : IBasicInformationRepository
{
    private InspectionDbContext _dbcontext;
    public BasicInformationRepository(InspectionDbContext dbContext)
    {
        _dbcontext = dbContext;
    }

    public Task<List<Order>> GetOrdersByProject(Project project)
    {
        List<Order> projectOrders = _dbcontext.Orders.Where(ord=>ord.Project.Id == project.Id).ToList();
        return Task.FromResult(projectOrders);
    }

    public Task<List<Project>> GetProjects()
    {
        List<Project> projects = _dbcontext.Projects.ToList();
        return Task.FromResult(projects);
    }

    public Task<List<VendorDocument>> GetVendorDocumentsByOrder(Vendor vendor, Order order)
    {
        List<VendorDocument> vendorDocuments = _dbcontext.Vendors
                                                                .Where(v=>v.Id == vendor.Id).FirstOrDefault()!.Orders
                                                                .Where(ord => ord.Id == order.Id).FirstOrDefault()!.VendorDocuments.ToList();
        
        return Task.FromResult(vendorDocuments);
    }

    Task<List<Project>> IBasicInformationRepository.GetProjectsByVendor(Vendor vendor)
    {

        List<Project> projects = _dbcontext.Projects.Where(proj => proj.vendors.Any(v=>v.VendorId==vendor.Id)).ToList();
        return Task.FromResult(projects);
    }
}
