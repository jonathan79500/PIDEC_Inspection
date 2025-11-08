using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

public partial class InspectionSubject
{
    [Key]
    public Guid InspectionSubjectId { get; set; }

    public string Name { get; set; } = null!;

    [InverseProperty("InspectionSubject")]
    public virtual ICollection<InspectionNotificationSubject> InspectionNotificationSubjects { get; set; } = new List<InspectionNotificationSubject>();

    [ForeignKey("InspectionSubjectId")]
    [InverseProperty("InspectionSubjects")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
