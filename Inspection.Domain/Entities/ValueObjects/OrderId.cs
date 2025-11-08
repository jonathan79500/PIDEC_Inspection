using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;


namespace Inspection.Domain.Entities.ValueObjects;



public sealed class OrderId : ValueObject
{
    public Guid Value { get; }

    public OrderId()
    { }

    public OrderId(Guid value)
    { Value = value; }
    

    public static OrderId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static OrderId Create(Guid value)
    {
        return new OrderId(value);
    }
    // Implicit conversion from Guid to OrderId
    public static implicit operator OrderId(Guid value) => new OrderId(value);

    // Implicit conversion from OrderId to Guid
    public static implicit operator Guid(OrderId orderId) => orderId!.Value;

    public override string ToString() => Value.ToString();
}


