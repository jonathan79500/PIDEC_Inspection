using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.ValueObjects
{
  
        public sealed class InspectionReportAttachmentId : ValueObject
        {
            public Guid Value { get; }

            public InspectionReportAttachmentId()
            { }

            public InspectionReportAttachmentId(Guid value)
            { Value = value; }


            public static InspectionReportAttachmentId CreateUnique()
            {
                return new(Guid.NewGuid());
            }

            public override IEnumerable<object> GetEqualityComponents()
            {
                yield return Value;
            }

            public static InspectionReportAttachmentId Create(Guid value)
            {
                return new InspectionReportAttachmentId(value);
            }
        // Implicit conversion from Guid to InspectionReportId
        public static implicit operator InspectionReportAttachmentId(Guid value) => new InspectionReportAttachmentId(value);

            // Implicit conversion from InspectionReportId to Guid
            public static implicit operator Guid(InspectionReportAttachmentId inspectionReportAttachmentId) => inspectionReportAttachmentId.Value;

            public override string ToString() => Value.ToString();

        }
       
    
}
