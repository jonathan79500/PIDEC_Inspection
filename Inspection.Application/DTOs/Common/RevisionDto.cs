using Inspection.Application.DTOs.InspectionReport;
using Inspection.Application.DTOs.InspectionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    
        public class RevisionDto
        {
        public RevisionDto()
        {
            
        }
        public Guid? RevisionId { get; set; } = Guid.Empty;
        public Guid? RequestId { get; set; } = Guid.Empty;
        public int? VersionNumber { get; set; }
        public int? RevisionNumber { get; set; } = 0;
        public DateTime? SubmittedDate { get; set; } = DateTime.Now;

            // Instead of full Contact entity, expose minimal info
            public Guid SubmittedById { get; set; }
            public ContactDto SubmittedBy { get; set; }

            public string? Comment { get; set; } = string.Empty;
            public string? Status { get; set; } = "Pending";
            public bool? IsFinal { get; set; } = false;

            // Circulations summarized
            public List<CirculationDto>? Circulations { get; set; } = new List<CirculationDto>();

            // Subtype references (IDs or summaries)
            //public List<InspectionNotificationDto>? InspectionNotifications { get; set; }
            public List<InspectionReportDto>? InspectionReports { get; set; }
        

    }
}
