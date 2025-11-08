using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Table("QCContactPersons")]
[Index("QcmemberId", Name = "IX_QCContactPersons_QCMemberId")]
public partial class QccontactPerson
{
    [Key]
    public Guid ContactId { get; set; }

    [Column("QCMemberId")]
    public Guid QcmemberId { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("QccontactPerson")]
    public virtual Contact Contact { get; set; } = null!;

    [InverseProperty("QccontactPerson")]
    public virtual ICollection<OrderQccontactPerson> OrderQccontactPeople { get; set; } = new List<OrderQccontactPerson>();

    [ForeignKey("QcmemberId")]
    [InverseProperty("QccontactPeople")]
    public virtual Qcmember Qcmember { get; set; } = null!;
}
