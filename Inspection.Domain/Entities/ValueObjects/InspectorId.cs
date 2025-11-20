using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.ValueObjects
{
    public sealed class InspectorId : ValueObject
    {
        public Guid Value { get; }

        public InspectorId()
        { }

        public InspectorId(Guid value)
        { Value = value; }


        public static InspectorId CreateUnique()
        {
            return new(Guid.NewGuid());
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static InspectorId Create(Guid value)
        {
            return new InspectorId(value);
        }
        // Implicit conversion from Guid to InspectionReportId
        public static implicit operator InspectorId(Guid value) => new InspectorId(value);

        // Implicit conversion from InspectionReportId to Guid
        public static implicit operator Guid(InspectorId inspectorId) => inspectorId.Value;

        public override string ToString() => Value.ToString();

    }
}
