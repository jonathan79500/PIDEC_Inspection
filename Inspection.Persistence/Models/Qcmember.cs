using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Table("QCMember")]
public partial class Qcmember
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int DomainId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [InverseProperty("Qcmember")]
    public virtual ICollection<QccontactPerson> QccontactPeople { get; set; } = new List<QccontactPerson>();
}
