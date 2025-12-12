using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Inspection.Domain.Entities.InspectionNotification.Entities;

public sealed class Attachment:BaseDomainEntity<AttachmentId>
{

    public AttachmentId AttachmentId { get;  set; }
    

    public Attachment(AttachmentId attachmentId):base()
    {
        AttachmentId = attachmentId;
    }
    
    private Attachment(string name, string description, byte[] content, int version, string edmsId, string pdfSize,string contentType) : base()
    {
        if(AttachmentId == null || AttachmentId.Value == null || AttachmentId.GetHashCode()==Guid.Empty.GetHashCode())
            AttachmentId = AttachmentId.Create(Guid.NewGuid());
        Name = name;
        Description = description;
        Content = content;
        Version = version;
        edmsId = edmsId;
        PDFSize = pdfSize;
        ContentType = contentType;

    }


    public Attachment()
    {
    }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public byte[]? Content { get; set; }
    public int Version { get;  set; }
    public string? edmsId { get;  set; }
    public string? PDFSize { get;  set; }
    // For client consumption
    public string? DownloadUrl { get; set; }
    public string? ContentType { get; set; }
    // OR
    public string? Base64Content { get; set; }

    public List<CirculationAttachments> Circulations { get; set; }

    public static Attachment Create(string name, string description, byte[] content, int version, string edmsId, string pdfSize,string contentType)
    {
        
        return new(name, description, content, version, edmsId, pdfSize,contentType);
    }

    //public static Attachment CreateFromObject(Attachment attachment)
    //{     

    //    return new(name, description, content, version, edmsId, pdfSize, contentType);
    //}
}
