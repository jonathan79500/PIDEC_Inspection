using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Commands.GiveAccessToReEditTask
{
    public class GiveAccessToReEditTaskCommand:IRequest<List<CirculationDto>>
    {
        public CirculationDto Contributor { get; set; }
        public CirculationDto[] AssigneeCirculations { get; set; }
    }
}
