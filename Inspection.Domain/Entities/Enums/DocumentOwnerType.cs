using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Enums
{
    /// <summary>
    /// Actually DocumentOwnerType specify in which Table we should find OwnerId of Document
    /// for example if OwnerType=Circulation and OwnerId=(a guid) I should search Circulation.CirculationId = OwnerId
    /// </summary>
    public enum DocumentOwnerType
    {
        Unknown = 0,

        Circulation = 1,
        Inspector = 2,
        Contact = 3,              // any Contacts row — vendor, owner, MC, PIDEC, etc.
        InspectionNotification = 4
    }

 
}
