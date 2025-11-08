using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.Entities
{
    public class InspectionNotificationSubject:BaseDomainEntity<Guid>
    {
        public InspectionNotificationSubject() { }  

        public InspectionNotificationId InspectionNotificationId { get; set; }
        public InspectionSubjectId? InspectionSubjectId { get;  set; }

        public InspectionNotification? InspectionNotification { get; set; }
        public InspectionSubject? InspectionSubject { get; set; }
    }
}
