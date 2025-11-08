using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[PrimaryKey("Id", "InspectionNotificationId")]
[Index("InspectionNotificationId", Name = "IX_Attachments_InspectionNotificationId")]
public partial class Attachment
{
    [Key]
    public Guid Id { get; set; }

    [Key]
    public Guid InspectionNotificationId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; }

    [Column(TypeName = "image")]
    public byte[] Content { get; set; } = null!;

    public int Version { get; set; }

    [Column("edmsId")]
    public string EdmsId { get; set; } = null!;

    [Column("PDFSize")]
    [StringLength(20)]
    public string Pdfsize { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [StringLength(100)]
    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("InspectionNotificationId")]
    [InverseProperty("Attachments")]
    public virtual InspectionNotification InspectionNotification { get; set; } = null!;
}
