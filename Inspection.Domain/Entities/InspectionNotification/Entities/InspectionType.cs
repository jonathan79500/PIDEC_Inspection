using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
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
        public InspectionType()
        {
            
        }

        private InspectionType(InspectionTypeId inspectionTypeId)
        {

            InspectionTypeId = inspectionTypeId;
            Name = string.Empty;
        }

        public static InspectionType Create(Guid id, string name)
        {
            return new InspectionType(new InspectionTypeId(id)) 
            {
                Name= name
            };
        }
        public InspectionTypeId InspectionTypeId { get; private set; }

        public string Name { get; set; }
    }
}
