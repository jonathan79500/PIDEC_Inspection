using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.ValueObjects
{
    // public readonly record struct InspectionTypeId(Guid value);

    public sealed class InspectionTypeId : ValueObject
    {
        public Guid Value { get; }

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

    }

}
