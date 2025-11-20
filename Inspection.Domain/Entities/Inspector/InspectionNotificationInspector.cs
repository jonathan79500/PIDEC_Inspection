using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class InspectionNotificationInspector
    {
        public InspectionNotificationInspector() { }
        public Guid IssuerUserId { get; set; }
        public InspectionNotificationId InspectionNotificationId { get; set; }
        public Inspector Inspector { get; set; }
        public int ActionType { get; set; } //ConfirmedByQC = 1; RejectByQC =0
        public DateTime issueDate { get; set; }
        public DateTime QCActionDate { get; set; }
        public string QCComment { get; set; }
    }
}
