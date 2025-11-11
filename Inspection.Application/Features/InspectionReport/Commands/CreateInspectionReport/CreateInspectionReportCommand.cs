using Inspection.Application.DTOs.InspectionReport;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Commands.CreateInspectionReport;

public class CreateInspectionReportCommand:IRequest<Inspection.Domain.Entities.InspectionReport.InspectionReport>
{
    public InspectionReportDto inspectionReport { get; set; }
}
