using Inspection.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Infrastructure
{
    public interface IEDMSService
    {
        Task<List<Vendor>> GetVendorInformation(string vendorUserName,int domainId);
        Task<bool> UpdateDocumentToStateInuseInEDMS(string DocumentId,bool isUsed);
        Task<bool> VendorIsValid(string userName, string password, int domainId);
    }
}
