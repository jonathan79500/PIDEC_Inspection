using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

public partial class SupplierContactPerson
{
    [Key]
    public Guid ContactId { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("SupplierContactPerson")]
    public virtual Contact Contact { get; set; } = null!;
}
