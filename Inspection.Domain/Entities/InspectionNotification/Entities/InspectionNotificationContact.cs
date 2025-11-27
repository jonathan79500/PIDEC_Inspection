using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.Entities
{
    public class InspectionNotificationContact:BaseDomainEntity<Guid>
    {       
        public InspectionNotificationContact() { }

        public new Guid Id { get; set; }

        public InspectionNotificationId? InspectionNotificationId { get;  set; }
        public Guid? ContactId { get;  set; }


        public InspectionNotification? InspectionNotification { get;  set; }
        public Contact? Contact { get;  set; }   

        public static InspectionNotificationContact Create(Contact contact, InspectionNotification inspectionNotification, ContactType contactType)
        {
            InspectionNotificationContact inspectionContact = new InspectionNotificationContact()
            {
                Contact = contact,
                InspectionNotification = inspectionNotification              
            };
            return inspectionContact;
        }
        
    }
}
