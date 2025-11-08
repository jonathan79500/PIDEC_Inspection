using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetNewNotificationNumber
{
    public class GetNewNotificationNumberQuery:IRequest<string>
    {
        public string ProjectNumber { get; set; }
        public string OrderNumber { get; set; }
    }
}
