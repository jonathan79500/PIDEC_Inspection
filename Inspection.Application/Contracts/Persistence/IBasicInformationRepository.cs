using Inspection.Domain.Entities;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Persistence
{
    public interface IBasicInformationRepository
    {
        Task<List<VendorDocument>> GetVendorDocumentsByOrder(Vendor vendor, Order order);
        Task<List<Order>> GetOrdersByProject(Project project);
        Task<List<Project>> GetProjects();
        Task<List<Project>> GetProjectsByVendor(Vendor vendor);
    }
}
