using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Index("AddressId", Name = "IX_Contacts_AddressId")]
public partial class Contact
{
    [Key]
    public Guid ContactId { get; set; }

    public string Name { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int DomainId { get; set; }

    /// <summary>
    /// undefined=0;Owner=1;Vendor=2;MC=3; PIDEC=4;Supplier =5; SubSupplier=6
    /// </summary>
    public int ContactType { get; set; }

    public Guid AddressId { get; set; }

    public string Phone { get; set; } = null!;

    public string Fax { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HomePage { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string Attention { get; set; } = null!;

    public string Cc { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("AddressId")]
    [InverseProperty("Contacts")]
    public virtual Address Address { get; set; } = null!;

    [InverseProperty("Contact")]
    public virtual ICollection<InspectionNotificationContact> InspectionNotificationContacts { get; set; } = new List<InspectionNotificationContact>();

    [InverseProperty("Contact")]
    public virtual MccontactPerson? MccontactPerson { get; set; }

    [InverseProperty("Contact")]
    public virtual OwnerContactPerson? OwnerContactPerson { get; set; }

    [InverseProperty("Contact")]
    public virtual QccontactPerson? QccontactPerson { get; set; }

    [InverseProperty("Contact")]
    public virtual SupplierContactPerson? SupplierContactPerson { get; set; }

    [InverseProperty("Contact")]
    public virtual VendorContactPerson? VendorContactPerson { get; set; }
}
