using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;


namespace Inspection.Domain.Entities.ValueObjects;

//public sealed class AttachmentId : ValueObject
//{
//    public Guid Value { get; }

//    public AttachmentId(Guid value)
//    { Value = value; }

//    public static AttachmentId CreateUnique()
//    {
//        return new(Guid.NewGuid());
//    }

//    public override IEnumerable<object> GetEqualityComponents()
//    {
//        yield return Value;
//    }

//    public static AttachmentId Create(Guid value)
//    {
//        return new AttachmentId(value);
//    }

//}


public sealed record VendorDocumentId
{
    public Guid Value { get; }

    public VendorDocumentId()
    {
        
    }

    public VendorDocumentId(Guid value)
    { Value = value; }

    public static VendorDocumentId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

   

    public static VendorDocumentId Create(Guid value)
    {
        return new VendorDocumentId(value);
    }

}

//public readonly record struct AttachmentId(Guid value);

