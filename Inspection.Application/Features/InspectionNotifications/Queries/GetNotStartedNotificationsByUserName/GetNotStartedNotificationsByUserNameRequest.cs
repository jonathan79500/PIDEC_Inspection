using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetNotStartedNotificationsByUserName
{
    public class GetNotStartedNotificationsByUserNameRequest : IRequest<List<InspectionNotificationDataSummaryDto>>
    {
        public string userName { get; set; }
    }
}
