using Inspection.Application.DTOs.InspectionReport;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Features.InspectionNotifications.Commands.SendInspectionNotification;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById;
using Inspection.Application.Features.InspectionNotifications.Queries.GetNewNotificationNumber;
using Inspection.Application.Features.InspectionReport.Commands.CreateInspectionReport;
using Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportById;
using Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportsByNotificationId;
using Inspection.Application.Features.InspectionReport.Queries.GetNewInspectionReportNumber;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspection.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionReportController : ControllerBase
    {
        private readonly ISender _mediator;
        private IWebHostEnvironment webHostEnvironment;
        public InspectionReportController(IMediator mediator, IWebHostEnvironment _webHostEnvironment)
        {
            _mediator = mediator;
            this.webHostEnvironment = _webHostEnvironment;
        }

        [HttpPost]
        [Route("GetInspectionReportById")]
        public async Task<ActionResult<InspectionReportDto>> GetInspectionReportById([FromBody] InspectionReportIdQuery inspectionReportId)
        {
            try
            {
                var inspectionReport = await _mediator.Send(new GetInspectionReportByIdQuery { InspectionReportId = inspectionReportId.InspectionReportId});
                return Ok(inspectionReport);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpPost]
        [Route("GetInspectionReportByInspectionNotificationId")]
        public async Task<ActionResult<InspectionReportDto>> GetInspectionReportByInspectionNotificationId([FromBody] InspectionNotificationIdQuery inspectionNotificationId)
        {
            try
            {
                var inspectionReport = await _mediator.Send(new GetInspectionReportsByNotificationIdQuery { InspectionNotificationId = inspectionNotificationId.InspectionNotificationId});
                return Ok(inspectionReport);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpPost]
        [Route("SaveNewInspectionReport")]
        public async Task<ActionResult> Post([FromBody] InspectionReportDto inspectionReport)
        {
            var command = new CreateInspectionReportCommand { inspectionReport = inspectionReport };

            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (Exception ex)
            {

                return Ok("jdljqwl");
            }

        }
        [HttpPost]
        [Route("GetNewInspectionReportNumber")]
        public async Task<ActionResult<queryResult>> GetNewInspectionReportNumber([FromBody] queryRNParams param)
        {
            var command = new GetNewInspectionReportNumberQuery { InspectionNotificationNumber = param.inspectionNotificationNumber,ReportType= param.reportType};
            try
            {
                var response = await _mediator.Send(command);
                queryResult q = new queryResult();
                q.newInspectionReportNumber = response;
                return Ok(q);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        public class InspectionNotificationIdQuery
        {

            public string InspectionNotificationId { get; set; }
        }

        public class InspectionReportIdQuery
        {

            public string InspectionReportId { get; set; }
        }

        public class queryRNParams
        {
            public string inspectionNotificationNumber { get; set; }   
            public int reportType { get; set; }
        }

        public class queryResult
        {
            public string newInspectionReportNumber { get; set; }
        }
    }
}
