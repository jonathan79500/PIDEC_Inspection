using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Commands.CompleteCirculation
{
    public class CompleteCirculationCommand:IRequest<CirculationDto>
    {
        public CompleteCirculationCommand() { }
        public CirculationDto CirculationDto { get; set; }
        public List<AttachmentDto> AttachmentDtos { get; set; }
    }
}
