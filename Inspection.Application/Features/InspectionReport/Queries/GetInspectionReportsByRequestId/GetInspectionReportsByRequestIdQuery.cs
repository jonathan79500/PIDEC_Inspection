using Inspection.Application.DTOs.InspectionReport;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportsByRequestId
{
    public class GetInspectionReportsByRequestIdQuery: IRequest<List<InspectionReportDto>>
    {
        public string RequestId { get; set; }
        public string UserId { get; set; }
    }
}
