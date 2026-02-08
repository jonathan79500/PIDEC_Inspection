using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class InspectionSubject
    {
        public InspectionSubjectId? InspectionSubjectId { get; set; }
        public string Name { get; set; }      
        public ContactId? CreatedBy_ContactId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? VisibleByDefault { get; set; }
        public List<InspectionNotificationSubject> InspectionNotifications { get; set; }
        public List<OrderInspectionSubject> Orders { get; set; }

        public InspectionSubject() { }
        public InspectionSubject(string name) { }


        private InspectionSubject(InspectionSubjectId inspectionSubjectId)
        {

            InspectionSubjectId = inspectionSubjectId;
            Name = string.Empty;
        }

        public static InspectionSubject Create(string name)
        {
            return new InspectionSubject(InspectionSubjectId.CreateUnique())
            {
                Name = name
            };
        }



    }
}
