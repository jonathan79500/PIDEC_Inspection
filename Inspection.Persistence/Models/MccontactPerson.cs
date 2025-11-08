using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Table("MCContactPersons")]
[Index("Mcid", Name = "IX_MCContactPersons_MCId")]
public partial class MccontactPerson
{
    [Key]
    public Guid ContactId { get; set; }

    [Column("MCId")]
    public Guid Mcid { get; set; }

    [ForeignKey("ContactId")]
    [InverseProperty("MccontactPerson")]
    public virtual Contact Contact { get; set; } = null!;

    [InverseProperty("MccontactPersonContact")]
    public virtual ICollection<InspectionNotification> InspectionNotifications { get; set; } = new List<InspectionNotification>();

    [ForeignKey("Mcid")]
    [InverseProperty("MccontactPeople")]
    public virtual Mc Mc { get; set; } = null!;

    [InverseProperty("MccontactPerson")]
    public virtual ICollection<ProjectMccontactPerson> ProjectMccontactPeople { get; set; } = new List<ProjectMccontactPerson>();
}
