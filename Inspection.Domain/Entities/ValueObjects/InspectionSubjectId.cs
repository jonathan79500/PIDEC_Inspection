using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.ValueObjects
{



    public sealed class InspectionSubjectId : ValueObject
    {
        public Guid Value { get; }

        public InspectionSubjectId()
        { }

        public InspectionSubjectId(Guid value)
        { Value = value; }


        public static InspectionSubjectId CreateUnique()
        {
            return new(Guid.NewGuid());
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static InspectionSubjectId Create(Guid value)
        {
            return new InspectionSubjectId(value);
        }
        // Implicit conversion from Guid to InspectionSubjectId
        public static implicit operator InspectionSubjectId(Guid value) => new InspectionSubjectId(value);

        // Implicit conversion from InspectionSubjectId to Guid
        public static implicit operator Guid(InspectionSubjectId inspectionSubjectId) => inspectionSubjectId.Value;

        public override string ToString() => Value.ToString();

    }

    //public readonly record struct InspectionNotificationId(Guid value);
}
