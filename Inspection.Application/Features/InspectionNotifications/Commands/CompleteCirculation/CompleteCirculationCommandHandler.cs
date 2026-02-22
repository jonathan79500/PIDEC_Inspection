using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;
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
    public class CompleteCirculationCommandHandler:IRequestHandler<CompleteCirculationCommand,CirculationDto>
    {
        private IInspectionNotificationRepository _notificationRepository;
        public CompleteCirculationCommandHandler(IInspectionNotificationRepository inspectionNotificationRepository) {
        _notificationRepository = inspectionNotificationRepository;
        }

        public Task<CirculationDto> Handle(CompleteCirculationCommand request, CancellationToken cancellationToken)
        {
            Circulation circulation = new Circulation();
            circulation = request.CirculationDto.Adapt<Circulation>();
            var attachments = request.AttachmentDtos.Adapt<List<Attachment>>();

            var result = _notificationRepository.CompleteCirculation(circulation,null, attachments);
            Circulation resultCirc = result.Result.Item2;
            return Task.FromResult(resultCirc.Adapt<CirculationDto>());// Task.FromResult(circulation);
        }
    }

   
}
