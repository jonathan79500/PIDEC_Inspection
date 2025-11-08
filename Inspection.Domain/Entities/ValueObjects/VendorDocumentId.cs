using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;


namespace Inspection.Domain.Entities.ValueObjects;

public sealed class VendorDocumentId:ValueObject
{
    public Guid Value { get; }

    public VendorDocumentId()
    { }

    public VendorDocumentId(Guid value)
    { Value = value; }


    public static VendorDocumentId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static VendorDocumentId Create(Guid value)
    {
        return new VendorDocumentId(value);
    }
    // Implicit conversion from Guid to VendorDocumentId
    public static implicit operator VendorDocumentId(Guid value) => new VendorDocumentId(value);

    // Implicit conversion from VendorDocumentId to Guid
    public static implicit operator Guid(VendorDocumentId vendorDocumentId) => vendorDocumentId.Value;

    public override string ToString() => Value.ToString();
}

//public readonly record struct AttachmentId(Guid value);

