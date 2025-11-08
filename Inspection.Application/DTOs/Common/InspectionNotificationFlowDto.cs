using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class InspectionNotificationFlowDto
    {
        
            public List<InspectionNotificationDto> InspectionNotifications { get; set; }
            public List<CirculationDto> Circulations { get; set; }
            public List<InspectionNotificationContactDto> Contacts { get; set; }
            public List<InspectionNotificationSubjectDto> Subjects { get; set; }
            public List<InspectionNotificationTypeDto> Types { get; set; }
        
    }
}
