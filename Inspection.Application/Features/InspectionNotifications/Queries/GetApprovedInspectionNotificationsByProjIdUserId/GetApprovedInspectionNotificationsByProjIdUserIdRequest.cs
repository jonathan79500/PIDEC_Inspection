using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetApprovedInspectionNotificationsByProjIdUserId
{
    public class GetApprovedInspectionNotificationsByProjIdUserIdRequest:IRequest<List<InspectionNotificationDataSummaryDto>>
    {
        public string ProjectId { get; set; }
        public string UserId { get; set; }
    }
}
