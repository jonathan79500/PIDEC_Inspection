using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[PrimaryKey("ProjectId", "VendorId")]
[Index("VendorId", Name = "IX_ProjectVendors_VendorId")]
public partial class ProjectVendor
{
    [Key]
    public Guid ProjectId { get; set; }

    [Key]
    public Guid VendorId { get; set; }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectVendors")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("VendorId")]
    [InverseProperty("ProjectVendors")]
    public virtual Vendor Vendor { get; set; } = null!;
}
