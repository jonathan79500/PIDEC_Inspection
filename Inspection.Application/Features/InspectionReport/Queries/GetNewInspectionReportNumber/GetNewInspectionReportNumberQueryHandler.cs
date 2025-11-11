using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.InspectionReport;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Queries.GetNewInspectionReportNumber
{
    public class GetNewInspectionReportNumberQueryHandler : IRequestHandler<GetNewInspectionReportNumberQuery, string>
    {

        private IInspectionReportRepository _inspectionReportRepository;
        public GetNewInspectionReportNumberQueryHandler(IInspectionReportRepository inspectionReportRepository)
        {
            _inspectionReportRepository = inspectionReportRepository;
        }

        Task<string> IRequestHandler<GetNewInspectionReportNumberQuery, string>.Handle(GetNewInspectionReportNumberQuery request, CancellationToken cancellationToken)
        {
            InspectionReportNumber inspectionReportNumber = _inspectionReportRepository.GenerateNewInspectionReportNumber(request.InspectionNotificationNumber, request.ReportType).Result;
            return Task.FromResult(inspectionReportNumber.ToString());
        }
    }
}
