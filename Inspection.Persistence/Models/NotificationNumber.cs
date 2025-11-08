using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Models;

public partial class NotificationNumber
{
    [Key]
    public Guid Id { get; set; }

    public string ProjectNumber { get; set; } = null!;

    [StringLength(3)]
    public string InfoPart { get; set; } = null!;

    public string OrderNumber { get; set; } = null!;

    public int Version { get; set; }

    public int Revision { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;
}
