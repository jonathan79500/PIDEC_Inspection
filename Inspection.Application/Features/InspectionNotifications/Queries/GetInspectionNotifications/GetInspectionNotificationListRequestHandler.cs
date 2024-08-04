using AutoMapper;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Contracts.Persistence;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mapster;

namespace Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotifications;

public class GetInspectionNotificationListRequestHandler : IRequestHandler<GetInspectionNotificationListRequest, List<InspectionNotificationDto>>
{
    private readonly IInspectionNotificationRepository _inspectionRequestRepository;
    private readonly IMapper _mapper;

    public GetInspectionNotificationListRequestHandler(IInspectionNotificationRepository inspectionRequestRepository, IMapper mapper)
    {
        _inspectionRequestRepository = inspectionRequestRepository;
        _mapper = mapper;
    }

    public async Task<List<InspectionNotificationDto>> Handle(GetInspectionNotificationListRequest request, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionRequestRepository.GetAll();
       //var k=  _mapper.Map<List<InspectionNotificationDto>>(inspections);
                
               

        try
        {
            return _mapper.Map<List<InspectionNotificationDto>>(inspections);
        }
        catch(System.Exception exp)
        {
            return new List<InspectionNotificationDto>();
        }
    }
}
