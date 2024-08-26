using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.ValueObjects;
using System;

namespace Inspection.Domain.Entities;

public class VendorDocument:BaseDomainEntity<VendorDocumentId>
{
    public VendorDocumentId VendorDocumentId { get;private set; }

    private VendorDocument()
    { 
    }

    public VendorDocument(VendorDocumentId VendorDocumentId) : base()
    {
        VendorDocumentId = VendorDocumentId;
    }

    private VendorDocument(string name, string description, byte[] content) : base()
    {
        VendorDocumentId.Create(Guid.NewGuid());
        Name = name;
        Description = description;
        Content = content;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public byte[] Content { get; private set; }
    public Order Order { get; private set; }

    public static VendorDocument Create(string name, string description, byte[] content)
    {
        return new(name, description, content);
    }
}
