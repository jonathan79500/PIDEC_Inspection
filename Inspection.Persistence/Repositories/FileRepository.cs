using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Microsoft.VisualBasic.FileIO;

namespace Inspection.Persistence.Repositories
{
    public class FileRepository: IFileService
    {

        private InspectionDbContext _dbcontext;
        public FileRepository(InspectionDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public Task DownloadFileById(int attachmentId)
        {
            throw new NotImplementedException();
        }

        public async Task PostFileAsync(Attachment attachment)
        {
            try
            {
                var _attachment = Attachment.Create(attachment.Name, attachment.Description, attachment.Content,attachment.Version,attachment.edmsId,attachment.PDFSize);
               
                                
                var result = _dbcontext.Attachments.Add(_attachment);
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task PostMultiFileAsync(List<Attachment> attachments)
        {
            throw new NotImplementedException();
        }
    }
}
