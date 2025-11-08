using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Index("OrderId", Name = "IX_VendorDocuments_OrderId")]
public partial class VendorDocument
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "image")]
    public byte[] Content { get; set; } = null!;

    public Guid OrderId { get; set; }

    public int Version { get; set; }

    [Column("edmsId")]
    [StringLength(12)]
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

    [ForeignKey("OrderId")]
    [InverseProperty("VendorDocuments")]
    public virtual Order Order { get; set; } = null!;
}
