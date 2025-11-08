using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspection.Domain.Entities.InspectionNotification.Entities;

namespace Inspection.Domain.Entities.Common
{
    public class InspectionNotificationFlow
    {
        public List<InspectionNotification.Entities.InspectionNotification> InspectionNotifications { get; set; }
        public List<Circulation> Circulations { get; set; }
        public List<InspectionNotificationContact> Contacts { get; set; }
        public List<InspectionNotificationSubject> Subjects { get; set; }
        public List<InspectionNotificationType> Types { get; set; }
    }
}
