using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.ValueObjects
{
  
        public sealed class InspectionReportId : ValueObject
        {
            public Guid Value { get; }

            public InspectionReportId()
            { }

            public InspectionReportId(Guid value)
            { Value = value; }


            public static InspectionReportId CreateUnique()
            {
                return new(Guid.NewGuid());
            }

            public override IEnumerable<object> GetEqualityComponents()
            {
                yield return Value;
            }

            public static InspectionReportId Create(Guid value)
            {
                return new InspectionReportId(value);
            }
        // Implicit conversion from Guid to InspectionReportId
        public static implicit operator InspectionReportId(Guid value) => new InspectionReportId(value);

            // Implicit conversion from InspectionReportId to Guid
            public static implicit operator Guid(InspectionReportId inspectionReportId) => inspectionReportId.Value;

            public override string ToString() => Value.ToString();

        }
       
    
}
