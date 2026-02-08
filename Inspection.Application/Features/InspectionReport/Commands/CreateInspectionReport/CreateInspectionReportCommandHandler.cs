using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.InspectionReport;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Domain.Entities.Common;
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
    public class CreateInspectionReportCommandHandler : IRequestHandler<CreateInspectionReportCommand, (InspectionReportDto,CirculationDto)>
    {
        private IInspectionReportRepository inspectionReportRepository { get; set; }
        private IInspectionNotificationRepository inspectionNotificationRepository { get; set; }

        public CreateInspectionReportCommandHandler(IInspectionReportRepository _inspectionReportRepository,IInspectionNotificationRepository _inspectionNotificationRepository)
        {
            inspectionReportRepository = _inspectionReportRepository;
            inspectionNotificationRepository = _inspectionNotificationRepository;
        }
        public async Task<(InspectionReportDto, CirculationDto)> Handle(CreateInspectionReportCommand request, CancellationToken cancellationToken)
        {
            Inspection.Domain.Entities.InspectionReport.InspectionReport resultinspectionReport = null;

            Inspection.Domain.Entities.InspectionReport.InspectionReport inspectionReport = request.inspectionReport.Adapt<Inspection.Domain.Entities.InspectionReport.InspectionReport>();

            var attachments = request.AttachmentDtos.Adapt<List<Attachment>>();
            Circulation currentCirculation = null;

            string ConnectedNotificationNumber = inspectionNotificationRepository.GetInspectionNotificationById(inspectionReport.RelatedInspectionNotificationId).Id.ToString();

            int ReportType = inspectionReport.ReportType.GetHashCode();

            InspectionReportNumber newInspectionReportNumber = inspectionReportRepository.GenerateNewInspectionReportNumber(ConnectedNotificationNumber, ReportType).Result;

            Inspection.Domain.Entities.InspectionReport.InspectionReport newInspectionReport = null;
            try
            {

                if (inspectionReport.InspectionReportId != null && !inspectionReport.InspectionReportId.Value.Equals(Guid.Empty))
                {
                    newInspectionReport = await inspectionReportRepository.UpdateInspectionReport(inspectionReport);
                }
                else
                    (resultinspectionReport,currentCirculation) = await inspectionReportRepository.CreateNewInspectionReport(inspectionReport,newInspectionReportNumber,attachments);

                //var simulatedVersion = SimulateInpectionDataForNextUsers(notificationForSend, project);
                //var result = CreateNextUsersCirculations(notificationForSend, project);

                await inspectionReportRepository.UpdateInspectionReportNumber(InspectionReportNumber.CreateFromString(newInspectionReport!.InspectionReportNumber.ToString()!));
            }
            catch (Exception exp)
            {

            }
            InspectionReportDto inspectionReportDto = resultinspectionReport.Adapt<InspectionReportDto>();
            CirculationDto currentCirculationDto = currentCirculation.Adapt<CirculationDto>();
            return await Task.FromResult((inspectionReportDto, currentCirculationDto));
            
        }
    }
}
