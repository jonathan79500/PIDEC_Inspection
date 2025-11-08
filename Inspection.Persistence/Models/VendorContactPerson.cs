using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Index("VendorId", Name = "IX_VendorContactPersons_VendorId")]
public partial class VendorContactPerson
{
    [Key]
    public Guid ContactId { get; set; }

    public Guid VendorId { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("VendorContactPerson")]
    public virtual Contact Contact { get; set; } = null!;

    [InverseProperty("VendorContactPersonContact")]
    public virtual ICollection<InspectionNotification> InspectionNotifications { get; set; } = new List<InspectionNotification>();

    [InverseProperty("VendorContactPersonContact")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("VendorId")]
    [InverseProperty("VendorContactPeople")]
    public virtual Vendor Vendor { get; set; } = null!;
}
