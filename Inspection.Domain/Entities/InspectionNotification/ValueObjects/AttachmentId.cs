using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;


namespace Inspection.Domain.Entities.InspectionNotification.ValueObjects;

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


public sealed record AttachmentId
{
    public Guid Value { get; }

    public AttachmentId()
    {
        
    }

    public AttachmentId(Guid value)
    { Value = value; }

    public static AttachmentId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

   

    public static AttachmentId Create(Guid value)
    {
        return new AttachmentId(value);
    }

}

//public readonly record struct AttachmentId(Guid value);

