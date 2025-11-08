using Inspection.Domain.Primitives;
using System;
using System.Collections.Generic;


namespace Inspection.Domain.Entities.ValueObjects;


public sealed class ProjectId : ValueObject
{
    public Guid Value { get; }

    public ProjectId()
    {
    }

    public ProjectId(Guid value)
    { 
        Value = value;
    }


    public static ProjectId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static ProjectId Create(Guid value)
    {
        return new ProjectId(value);
    }
    // Implicit conversion from Guid to ProjectId
    public static implicit operator ProjectId(Guid value) => new ProjectId(value);

    // Implicit conversion from ProjectId to Guid
    public static implicit operator Guid(ProjectId projectId) => projectId.Value;

    public override string ToString() => Value.ToString();

}

