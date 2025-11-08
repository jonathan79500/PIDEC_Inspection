using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class ActionTypeDto
    {
        public ActionTypeDto() { }
        public Guid ActionTypeId { get; set; }
        public string ActionName { get; set; } = "Save | Submit | QCControl | NewVersion | Confirm | Reject |Distribute";
        public int Code { get; set; } // Save =0 | Submit = 1 |Confirm = 2 | Distribute = 3 | Reject = 4
    }
}
