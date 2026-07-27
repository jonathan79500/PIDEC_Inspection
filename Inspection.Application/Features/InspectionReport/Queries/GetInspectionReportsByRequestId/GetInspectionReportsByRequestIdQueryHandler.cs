using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.InspectionReport;
using Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportsByNotificationId;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportsByRequestId
{
    public class GetInspectionReportsByRequestIdQueryHandler : IRequestHandler<GetInspectionReportsByRequestIdQuery, List<InspectionReportDto>>
    {
        IInspectionReportRepository _inspectionReportRepository;
        public GetInspectionReportsByRequestIdQueryHandler(IInspectionReportRepository inspectionReportRepository)
        {
            _inspectionReportRepository = inspectionReportRepository;    
        }
        public Task<List<InspectionReportDto>> Handle(GetInspectionReportsByRequestIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Guid requestId = new Guid(request.RequestId);
                var inspectionReports = _inspectionReportRepository.GetInspectionReportsByRequestId(requestId, request.UserId).Result;

                List<InspectionReportDto> inspectionReportDtos = null;
                if (inspectionReports != null)
                    inspectionReportDtos = inspectionReports.Adapt<List<InspectionReportDto>>();

                return Task.FromResult(inspectionReportDtos);
            }
            catch (Exception exp)
            {
                return null;
            }
        }
    }
}
