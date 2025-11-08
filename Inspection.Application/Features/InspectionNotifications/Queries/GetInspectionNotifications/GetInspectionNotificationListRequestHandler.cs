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
  

    public GetInspectionNotificationListRequestHandler(IInspectionNotificationRepository inspectionRequestRepository)
    {
        _inspectionRequestRepository = inspectionRequestRepository;       
    }

    public async Task<List<InspectionNotificationDto>> Handle(GetInspectionNotificationListRequest request, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionRequestRepository.GetAll();
        try
        {            
            var k = inspections[0].Adapt<InspectionNotificationDto>();
        }
        catch(System.Exception d)
        {
            string str= d.Message;
        }      
               

        try
        {
            return inspections.Adapt<List<InspectionNotificationDto>>();            
        }
        catch(System.Exception exp)
        {
            return new List<InspectionNotificationDto>();
        }
    }
}
