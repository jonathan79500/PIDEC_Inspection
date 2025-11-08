
using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.DTOs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Features.BasicInfo.Vendors.Queries.BasicVendorInfos
{
    public class GetBasicVendorInfoQuery  :IRequest<BasicVendorInfoDto>
    {
      public ContactDto LogginedVendor { get; set; }

    }
}
