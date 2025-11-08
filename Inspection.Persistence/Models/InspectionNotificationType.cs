using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[PrimaryKey("InspectionNotificationId", "InspectionTypeId")]
[Index("InspectionTypeId", Name = "IX_InspectionNotificationTypes_InspectionTypeId")]
public partial class InspectionNotificationType
{
    [Key]
    public Guid InspectionNotificationId { get; set; }

    [Key]
    public Guid InspectionTypeId { get; set; }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("InspectionNotificationId")]
    [InverseProperty("InspectionNotificationTypes")]
    public virtual InspectionNotification InspectionNotification { get; set; } = null!;

    [ForeignKey("InspectionTypeId")]
    [InverseProperty("InspectionNotificationTypes")]
    public virtual InspectionType InspectionType { get; set; } = null!;
}
