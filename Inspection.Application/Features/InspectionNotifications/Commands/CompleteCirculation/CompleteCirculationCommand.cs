using Inspection.Application.DTOs;
using Inspection.Domain.Entities.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Commands.CompleteCirculation
{
    public class CompleteCirculationCommand:IRequest<Circulation>
    {
        public CompleteCirculationCommand() { }
        public CirculationDto CirculationDto { get; set; }
    }
}
