using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.PIDECLeads
{
    public class GetPIDECLeadsDataByUserNameQuery:IRequest<BasicPIDECLeadsInfoDto>
    {
        public string UserName { get; set; }
        public int DomainId { get; set; }
    }
}
