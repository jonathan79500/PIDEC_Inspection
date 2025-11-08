using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.EDMS
{
    public class EDMSOrderDepartmentLeads
    {
        public string originator { set; get; }
        public string leadEngineer { set; get; }
        public string deptCode { set; get; }
    }
}
