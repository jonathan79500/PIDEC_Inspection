using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;

namespace Inspection.Domain.Entities.InspectionNotification.Entities;

public sealed class Attachment:BaseDomainEntity<AttachmentId>
{

    public AttachmentId AttachmentId { get; private set; }

    public Attachment(AttachmentId attachmentId):base()
    {
        AttachmentId = attachmentId;
    }
    
    private Attachment(string name, string description, byte[] content) : base()
    {
        AttachmentId.Create(Guid.NewGuid());
        Name = name;
        Description = description;
        Content = content;
    }

    public string Name { get;private set; }
    public string? Description { get;private set; }
    public byte[] Content { get;private set; }

    public static Attachment Create(string name, string description, byte[] content)
    {
        return new(name, description, content);
    }
}
