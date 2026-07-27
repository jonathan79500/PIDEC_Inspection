using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionsReadyForReportByUserName
{
    public class GetInspectionsReadyForReportByUserNameQuery:IRequest<List<InspectionNotificationDataSummaryDto>>
    {
        public string userId { get; set; }
        public string projectId { get; set; }
    }
}
