using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.Enums
{
   
        public enum ActionTypes
        {
            CreateInspection = 0,
            SendInspectionToPIDEC = 1,
            CreateNewVersionForInspection = 4,
            PropagateInspectionToPIDECOrganizations = 6,
            ConfirmInspectionNotification = 1,
            RejectInspectionNotification = 2,
            BackwardInspectionNotification = 3,
            CompleteInspectionNotification = 7,
            NoAction = -1
        }
    
}
