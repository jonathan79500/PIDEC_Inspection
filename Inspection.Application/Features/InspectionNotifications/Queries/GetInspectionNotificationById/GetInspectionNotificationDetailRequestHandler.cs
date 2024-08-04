using AutoMapper;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Contracts.Persistence;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById
{
    public class GetInspectionNotificationDetailRequestHandler : IRequestHandler<GetInspectionNotificationDetailRequest, InspectionNotificationDto>
    {
        private readonly IInspectionNotificationRepository _inspectionRequestRepository;
        private readonly IMapper _mapper;

        public GetInspectionNotificationDetailRequestHandler(IInspectionNotificationRepository inspectionRequestRepository, IMapper mapper)
        {
            _mapper = mapper;
            _inspectionRequestRepository = inspectionRequestRepository;
        }

        public async Task<InspectionNotificationDto> Handle(GetInspectionNotificationDetailRequest request, CancellationToken cancellationToken)
        {
            var inspectionRequest = await _inspectionRequestRepository.Get(request.Id);
            return _mapper.Map<InspectionNotificationDto>(inspectionRequest);

        }
    }
}
