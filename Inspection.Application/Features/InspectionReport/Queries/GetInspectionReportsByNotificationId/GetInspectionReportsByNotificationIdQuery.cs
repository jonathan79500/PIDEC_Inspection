using Inspection.Application.DTOs.InspectionReport;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportsByNotificationId
{
    public class GetInspectionReportsByNotificationIdQuery:IRequest<List<InspectionReportDto>>
    {
        public string InspectionNotificationId { get; set; }
    }
}
