using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities;
using Inspection.Domain.Entities.Common;
using MediatR;

namespace Inspection.Application.Features.BasicInfo.Vendors.Queries.GetVendorDataByUserName;

public class GetVendorDataByUserNameQuery : IRequest<BasicVendorInfoDto>
{
  public  string vendorUserName { get; set; }
public int domainId { get; set; }

}
