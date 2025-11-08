using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[PrimaryKey("InspectionSubjectId", "InspectionNotificationId")]
[Index("InspectionNotificationId", Name = "IX_InspectionNotificationSubjects_InspectionNotificationId")]
public partial class InspectionNotificationSubject
{
    [Key]
    public Guid InspectionNotificationId { get; set; }

    [Key]
    public Guid InspectionSubjectId { get; set; }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("InspectionNotificationId")]
    [InverseProperty("InspectionNotificationSubjects")]
    public virtual InspectionNotification InspectionNotification { get; set; } = null!;

    [ForeignKey("InspectionSubjectId")]
    [InverseProperty("InspectionNotificationSubjects")]
    public virtual InspectionSubject InspectionSubject { get; set; } = null!;
}
