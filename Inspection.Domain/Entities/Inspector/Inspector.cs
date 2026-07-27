using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Inspector
{
    public class Inspector
    {
        
        public Guid InspectorId { get; set; }
        public Guid InspectionCompanyContactId { get; set; }
        public Contact InspectionCompanyContact { get; set; } = null!;
        public bool IsActive { get; set; }

        public string? Name { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Position { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;

        public List<InspectionNotificationInspector>? InspectionNotifications { get; set; }
    }
}
