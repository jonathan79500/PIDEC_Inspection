using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.Common
{
    public class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Position { get; set; } = string.Empty;
        public string? Phone { get; set; }= string.Empty;
        public string? Email { get; set; } = string.Empty;
        public List<PersonAttachment>? Attachments { get; set; }

        public List<InspectionReportPerson> InspectionReports { get; set; }
    }
}
