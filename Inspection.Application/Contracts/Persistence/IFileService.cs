using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;

namespace Inspection.Application.Contracts.Persistence
{
    public interface IFileService
    {
        public Task PostFileAsync(Attachment attachment);
        public Task PostMultiFileAsync(List<Attachment> attachments);
        public Task<FileDto> DownloadFileById(Guid attachmentId);
    }
}
