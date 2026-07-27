using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationHistoryByRequestId
{
    public class GetInspectionNotificationHistoryByRequestIdRequest:IRequest<List<InspectionNotificationHistoryDto>>
    {
        public Guid RequestId { get; set; }
    }
}
