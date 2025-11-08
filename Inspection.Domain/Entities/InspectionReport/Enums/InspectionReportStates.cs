using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionReport.Enums
{
    public enum InspectionReportStates
    {
        None = 0,
        Conditional =1,
        Confirm =2,
        Reject =3,
        Waiting = 4,
        Mismatch = 5,
        ResultBasedOnTable = 6
    }
}
