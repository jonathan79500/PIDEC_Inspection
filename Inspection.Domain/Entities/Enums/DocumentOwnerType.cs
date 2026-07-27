using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Enums
{
    public enum DocumentOwnerType
    {
        Unknown = 0,

        Circulation = 1,
        Inspector = 2,
        Vendor = 3,
        QC = 4,
        MC = 5,
        Owner = 6,
        ThirdPartyInspector = 7
    }
}
