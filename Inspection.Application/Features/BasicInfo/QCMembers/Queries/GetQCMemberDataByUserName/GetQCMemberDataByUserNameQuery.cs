using Inspection.Application.DTOs.Common;
using MediatR;

namespace Inspection.Application.Features.BasicInfo.QCMembers.Queries.GetQCMemberDataByUserName
{
    public class GetQCMemberDataByUserNameQuery:IRequest<BasicQCMemberInfoDto>
    {
        public string UserName { get; set; }
        public int DomainId { get; set; }
    }
}
