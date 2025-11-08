using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Primitives
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        public abstract IEnumerable<object> GetEqualityComponents();
        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType()) return false;
            var valueObject = obj as ValueObject;
            return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents().Select(x => x?.GetHashCode() ?? 0).Aggregate((x, y) => x ^ y);
        }

        public bool Equals(ValueObject? other)
        {
            return Equals(this, other);
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if ((left is null && right is not null) || (left is not null && right is null)) return false;
            if (left is null && right is null) return true;
            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right) { 
            if((left is null && right is not null) || (left is not null && right is null)) return true;
            if(left is null && right is null) return false;
            return !left.Equals(right); 
        }

        
    }
}
