using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectionNotificationSubjectDto
    {
        public Guid? Id { get; set; }=Guid.Empty;
        public string? Name { get; set; }
        public int SequenceNo { get; set; }
        public Guid InspectionNotificationId { get; set; }
        public string ITP_Row_No { get; set; } = string.Empty;
    }
}
