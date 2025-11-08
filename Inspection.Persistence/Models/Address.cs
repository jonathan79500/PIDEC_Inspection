using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

public partial class Address
{
    [Key]
    public Guid Id { get; set; }

    public string Country { get; set; } = null!;

    public string? State { get; set; }

    public string City { get; set; } = null!;

    public string? StreetLine1 { get; set; }

    public string? StreetLine2 { get; set; }

    public string? ZipCode { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [StringLength(100)]
    public string ModifiedBy { get; set; } = null!;

    [InverseProperty("Address")]
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    [InverseProperty("Location")]
    public virtual ICollection<InspectionNotification> InspectionNotifications { get; set; } = new List<InspectionNotification>();

    [InverseProperty("Address")]
    public virtual ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
}
