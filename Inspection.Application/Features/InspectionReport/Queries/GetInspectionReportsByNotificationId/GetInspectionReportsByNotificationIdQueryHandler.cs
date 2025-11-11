using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.InspectionReport;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportsByNotificationId
{
    public class GetInspectionReportsByNotificationIdQueryHandler : IRequestHandler<GetInspectionReportsByNotificationIdQuery, List<InspectionReportDto>>
    {
        IInspectionReportRepository _inspectionReportRepository;
        public GetInspectionReportsByNotificationIdQueryHandler(IInspectionReportRepository inspectionReportRepository)
        {
            _inspectionReportRepository = inspectionReportRepository;
        }
        public Task<List<InspectionReportDto>> Handle(GetInspectionReportsByNotificationIdQuery request, CancellationToken cancellationToken)
        {
            InspectionNotificationId inspectionNotificationId = InspectionNotificationId.Create(new Guid(request.InspectionNotificationId));
            List<Domain.Entities.InspectionReport.InspectionReport> inspectionReports= _inspectionReportRepository.GetInspectionReportsByNotificationId(inspectionNotificationId).Result.ToList();
            List<InspectionReportDto> inspectionReportDtos= inspectionReports.Adapt<List<InspectionReportDto>>();

            return Task.FromResult(inspectionReportDtos);
        }
    }
}
