using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionReport;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.Features.InspectionNotifications.Commands.SendInspectionNotification;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById;
using Inspection.Application.Features.InspectionNotifications.Queries.GetNewNotificationNumber;
using Inspection.Application.Features.InspectionReport.Commands.CreateInspectionReport;
using Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportById;
using Inspection.Application.Features.InspectionReport.Queries.GetInspectionReportsByNotificationId;
using Inspection.Application.Features.InspectionReport.Queries.GetNewInspectionReportNumber;
using Inspection.Infrastructure.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        // GET api/<TestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        [HttpPost]
        [Route("GetInspectionReportById")]
        public async Task<ActionResult<InspectionReportDto>> GetInspectionReportById([FromBody] InspectionReportIdQuery inspectionReportId)
        {
            try
            {
                var inspectionReport = await _mediator.Send(new GetInspectionReportByIdQuery { InspectionReportId = inspectionReportId.InspectionReportId });
                return Ok(inspectionReport);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpPost]
        [Route("GetInspectionReportsByInspectionNotificationId")]
        public async Task<ActionResult<List<InspectionReportDto>>> GetInspectionReportsByInspectionNotificationId([FromBody] NotificationIdQuery inspectionNotificationId)
        {
            try
            {
                var inspectionReports = await _mediator.Send(new GetInspectionReportsByNotificationIdQuery { InspectionNotificationId = inspectionNotificationId.InspectionNotificationId });
                return Ok(inspectionReports);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }



        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [HttpPost("SaveNewInspectionReport")]
        public async Task<ActionResult> SaveNewInspectionReport(IFormFile[] reportAttachments, IFormCollection inspectionReportFull)
        {
            try
            {
                //Debug.Write("File Name is: "+attachment.FileName);
                //Debug.Write("File Type is: " + attachment.ContentType);
                foreach (var attachment in reportAttachments)
                {
                    var path = Path.Combine(webHostEnvironment.ContentRootPath, "reportAttachments", FileHelper.generateFileName(attachment.FileName));


                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        attachment.CopyTo(fileStream);
                    }

                }
                var inspectionReport = JsonConvert.DeserializeObject<InspectionReportDto>(inspectionReportFull["InspectionReport"]!);
                //var currentCirculation = JsonConvert.DeserializeObject<CirculationDto>(inspectionNotification["CurrentCirculation"]!);
                //var project = JsonConvert.DeserializeObject<ProjectDto>(inspectionNotification["Project"]!);
                var command = new CreateInspectionReportCommand { inspectionReport = inspectionReport };

                var response = await _mediator.Send(command);

                return Ok(reportAttachments);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("GetNewInspectionReportNumber")]
        public async Task<ActionResult<queryReportResult>> GetNewInspectionReportNumber([FromBody] queryRNParams param)
        {
            var command = new GetNewInspectionReportNumberQuery { InspectionNotificationNumber = param.inspectionNotificationNumber, ReportType = param.reportType };
            try
            {
                var response = await _mediator.Send(command);
                queryReportResult q = new queryReportResult();
                q.newInspectionReportNumber = response;
                return Ok(q);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        public class NotificationIdQuery
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

        public class queryReportResult
        {
            public string newInspectionReportNumber { get; set; }
        }
    }
}
