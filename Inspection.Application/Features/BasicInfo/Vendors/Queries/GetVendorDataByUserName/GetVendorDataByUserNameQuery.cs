using Inspection.Application.DTOs.Common;
using MediatR;

namespace Inspection.Application.Features.BasicInfo.Vendors.Queries.GetVendorDataByUserName;

public class GetVendorDataByUserNameQuery : IRequest<VendorDto>
{
  public  string vendorUserName { get; set; }
public int domainId { get; set; }

}
