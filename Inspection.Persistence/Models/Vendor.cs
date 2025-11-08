using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Index("AddressId", Name = "IX_Vendors_AddressId")]
public partial class Vendor
{
    [Key]
    public Guid Id { get; set; }

    public Guid AddressId { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Fax { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int DomainId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("AddressId")]
    [InverseProperty("Vendors")]
    public virtual Address Address { get; set; } = null!;

    [InverseProperty("Vendor")]
    public virtual ICollection<ProjectVendor> ProjectVendors { get; set; } = new List<ProjectVendor>();

    [InverseProperty("Vendor")]
    public virtual ICollection<VendorContactPerson> VendorContactPeople { get; set; } = new List<VendorContactPerson>();
}
