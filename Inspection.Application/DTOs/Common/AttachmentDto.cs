using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.Common
{
    public class AttachmentDto
    {
        public string Id { get; set; }  
        public string Name { get;  set; }
        public string Version { get;  set; }
        public string Description { get;  set; }
        public IFormFile Content { get;  set; }
    }
}
