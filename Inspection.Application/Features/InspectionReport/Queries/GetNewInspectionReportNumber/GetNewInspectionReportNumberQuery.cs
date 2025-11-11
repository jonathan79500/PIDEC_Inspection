using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Queries.GetNewInspectionReportNumber
{
    public class GetNewInspectionReportNumberQuery:IRequest<string>
    {
        public string InspectionNotificationNumber { get; set; }
        public int ReportType { get; set; }
    }
}
