using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.ValueObjects
{

    public sealed class ContactId : ValueObject
    {
        public Guid Value { get; }

        public ContactId()
        { }

        public ContactId(Guid value)
        { Value = value; }


        public static ContactId CreateUnique()
        {
            return new(Guid.NewGuid());
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static ContactId Create(Guid value)
        {
            return new ContactId(value);
        }
        // Implicit conversion from Guid to ContactId
        public static implicit operator ContactId(Guid value) => new ContactId(value);

        // Implicit conversion from ContactId to Guid
        public static implicit operator Guid(ContactId contactId) => contactId==null? Guid.Empty : contactId.Value;

        public override string ToString() => Value.ToString();

    }
}
