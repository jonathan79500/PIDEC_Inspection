using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.InspectionReport;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.InspectionReport.Commands.CreateInspectionReport
{
    public class CreateInspectionReportCommandHandler : IRequestHandler<CreateInspectionReportCommand, Inspection.Domain.Entities.InspectionReport.InspectionReport>
    {
        private IInspectionReportRepository inspectionReportRepository { get; set; }

        public CreateInspectionReportCommandHandler(IInspectionReportRepository _inspectionReportRepository)
        {
            inspectionReportRepository = _inspectionReportRepository;
        }
        public async Task<Domain.Entities.InspectionReport.InspectionReport> Handle(CreateInspectionReportCommand request, CancellationToken cancellationToken)
        {
            Inspection.Domain.Entities.InspectionReport.InspectionReport inspectionReport = request.inspectionReport.Adapt<Inspection.Domain.Entities.InspectionReport.InspectionReport>();
            var result = await inspectionReportRepository.CreateNewInspectionReport(inspectionReport);
            return result;
        }
    }
}
