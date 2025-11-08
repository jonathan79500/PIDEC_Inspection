using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Index("OwnerId", Name = "IX_OwnerContactPersons_OwnerId")]
public partial class OwnerContactPerson
{
    [Key]
    public Guid ContactId { get; set; }

    public Guid OwnerId { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("OwnerContactPerson")]
    public virtual Contact Contact { get; set; } = null!;

    [InverseProperty("OwnerContactPersonContact")]
    public virtual ICollection<InspectionNotification> InspectionNotifications { get; set; } = new List<InspectionNotification>();

    [InverseProperty("OwnerContactPerson")]
    public virtual ICollection<OrderOwnerContactPerson> OrderOwnerContactPeople { get; set; } = new List<OrderOwnerContactPerson>();

    [ForeignKey("OwnerId")]
    [InverseProperty("OwnerContactPeople")]
    public virtual Owner Owner { get; set; } = null!;
}
