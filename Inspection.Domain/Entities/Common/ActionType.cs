using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class ActionType
    {
        public ActionType() { }
        public Guid ActionTypeId { get; set; }
        public string ActionName { get; set; } = "NoAction | Save | Submit | QCControl | NewVersion | Confirm | Reject |Distribute";
        public int Code { get; set; } // NoAction = -1 | Save =0 | Submit = 1 |Confirm = 2 | Distribute = 3 | Reject = 4

    }
}
