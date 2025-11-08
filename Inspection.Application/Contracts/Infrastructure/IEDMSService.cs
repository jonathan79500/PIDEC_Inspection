using Inspection.Application.DTOs.User;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.EDMS;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.Vendor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Infrastructure
{
    public interface IEDMSService
    {
        Task<Contact> GetVendorInformation(string vendorUserName,int domainId);
        Task<bool> UpdateDocumentToStateInuseInEDMS(string DocumentId,bool isUsed);
        Task<SystematicUser> UserIsValid(string userName, string password, int domainId);
        Task<List<Order>> GetProjectOrders(string[] projectIds);
        Task<List<EDMSOrder>> GetEdmsOrders_ByProId(string proId);
        Task<EDMSOrderDepartmentLeads> GetOrderDepartmentLeads(string orderDotId, string projectId);
    }
}
