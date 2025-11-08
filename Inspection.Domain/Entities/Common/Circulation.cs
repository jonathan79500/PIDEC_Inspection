using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;

namespace Inspection.Domain.Entities.Common
{
    public class Circulation :BaseDomainEntity<Guid>
    {
        public Circulation() { 
        
           
        }
       // public Request? Request { get; set; }
        //public ActionType ActionType { get; set; }
        public int? ActionType { get; set; }
        public string? PerformerPN { get; set; }
        public string? PerformedActionDate { get; set; }
        public int? SequenceNo { get; set; }
        public DateTime? PerformedActionDateGregorian { get; set; }
        public Contact? PerformerContactPerson { get; set; }
        public int? PerformerContactType { get; set; }
        public int? QCActionType { get; set; }

        public string Comment { get; set; }
       // public InspectionNotificationId InspectionNotificationId { get; set; }
        
        //[NotMapped]
        public InspectionNotification.Entities.InspectionNotification InspectionNotification { get; set; }

        public static Circulation CreateNew(InspectionNotification.Entities.InspectionNotification inspectionNotification,Contact creatorContactPerson)
        {          
          

            string currentDate = "";
            return new Circulation()
            {                
                InspectionNotification = inspectionNotification, 
                ActionType = ActionTypes.CreateInspection.GetHashCode(),
                PerformerContactPerson = creatorContactPerson,
                SequenceNo = 0,
                Comment = ""
            };
        }

        public static Circulation CreateForNextContact(InspectionNotification.Entities.InspectionNotification inspectionNotification, Contact contactPerson)
        {


            string currentDate = "";
            return new Circulation()
            {
                InspectionNotification = inspectionNotification,
                ActionType = null,
                PerformerContactPerson = contactPerson,
                SequenceNo = 1,
                PerformedActionDate = null,
                PerformedActionDateGregorian = null,
                PerformerContactType= contactPerson.ContactType,
                Comment = ""
            };
        }
    }
}
