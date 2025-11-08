using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

[Index("LocationId", Name = "IX_InspectionNotifications_LocationId")]
[Index("MccontactPersonContactId", Name = "IX_InspectionNotifications_MCContactPersonContactId")]
[Index("OrderId", Name = "IX_InspectionNotifications_OrderId")]
[Index("OwnerContactPersonContactId", Name = "IX_InspectionNotifications_OwnerContactPersonContactId")]
[Index("VendorContactPersonContactId", Name = "IX_InspectionNotifications_VendorContactPersonContactId")]
public partial class InspectionNotification
{
    [Key]
    public Guid InspectionNotificationId { get; set; }

    [StringLength(200)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string NotificationNumber { get; set; } = null!;

    public DateTime? ContractShippingDueDate { get; set; }

    public DateTime? ActualShippingDueDate { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? RequestInspectionDate { get; set; }

    public DateTime? ProposedDate { get; set; }

    public Guid? VendorContactPersonContactId { get; set; }

    public Guid? OwnerContactPersonContactId { get; set; }

    [Column("MCContactPersonContactId")]
    public Guid? MccontactPersonContactId { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? LocationId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = null!;

    [StringLength(100)]
    public string ModifiedBy { get; set; } = null!;

    [InverseProperty("InspectionNotification")]
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    [InverseProperty("InspectionNotification")]
    public virtual ICollection<InspectionNotificationContact> InspectionNotificationContacts { get; set; } = new List<InspectionNotificationContact>();

    [InverseProperty("InspectionNotification")]
    public virtual ICollection<InspectionNotificationSubject> InspectionNotificationSubjects { get; set; } = new List<InspectionNotificationSubject>();

    [InverseProperty("InspectionNotification")]
    public virtual ICollection<InspectionNotificationType> InspectionNotificationTypes { get; set; } = new List<InspectionNotificationType>();

    [ForeignKey("LocationId")]
    [InverseProperty("InspectionNotifications")]
    public virtual Address? Location { get; set; }

    [ForeignKey("MccontactPersonContactId")]
    [InverseProperty("InspectionNotifications")]
    public virtual MccontactPerson? MccontactPersonContact { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("InspectionNotifications")]
    public virtual Order? Order { get; set; }

    [ForeignKey("OwnerContactPersonContactId")]
    [InverseProperty("InspectionNotifications")]
    public virtual OwnerContactPerson? OwnerContactPersonContact { get; set; }

    [ForeignKey("VendorContactPersonContactId")]
    [InverseProperty("InspectionNotifications")]
    public virtual VendorContactPerson? VendorContactPersonContact { get; set; }
}
