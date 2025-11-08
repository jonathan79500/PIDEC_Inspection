using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[PrimaryKey("ProjectId", "OwnerContactPersonId")]
[Index("OwnerContactPersonId", Name = "IX_OrderOwnerContactPersons_OwnerContactPersonId")]
public partial class OrderOwnerContactPerson
{
    [Key]
    public Guid ProjectId { get; set; }

    [Key]
    public Guid OwnerContactPersonId { get; set; }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("OwnerContactPersonId")]
    [InverseProperty("OrderOwnerContactPeople")]
    public virtual OwnerContactPerson OwnerContactPerson { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("OrderOwnerContactPeople")]
    public virtual Project Project { get; set; } = null!;
}
