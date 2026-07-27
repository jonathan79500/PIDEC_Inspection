using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.Enums
{
    public enum ApprovalStatus
    {
        Approved = 1,
        Rejected=2,
        Backwarded =3,
        ApprovedWithComment = 4,
        Wave = 5,
        Review=6
    }
}
