using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Commands.GiveAccessToReEditTask
{
    public class GiveAccessToReEditTaskCommandHandler : IRequestHandler<GiveAccessToReEditTaskCommand, List<CirculationDto>>
    {
        private IInspectionNotificationRepository _inspectionNotificationRepository;
        public GiveAccessToReEditTaskCommandHandler(IInspectionNotificationRepository inspectionNotificationRepository)
        {
            _inspectionNotificationRepository = inspectionNotificationRepository;
        }
        public Task<List<CirculationDto>> Handle(GiveAccessToReEditTaskCommand request, CancellationToken cancellationToken)
        {
           Circulation cntCirculation = request.Contributor.Adapt<Circulation>();
           Circulation[] asnCirculations = request.AssigneeCirculations.Adapt<Circulation[]>();
            var result =  _inspectionNotificationRepository.GiveEditRightToSpecificRole(cntCirculation, asnCirculations);
            List<CirculationDto> addedCirculationDtos= result.Result.Adapt<List<CirculationDto>>();
           return Task.FromResult(addedCirculationDtos);
        }
    }
}
