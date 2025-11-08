using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.Entities
{
    //[ComplexType]
    public sealed class InspectionType
    {

        public List<InspectionNotificationType> InspectionNotifications { get; set; }

        public InspectionTypeId InspectionTypeId { get; set; }

        public string Name { get; set; }

        public ContactId? CreatedBy_ContactId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? VisibleByDefault { get; set; }


        public InspectionType()
        {
            
        }

        private InspectionType(InspectionTypeId inspectionTypeId)
        {

            InspectionTypeId = inspectionTypeId;
            Name = string.Empty;
        }

        public static InspectionType Create(string name)
        {
            return new InspectionType(InspectionTypeId.CreateUnique()) 
            {
                Name= name
            };
        }

       
    }
}
