using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.User.Enums
{
    public enum UserType
    {
        UndefinedUser = 0,
        Owner = 1,
        Vendor = 2,
        MC = 3,
        PIDEC = 4,
        PIDECDepartmentLead = 5,
        PIDECDepartmentOriginator = 6,
        PIDECInspector = 7,
        ThirdPartyInspector =8
    }
}
