using Inspection.Application.DTOs.InspectionReport;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportById
{
    public class GetInspectionReportByIdQuery:IRequest<InspectionReportDto>
    {
        public string InspectionReportId { get; set; }
    }
}
