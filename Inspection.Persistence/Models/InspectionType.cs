using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

public partial class InspectionType
{
    [Key]
    public Guid InspectionTypeId { get; set; }

    public string Name { get; set; } = null!;

    [InverseProperty("InspectionType")]
    public virtual ICollection<InspectionNotificationType> InspectionNotificationTypes { get; set; } = new List<InspectionNotificationType>();
}
