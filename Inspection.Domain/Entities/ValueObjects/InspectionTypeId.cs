using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.ValueObjects
{
    
    public sealed class InspectionTypeId : ValueObject
    {
        public Guid Value { get; }

        public InspectionTypeId()
        { }

        public InspectionTypeId(Guid value)
        { Value = value; }


        public static InspectionTypeId CreateUnique()
        {
            return new(Guid.NewGuid());
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static InspectionTypeId Create(Guid value)
        {
            return new InspectionTypeId(value);
        }
        // Implicit conversion from Guid to InspectionTypeId
        public static implicit operator InspectionTypeId(Guid value) => new InspectionTypeId(value);

        // Implicit conversion from InspectionTypeId to Guid
        public static implicit operator Guid(InspectionTypeId inspectionTypeId) => inspectionTypeId.Value;

        public override string ToString() => Value.ToString();

    }
}
