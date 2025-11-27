using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Persistence.Repositories
{
    public class FileRepository: IFileService
    {

      
        private readonly IBasicInformationRepository _basicInfoRepository;
        public FileRepository(IBasicInformationRepository basicInfoRepository)
        {
            _basicInfoRepository = basicInfoRepository;
        }

        public Task<FileDto> DownloadFileById(Guid attachmentId)
        {
            Attachment attachment =   _basicInfoRepository.GetAttachmentById(attachmentId).Result;
            if (attachment == null) return null;

            return Task.FromResult(new FileDto
            {
                FileName = attachment.Name!,                
                Content = attachment.Content,
                Description = attachment.Description==null? "":attachment.Description,
                Id = attachment.AttachmentId.Value,
                ContentType = attachment.ContentType
            });
        }

        public async Task PostFileAsync(Attachment attachment)
        {
            //try
            //{
            //    var _attachment = Attachment.Create(attachment.Name, attachment.Description, attachment.Content,attachment.Version,attachment.edmsId,attachment.PDFSize);
               
                                
            //    var result = _dbcontext.Attachments.Add(_attachment);
            //    await _dbcontext.SaveChangesAsync();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public Task PostMultiFileAsync(List<Attachment> attachments)
        {
            throw new NotImplementedException();
        }

       
    }
}
