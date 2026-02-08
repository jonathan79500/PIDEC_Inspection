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
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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


        [HttpPost("SaveNewInspectionReport")]
        [Consumes("multipart/form-data")]
        // [Produces("application/json")]
        public async Task<ActionResult> SaveNewInspectionReport([FromForm] InspectionReportUploadDto inspectionReportUpload)
        // IFormCollection inspectionUploadDto)
        {
            try
            {

                List<AttachmentDto> attachmentDtos = new List<AttachmentDto>();
                AttachmentDto attachmentDto;
                if (inspectionReportUpload.Attachments != null)
                {
                    for (int i = 0; i < inspectionReportUpload.Attachments.Count; i++)
                    {
                        attachmentDto = new AttachmentDto();
                        attachmentDto.Content = inspectionReportUpload.Attachments[i];
                        attachmentDto.ContentType = inspectionReportUpload.Attachments[i].ContentType;
                        if (inspectionReportUpload.AttachmentIds[i] != null && inspectionReportUpload.AttachmentIds[i] != "")
                            attachmentDto.Id = new Guid(inspectionReportUpload.AttachmentIds[i]);
                        else
                            attachmentDto.Id = Guid.NewGuid();
                        attachmentDto.Name = inspectionReportUpload.AttachmentNames[i];
                        attachmentDto.Version = inspectionReportUpload.AttachmentVersions[i];
                        attachmentDto.Description = inspectionReportUpload.AttachmentDescriptions[i];
                        attachmentDto.DocNo = inspectionReportUpload.AttachmentDocNos[i];
                        attachmentDto.Status = inspectionReportUpload.AttachmentStatuses[i];

                        var path = Path.Combine(webHostEnvironment.ContentRootPath, "Attachments", FileHelper.generateFileName(attachmentDto.Name));




                        using var stream = new FileStream(path, FileMode.Create);
                        await attachmentDto.Content.CopyToAsync(stream);

                        attachmentDtos.Add(attachmentDto);
                        // Here you can persist metadata + file path to DB
                    }
                }
                var command = new CreateInspectionReportCommand
                {
                    inspectionReport = JsonConvert.DeserializeObject<InspectionReportDto>(inspectionReportUpload.inspectionReport),                   
                    CurrentCirculationDto = (inspectionReportUpload.currentCirculation!=null ? JsonConvert.DeserializeObject<CirculationDto>(inspectionReportUpload.currentCirculation): null),
                    AttachmentDtos = attachmentDtos
                };
                
                var response = await _mediator.Send(command);
                return Ok(new
                {
                    InspectionReport = response.Item1,
                    CurrentCirculation = response.Item2
                });
                //return Ok(response);
            }
            catch (Exception exp)
            {
                return Ok();
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

        public class CirculationDataDto
        {
            public string circulation { get; set; }
            public List<string>? AttachmentIds { get; set; }
            public List<string>? AttachmentNames { get; set; }
            public List<string>? AttachmentVersions { get; set; }
            public List<string>? AttachmentDescriptions { get; set; }
            [ValidateNever]
            public List<IFormFile> Attachments { get; set; }
        }
        public class InspectionReportUploadDto
        {
            public string? inspectionReport { get; set; }
            public string? project { get; set; }
            public string? currentCirculation { get; set; }
            public List<string>? AttachmentIds { get; set; }
            public List<string>? AttachmentNames { get; set; }
            public List<string>? AttachmentVersions { get; set; }
            public List<string>? AttachmentDescriptions { get; set; }
            public List<string>? AttachmentDocNos { get; set; }
            public List<string>? AttachmentStatuses { get; set; }
            public List<IFormFile>? Attachments { get; set; }
            //[ModelBinder(BinderType = typeof(AttachmentListBinder))]
            //public List<AttachmentDto> Attachments { get; set; }

        }
    }
}
