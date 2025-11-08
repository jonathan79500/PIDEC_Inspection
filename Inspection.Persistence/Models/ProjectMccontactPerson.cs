using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[PrimaryKey("ProjectId", "MccontactPersonId")]
[Table("ProjectMCContactPersons")]
[Index("MccontactPersonId", Name = "IX_ProjectMCContactPersons_MCContactPersonId")]
public partial class ProjectMccontactPerson
{
    [Key]
    public Guid ProjectId { get; set; }

    [Key]
    [Column("MCContactPersonId")]
    public Guid MccontactPersonId { get; set; }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("MccontactPersonId")]
    [InverseProperty("ProjectMccontactPeople")]
    public virtual MccontactPerson MccontactPerson { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectMccontactPeople")]
    public virtual Project Project { get; set; } = null!;
}
