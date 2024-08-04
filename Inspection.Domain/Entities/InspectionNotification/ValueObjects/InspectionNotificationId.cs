using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.InspectionNotification.ValueObjects
{

    //public readonly struct InspectionNotificationId : IComparable<InspectionNotificationId>, IEquatable<InspectionNotificationId>
    //{
    //    public Guid Value { get; }

    //    public InspectionNotificationId(Guid value)
    //    {
    //        Value = value;
    //    }

    //    public static InspectionNotificationId New() => new InspectionNotificationId(Guid.NewGuid());

    //    public bool Equals(InspectionNotificationId other) => this.Value.Equals(other.Value);
    //    public int CompareTo(InspectionNotificationId other) => Value.CompareTo(other.Value);

    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj)) return false;
    //        return obj is InspectionNotificationId other && Equals(other);
    //    }

    //    public override int GetHashCode() => Value.GetHashCode();
    //    public override string ToString() => Value.ToString();

    //    public static bool operator ==(InspectionNotificationId a, InspectionNotificationId b) => a.CompareTo(b) == 0;
    //    public static bool operator !=(InspectionNotificationId a, InspectionNotificationId b) => !(a == b);
    //}

    public sealed record InspectionNotificationId //: ValueObject
    {
        public Guid Value { get; }

        public InspectionNotificationId()
        { }

        public InspectionNotificationId(Guid value)
        { Value = value; }

        public static InspectionNotificationId CreateUnique()
        {
            return new(Guid.NewGuid());
        }

        //public override IEnumerable<object> GetEqualityComponents()
        //{
        //    yield return Value;
        //}

        public static InspectionNotificationId Create(Guid value)
        {
            return new InspectionNotificationId(value);
        }

    }

    //public readonly record struct InspectionNotificationId(Guid value);
}
