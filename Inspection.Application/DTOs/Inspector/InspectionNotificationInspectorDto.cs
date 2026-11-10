using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Inspector;

    public class InspectionNotificationInspectorDto
    {

        public Guid InspectionInspectorId { get; set; }

        public InspectionNotificationId InspectionNotificationId { get; set; }
        

        public Guid InspectorId { get; set; }
        public InspectorDto Inspector { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; }

        public Guid? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? Remarks { get; set; }

    }

