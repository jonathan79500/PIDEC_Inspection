using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

public partial class Project
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(400)]
    public string Title { get; set; } = null!;

    [StringLength(500)]
    public string Description { get; set; } = null!;

    [StringLength(200)]
    public string Number { get; set; } = null!;

    [Column("FSId")]
    [StringLength(100)]
    public string Fsid { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [StringLength(100)]
    public string ModifiedBy { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<OrderOwnerContactPerson> OrderOwnerContactPeople { get; set; } = new List<OrderOwnerContactPerson>();

    [InverseProperty("Project")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectMccontactPerson> ProjectMccontactPeople { get; set; } = new List<ProjectMccontactPerson>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectVendor> ProjectVendors { get; set; } = new List<ProjectVendor>();

    [ForeignKey("ProjectId")]
    [InverseProperty("Projects")]
    public virtual ICollection<Mc> Mcs { get; set; } = new List<Mc>();

    [ForeignKey("ProjectId")]
    [InverseProperty("Projects")]
    public virtual ICollection<Owner> Owners { get; set; } = new List<Owner>();
}
