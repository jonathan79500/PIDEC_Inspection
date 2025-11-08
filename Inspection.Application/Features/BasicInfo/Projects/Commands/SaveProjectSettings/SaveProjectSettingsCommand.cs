using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Projects.Commands.SaveProjectSettings
{
    public class SaveProjectSettingsCommand:IRequest<bool>
    {
        public ProjectDto projectDto;       
    }
}
