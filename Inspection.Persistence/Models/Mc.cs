using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Table("MC")]
public partial class Mc
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

    [InverseProperty("Mc")]
    public virtual ICollection<MccontactPerson> MccontactPeople { get; set; } = new List<MccontactPerson>();

    [ForeignKey("Mcid")]
    [InverseProperty("Mcs")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
