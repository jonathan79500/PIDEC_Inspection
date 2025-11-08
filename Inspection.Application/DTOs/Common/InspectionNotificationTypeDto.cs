using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectionNotificationTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SequenceNo { get; set; }
        public Guid InspectionNotificationId { get; set; }
    }
}
