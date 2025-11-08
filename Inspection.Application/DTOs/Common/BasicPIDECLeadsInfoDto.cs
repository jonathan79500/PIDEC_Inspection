using Inspection.Application.DTOs.InspectionRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class BasicPIDECLeadsInfoDto
    {

        public BasicPIDECLeadsInfoDto() { }
        public string? Name { get; set; }// = string.Empty;
        public string? UserName { get; set; } //= string.Empty;
        public int? DomainId { get; set; }
        public List<ProjectDto>? Projects { get; set; }
        [JsonIgnore]
        [JsonProperty("basicPIDECLeadsOrders")]
        public List<OrderDto>? Orders { get; set; }
        public List<OrderDto>? EDMSOrders { get; set; }
        public List<InspectionTypeDto>? InspectionTypes { get; set; }
        public List<InspectionSubjectDto>? InspectionSubjects { get; set; } = [];
        public List<OrderInspectionSubjectDto>? OrderInspectionSubjects { get; set; }
        public List<CirculationDto>? Participated_Circulations { get; set; }
        public List<OrderContactDto>? OrderContacts { get; set; }
        public List<InspectionNotificationDto>? ReceivedInspectionNotifications { get; set; }

    }
}
