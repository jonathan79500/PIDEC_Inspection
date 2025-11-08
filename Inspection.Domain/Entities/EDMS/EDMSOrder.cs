using Inspection.Domain.Entities.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.EDMS
{
    public class EDMSOrder
    {
        public EDMSOrder() { }

        public string ProjectId;
        public int Dot_Id;
        public string No;
        public bool IsAccessibleByEmployer;
        public bool HasReadPermission;
        public edmsVendorDocuments[] VendorDocuments;
        public string ContactPersonUserName;
        public List<Contact> vendorContact;
        public string VendorCompanyName;
    }
}
