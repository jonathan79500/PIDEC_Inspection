using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.Enums
{
   
        public enum ContactType
        {
            Undefined = 0,
            Owner = 1,
            Vendor = 2,
            MC = 3,
            PIDEC_TO = 4,
            Supplier = 5,
            SubSupplier = 6,
            PIDEC_CC = 7,
            PIDEC_Inspector = 8,
            ThirdPart_Inspector = 9,
            PIDEC_LeadDepartment = 10,
            PIDEC_Originator =11
        }

    
}
