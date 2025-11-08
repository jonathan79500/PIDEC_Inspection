using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.Common;
using Mapster;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Commands.CompleteCirculation
{
    public class CompleteCirculationCommandHandler:IRequestHandler<CompleteCirculationCommand,Circulation>
    {
        private IInspectionNotificationRepository _notificationRepository;
        public CompleteCirculationCommandHandler(IInspectionNotificationRepository inspectionNotificationRepository) {
        _notificationRepository = inspectionNotificationRepository;
        }

        public Task<Circulation> Handle(CompleteCirculationCommand request, CancellationToken cancellationToken)
        {
            Circulation circulation = new Circulation();
            circulation = request.CirculationDto.Adapt<Circulation>();
            var result = _notificationRepository.CompleteCirculation(circulation);
            Circulation resultCirc = result.Result;
            return Task.FromResult(circulation);// Task.FromResult(circulation);
        }
    }

   
}
