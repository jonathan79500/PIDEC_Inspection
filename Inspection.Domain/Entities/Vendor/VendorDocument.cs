using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.ValueObjects;
using System;

namespace Inspection.Domain.Entities.Vendor;

public class VendorDocument : BaseDomainEntity<VendorDocumentId>
{
    public VendorDocumentId VendorDocumentId { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }
    public byte[] Content { get; set; }
    public Order Order { get; set; }
    public int Version { get; set; }
    public string edmsId { get; set; }
    public string PDFSize { get; set; }

    public VendorDocument()
    {
    }

    public VendorDocument(VendorDocumentId VendorDocumentId) : base()
    {
        VendorDocumentId = VendorDocumentId;
    }

    private VendorDocument(string name, string description, byte[] content, int version, string edmsId, string pdfSize) : base()
    {
        VendorDocumentId.Create(Guid.NewGuid());
        Name = name;
        Description = description;
        Content = content;
        Version = version;
        edmsId = edmsId;
        PDFSize = pdfSize;
    }


    public static VendorDocument Create(string name, string description, byte[] content, int version, string edmsId, string pdfSize)
    {
        return new(name, description, content, version, edmsId, pdfSize);
    }
}
