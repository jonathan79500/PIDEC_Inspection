using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionReport;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Commands.CreateInspectionReport;

public class CreateInspectionReportCommand:IRequest<(InspectionReportDto, CirculationDto)>
{
    public InspectionReportDto? inspectionReport { get; set; }
    public CirculationDto? CurrentCirculationDto { get; set; }
    public List<AttachmentDto>? AttachmentDtos { get; set; }
}
