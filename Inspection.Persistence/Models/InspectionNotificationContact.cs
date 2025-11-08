using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[PrimaryKey("InspectionNotificationId", "ContactId")]
[Index("ContactId", Name = "IX_InspectionNotificationContacts_ContactId")]
public partial class InspectionNotificationContact
{
    [Key]
    public Guid InspectionNotificationId { get; set; }

    [Key]
    public Guid ContactId { get; set; }

    public int ContactType { get; set; }

    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    [ForeignKey("ContactId")]
    [InverseProperty("InspectionNotificationContacts")]
    public virtual Contact Contact { get; set; } = null!;

    [ForeignKey("InspectionNotificationId")]
    [InverseProperty("InspectionNotificationContacts")]
    public virtual InspectionNotification InspectionNotification { get; set; } = null!;
}
