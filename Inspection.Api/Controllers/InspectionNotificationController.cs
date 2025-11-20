using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.InspectionRequest;
using Inspection.Application.DTOs.User;
using Inspection.Application.Features.InspectionNotifications.Commands.CompleteCirculation;
using Inspection.Application.Features.InspectionNotifications.Commands.CreateInspectionNotification;
using Inspection.Application.Features.InspectionNotifications.Commands.SendInspectionNotification;
using Inspection.Application.Features.InspectionNotifications.Queries.GetDraftedNotificationsByUserName;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationById;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationFlow;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotifications;
using Inspection.Application.Features.InspectionNotifications.Queries.GetInspectionNotificationsByProjUsrIds;
using Inspection.Application.Features.InspectionNotifications.Queries.GetNewNotificationNumber;
using Inspection.Application.Features.InspectionNotifications.Queries.GetNotStartedNotificationsByUserName;
using Inspection.Application.Features.InspectionNotifications.Queries.GetSentNotificationsByUserName;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Infrastructure.Helpers;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;


namespace Inspection.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class InspectionNotificationController : ControllerBase
{
    private readonly ISender _mediator;
    private IWebHostEnvironment webHostEnvironment;

    public InspectionNotificationController(IMediator mediator, IWebHostEnvironment _webHostEnvironment)
    {
        _mediator = mediator;
        this.webHostEnvironment = _webHostEnvironment;
    }
    // GET: api/<InspectionController>
    [HttpGet]    
    [Route("GetAll")]
    //[AllowAnonymous]
    public async Task<ActionResult<List<InspectionNotificationDto>>> Get()
    {
        var inspectionNotifications = await _mediator.Send(new GetInspectionNotificationListRequest());

        return Ok(inspectionNotifications);
    }

    // GET api/<InspectionController>/5 [Route("{id}/GetById")]
    //[HttpGet("{id}")]
    //[HttpGet]
    //[Route("GetById/{id}")]
    //public async Task<ActionResult<InspectionNotificationDto>> GetById([FromRoute]Guid id)
    //{
    //    var inspectionNotification = await _mediator.Send(new GetInspectionNotificationDetailRequest { Id = new Domain.Entities.InspectionNotification.ValueObjects.InspectionNotificationId(id)});
    //    return Ok(inspectionNotification);
    //}


    //[HttpPost]
    //[Route("GetById")]
    //public async Task<ActionResult<InspectionNotificationDto>> GetById([FromBody] InspectionId id)
    //{
    //    var inspectionNotification = await _mediator.Send(new GetInspectionNotificationDetailRequest { Id = new Domain.Entities.InspectionNotification.ValueObjects.InspectionNotificationId(new Guid(id.Id)) });
    //    return Ok(inspectionNotification);
    //}

    [HttpPost]
    [Route("SaveNew")]
    public async Task<ActionResult> Post([FromBody] CreateInspectionNotificationDto inspectionNotification)
    {
        var command = new SendInspectionNotificationCommand { CreateInspectionNotificationDto = inspectionNotification };

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
    [Route("GetNewNotificationNumber")]
    public async Task<ActionResult<queryResult>> GetNewNotificationNumber([FromBody]queryNNParams param)
    {
        var command = new GetNewNotificationNumberQuery { ProjectNumber= param.projectnumber, OrderNumber= param.ordernumber };
        try
        { 
            var response = await _mediator.Send(command);
            queryResult q= new queryResult();
            q.newNotificationNumber = response;
            return Ok(q);
        }
        catch (Exception ex) {
            return Ok(ex.Message);
        }
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
    [Route("GetInspectionByInspectionNotificationId")]
    public async Task<ActionResult<InspectionNotificationDto>> GetInspectionByInspectionNotificationId([FromBody] InspectionNotificationIdQuery inspectionNotificationId)
    {
        try
        {
            var inspectionNotification = await _mediator.Send(new GetInspectionNotificationByIdRequest { InspectionNotificationId = inspectionNotificationId.InspectionNotificationId });
            return Ok(inspectionNotification);
        }
        catch (Exception exp)
        {
            return BadRequest(exp.Message);
        }
    }

    [HttpPost]
    [Route("GetInspectionByNumber")]
    public async Task<ActionResult<InspectionNotificationFlowDto>> GetInspectionByNumber([FromBody] InspectionNotificationNumber notificationNumber)
    {
        try
        {
            var inspectionNotificationflow = await _mediator.Send(new GetInspectionNotificationFlow { NotificationNumber = notificationNumber.notificationNumber });
            return Ok(inspectionNotificationflow);
        }
        catch (Exception exp)
        {
            return BadRequest(exp.Message);
        }
    }


    [HttpPost]
    [Route("GetReceivedInspectionsByProjectId_UserId")]
    public async Task<ActionResult<List<InspectionNotificationDto>>> GetReceivedInspectionsByProjectId_UserId([FromBody] InspectionListRequestDto request)
    {
        try
        {
            var receivedInspectionNotifications = await _mediator.Send(new GetInspectionNotificationsByProjUsrIdsRequest {ProjectId=request.ProjectId,UserId=request.UserId });
            return Ok(receivedInspectionNotifications);
        }
        catch (Exception exp)
        {
            return BadRequest(exp.Message);
        }
    }

    [HttpPost]
    [Route("GetDraftedInspectionsByUserName")]
    public async Task<ActionResult<List<InspectionNotificationDataSummaryDto>>> GetDraftedInspectionsByUserName([FromBody] InspectionsQuery request)
    {
        try
        {
            var receivedInspectionNotifications = await _mediator.Send(new GetDraftedNotificationsByUserNameRequest { userName = request.userName});
            return Ok(receivedInspectionNotifications);
        }
        catch (Exception exp)
        {
            return BadRequest(exp.Message);
        }
    }

    [HttpPost]
    [Route("GetSentInspectionsByUserName")]
    public async Task<ActionResult<List<InspectionNotificationDataSummaryDto>>> GetSentInspectionsByUserName([FromBody] InspectionsQuery request)
    {
        try
        {
            var sentInspectionNotifications = await _mediator.Send(new GetSentNotificationsByUserNameRequest { userName = request.userName });
            return Ok(sentInspectionNotifications);
        }
        catch (Exception exp)
        {
            return BadRequest(exp.Message);
        }
    }

    [HttpPost]
    [Route("GetNotStartedInspectionsByUserName")]
    public async Task<ActionResult<List<InspectionNotificationDataSummaryDto>>> GetNotStartedInspectionsByUserName([FromBody] InspectionsQuery request)
    {
        try
        {
            var notStartedInspectionNotifications = await _mediator.Send(new GetNotStartedNotificationsByUserNameRequest { userName = request.userName });
            return Ok(notStartedInspectionNotifications);
        }
        catch (Exception exp)
        {
            return BadRequest(exp.Message);
        }
    }

    //[HttpPost("PostSingleFile")]
    //public async Task<ActionResult> PostSingleFile([FromForm] FileUploadModel fileDetails)
    //{
    //    if (fileDetails == null)
    //    {
    //        return BadRequest();
    //    }

    //    try
    //    {
    //        await _uploadService.PostFileAsync(fileDetails.FileDetails, fileDetails.FileType);
    //        return Ok();
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //}
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [HttpPost("DraftInspectionNotification")]
    public async Task<ActionResult<InspectionNotificationDto>> DraftInspectionNotification(IFormFile[] attachments,IFormCollection inspectionNotification)
    {
        try
        {
            //Debug.Write("File Name is: "+attachment.FileName);
            //Debug.Write("File Type is: " + attachment.ContentType);
            foreach (var attachment in attachments)
            {
                    var path = Path.Combine(webHostEnvironment.ContentRootPath, "Attachments", FileHelper.generateFileName(attachment.FileName));


                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    attachment.CopyTo(fileStream);
                }

            }
            var inspection = JsonConvert.DeserializeObject<CreateInspectionNotificationDto>(inspectionNotification["Inspection"]!);
            var command = new CreateInspectionNotificationCommand { createInspectionNotificationDto = inspection! };

            var response = await _mediator.Send(command);

            InspectionNotificationDto dd= response.Adapt<InspectionNotificationDto>();
            return Ok(dd);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("CompleteTask")]
    public async Task<ActionResult> CompleteTask(IFormFile[] attachments, IFormCollection inspectionNotification)//([FromBody] CirculationDto circulationDto)
    {
        try
        {
            var currentCirculation = JsonConvert.DeserializeObject<CirculationDto>(inspectionNotification["circulation"]!);
            var command = new CompleteCirculationCommand { CirculationDto = currentCirculation };
            var response = _mediator.Send(command);
            return Ok(response);
        }
        catch
        {
          
            return BadRequest("task ended with error");
        }
    }


    [HttpPost("SendInspectionNotification")]
    [Consumes("multipart/form-data")]
   // [Produces("application/json")]
    public async Task<ActionResult> SendInspectionNotification([FromForm] InspectionUploadDto inspectionUpload) 
   // IFormCollection inspectionUploadDto)
    {
        try
        {

            List<AttachmentDto> attachmentDtos = new List<AttachmentDto>();
            AttachmentDto attachmentDto;
            for (int i = 0; i < inspectionUpload.Attachments.Count; i++)
            {
                attachmentDto = new AttachmentDto();
                attachmentDto.Content = inspectionUpload.Attachments[i];
                attachmentDto.Id = inspectionUpload.AttachmentIds[i];
                attachmentDto.Name = inspectionUpload.AttachmentNames[i];
                attachmentDto.Version = inspectionUpload.AttachmentVersions[i];
                attachmentDto.Description = inspectionUpload.AttachmentDescriptions[i];
                var path = Path.Combine(webHostEnvironment.ContentRootPath, "Attachments", FileHelper.generateFileName(attachmentDto.Name));




                using var stream = new FileStream(path, FileMode.Create);
                await attachmentDto.Content.CopyToAsync(stream);

                attachmentDtos.Add(attachmentDto);
                // Here you can persist metadata + file path to DB
            }

            var command = new SendInspectionNotificationCommand
            {
                CreateInspectionNotificationDto = JsonConvert.DeserializeObject<CreateInspectionNotificationDto>(inspectionUpload.inspection),
                ProjectDto = JsonConvert.DeserializeObject<ProjectDto>(inspectionUpload.project),
                CurrentCirculationDto = JsonConvert.DeserializeObject<CirculationDto>(inspectionUpload.currentCirculation),
                AttachmentDtos = attachmentDtos//inspectionUpload.Attachments
            };
            //var command = new SendInspectionNotificationCommand
            //{
            //    CreateInspectionNotificationDto = null,
            //    ProjectDto = null,
            //    CurrentCirculationDto = null,
            //    AttachmentDtos = null
            //};

            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception exp)
        {
            return Ok();
        }
    }

    //[Consumes("multipart/form-data")]
    //[Produces("application/json")]
    //[HttpPost("SendInspectionNotification")]
    //public async Task<ActionResult> SendInspectionNotification([FromForm] CreateInspectionNotificationDto inspection,
    //    [FromForm] ProjectDto project,
    //    [FromForm] CirculationDto currentCirculation,
    //    [FromForm] List<AttachmentDto> attachments)//([FromForm] List<AttachmentDto> attachments, IFormCollection inspectionNotification)
    //{
    //    try
    //        {
    //        var dtoAttachment = new AttachmentDto();
          
    //        //Debug.Write("File Name is: "+attachment.FileName);
    //        //Debug.Write("File Type is: " + attachment.ContentType);
    //        AttachmentDto[] attachmentDtos = new AttachmentDto[10];
    //    //foreach (var attachment in attachments)
    //    //{
    //    //    var path = Path.Combine(webHostEnvironment.ContentRootPath, "Attachments", FileHelper.generateFileName(attachment.Name));

            
    //    //        if (dtoAttachment.Content != null)
    //    //        {
    //    //            dtoAttachment.Id = Guid.NewGuid();
    //    //            dtoAttachment.Version = attachment.Version;
    //    //            dtoAttachment.Name = attachment.Name;
    //    //            dtoAttachment.Description = attachment.Description;
    //    //            using var ms = new MemoryStream();
    //    //            await dtoAttachment.Content.CopyToAsync(ms);
    //    //           attachmentDtos.Append(attachment);
    //    //        }

    //    //}
    //    //var inspection = JsonConvert.DeserializeObject<CreateInspectionNotificationDto>(inspectionNotification["Inspection"]!);
    //    //    var currentCirculation = JsonConvert.DeserializeObject<CirculationDto>(inspectionNotification["CurrentCirculation"]!);
    //    //    var project = JsonConvert.DeserializeObject<ProjectDto>(inspectionNotification["Project"]!);
    //    //    var command = new SendInspectionNotificationCommand { CreateInspectionNotificationDto = inspection!,CurrentCirculationDto = currentCirculation! ,ProjectDto = project,AttachmentDtos= attachmentDtos};

    //   // var response = await _mediator.Send(command);

    //    return Ok(true);
    //    }
    //    catch (Exception ex)
    //    {
    //    return BadRequest();
    //    }
    //}
}
public class InspectionUploadDto
{
    public string inspection { get; set; }
     public string project { get; set; }
      public string currentCirculation { get; set; }
    public List<string> AttachmentIds { get; set; }
    public List<string> AttachmentNames { get; set; }
    public List<string> AttachmentVersions { get; set; }
    public List<string> AttachmentDescriptions { get; set; }
    public List<IFormFile> Attachments { get; set; }
    //[ModelBinder(BinderType = typeof(AttachmentListBinder))]
    //public List<AttachmentDto> Attachments { get; set; }

}

public class queryResult {
   public string newNotificationNumber { get; set; }
}
public class queryNNParams
{
    public string projectnumber { get; set; }
    public string ordernumber { get;set; }
}
public class InspectionId { 

    public string Id { get; set; }
}

public class InspectionNotificationNumber
{

    public string notificationNumber { get; set; }
}

public class InspectionNotificationIdQuery
{

    public string InspectionNotificationId { get; set; }
}

public class InspectionListRequestDto
{
    public string ProjectId { get; set; }
    public string UserId { get; set; }
}

public class InspectionsQuery { 
    public string userName {  get; set; }
}