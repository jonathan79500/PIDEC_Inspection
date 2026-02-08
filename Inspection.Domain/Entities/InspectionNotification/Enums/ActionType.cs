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
            CreateNewVersionForInspection = 2,
            PropagateInspectionToPIDECOrganizations = 3,
            ConfirmInspectionNotification = 4,
            RejectInspectionNotification = 5,
            BackwardInspectionNotification = 6,
            CompleteInspectionNotification = 7,
            NoAction = -1
        }
    
}
