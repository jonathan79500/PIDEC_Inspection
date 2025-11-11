using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.InspectionReport;
using Inspection.Domain.Entities.ValueObjects;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportById
{
    public class GetInspectionReportByIdQueryHandler : IRequestHandler<GetInspectionReportByIdQuery, InspectionReportDto>
    {
        IInspectionReportRepository _inspectionReportRepository;

        public GetInspectionReportByIdQueryHandler(IInspectionReportRepository inspectionReportRepository)
        {
            _inspectionReportRepository = inspectionReportRepository;
        }

        public Task<InspectionReportDto> Handle(GetInspectionReportByIdQuery request, CancellationToken cancellationToken)
        {
            InspectionReportId inspectionReportId = InspectionReportId.Create(new Guid(request.InspectionReportId));
            Domain.Entities.InspectionReport.InspectionReport inspectionReport = _inspectionReportRepository.GetInspectionReportById(inspectionReportId).Result;
            var inspectionReportDto =  inspectionReport.Adapt<InspectionReportDto>();
            return Task.FromResult(inspectionReportDto);
        }
    }
}
