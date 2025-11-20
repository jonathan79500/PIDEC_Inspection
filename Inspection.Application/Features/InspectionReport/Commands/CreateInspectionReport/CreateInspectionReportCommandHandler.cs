using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.Projects;
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
        private IInspectionNotificationRepository inspectionNotificationRepository { get; set; }

        public CreateInspectionReportCommandHandler(IInspectionReportRepository _inspectionReportRepository,IInspectionNotificationRepository _inspectionNotificationRepository)
        {
            inspectionReportRepository = _inspectionReportRepository;
            inspectionNotificationRepository = _inspectionNotificationRepository;
        }
        public async Task<Domain.Entities.InspectionReport.InspectionReport> Handle(CreateInspectionReportCommand request, CancellationToken cancellationToken)
        {
            Inspection.Domain.Entities.InspectionReport.InspectionReport inspectionReport = request.inspectionReport.Adapt<Inspection.Domain.Entities.InspectionReport.InspectionReport>();

            string ConnectedNotificationNumber = inspectionNotificationRepository.GetInspectionNotificationById(inspectionReport.RelatedInspectionNotificationId).Id.ToString();

            int ReportType = inspectionReport.ReportType.GetHashCode();

            string newInspectionReportNumber = inspectionReportRepository.GenerateNewInspectionReportNumber(ConnectedNotificationNumber, ReportType).Result.ToString();

            Inspection.Domain.Entities.InspectionReport.InspectionReport newInspectionReport = null;
            try
            {

                if (inspectionReport.InspectionReportId != null && !inspectionReport.InspectionReportId.Value.Equals(Guid.Empty))
                {
                    newInspectionReport = await inspectionReportRepository.UpdateInspectionReport(inspectionReport);
                }
                else
                    newInspectionReport = await inspectionReportRepository.CreateNewInspectionReport(inspectionReport);

                //var simulatedVersion = SimulateInpectionDataForNextUsers(notificationForSend, project);
                //var result = CreateNextUsersCirculations(notificationForSend, project);

                await inspectionReportRepository.UpdateInspectionReportNumber(InspectionReportNumber.CreateFromString(newInspectionReport!.InspectionReportNumber.ToString()!));
            }
            catch (Exception exp)
            {

            }
            
            return newInspectionReport;
        }
    }
}
