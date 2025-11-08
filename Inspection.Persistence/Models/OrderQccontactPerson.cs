using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[PrimaryKey("OrderId", "QccontactPersonId")]
[Table("OrderQCContactPersons")]
[Index("QccontactPersonId", Name = "IX_OrderQCContactPersons_QCContactPersonId")]
public partial class OrderQccontactPerson
{
    [Key]
    public Guid OrderId { get; set; }

    [Key]
    [Column("QCContactPersonId")]
    public Guid QccontactPersonId { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderQccontactPeople")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("QccontactPersonId")]
    [InverseProperty("OrderQccontactPeople")]
    public virtual QccontactPerson QccontactPerson { get; set; } = null!;
}
