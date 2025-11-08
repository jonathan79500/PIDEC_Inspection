using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Index("ProjectId", Name = "IX_Orders_ProjectId")]
[Index("VendorContactPersonContactId", Name = "IX_Orders_VendorContactPersonContactId")]
public partial class Order
{
    [Key]
    public Guid OrderId { get; set; }

    public Guid ProjectId { get; set; }

    [StringLength(50)]
    public string No { get; set; } = null!;

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int Version { get; set; }

    [StringLength(100)]
    public string EdmsId { get; set; } = null!;

    [StringLength(100)]
    public string EquipmentTagNo { get; set; } = null!;

    [Column("ITP_NO")]
    [StringLength(50)]
    public string ItpNo { get; set; } = null!;

    public Guid VendorContactPersonContactId { get; set; }

    public bool ShowForOwner { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<InspectionNotification> InspectionNotifications { get; set; } = new List<InspectionNotification>();

    [InverseProperty("Order")]
    public virtual ICollection<OrderQccontactPerson> OrderQccontactPeople { get; set; } = new List<OrderQccontactPerson>();

    [ForeignKey("ProjectId")]
    [InverseProperty("Orders")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("VendorContactPersonContactId")]
    [InverseProperty("Orders")]
    public virtual VendorContactPerson VendorContactPersonContact { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<VendorDocument> VendorDocuments { get; set; } = new List<VendorDocument>();

    [ForeignKey("OrderId")]
    [InverseProperty("Orders")]
    public virtual ICollection<InspectionSubject> InspectionSubjects { get; set; } = new List<InspectionSubject>();
}
