using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Inspection.Application.Contracts.Infrastructure;

namespace Inspection.Application.Features.BasicInfo.QCMembers.Queries.GetQCMemberDataByUserName
{
    public class GetQCMemberDataByUserNameQueryHandler : IRequestHandler<GetQCMemberDataByUserNameQuery,BasicQCMemberInfoDto>
    {
        private readonly IEDMSService _eDMSService;
        private IBasicInformationRepository basicInformationRepository;

        public GetQCMemberDataByUserNameQueryHandler(IEDMSService eDMSService, IBasicInformationRepository _basicInformationRepository)
        {
            _eDMSService = eDMSService;
            basicInformationRepository = _basicInformationRepository;
        }

        Task<BasicQCMemberInfoDto> IRequestHandler<GetQCMemberDataByUserNameQuery, BasicQCMemberInfoDto>.Handle(GetQCMemberDataByUserNameQuery request, CancellationToken cancellationToken)
        {
            //var EDMSresult = _eDMSService.GetEdmsOrders_ByProId("69");
            Task<BasicQCMemberInfo> basicQCMemberInfoTask = basicInformationRepository.GetBasicQCMemberInfo(request.UserName, request.DomainId);
            try
            {
                //var inspectionSubject = new InspectionSubject();
                //var insDto = inspectionSubject.Adapt<InspectionSubjectDto>();
                
                //Project prj=new Project();
                //var prjDto = prj.Adapt<ProjectDto>();

                //var vendorDocument = new VendorDocument(VendorDocumentId.CreateUnique());

                //// Convert VendorDocument to VendorDocumentDto
                //var vendorDocumentDto = vendorDocument.Adapt<VendorDocumentDto>();

                //Owner owner = new Owner();
                //var ownerDto = owner.Adapt<OwnerDto>();

                //Contact contact = new Contact();
                //var contactDto = contact.Adapt<ContactDto>();

                //Vendor vendor = new Vendor();
                //var vendorDto = vendor.Adapt<VendorDto>();

                //OrderId orderId = OrderId.CreateUnique();
                //var order = new Order(orderId) ;
                //var ordDto = order.Adapt<OrderDto>();

                BasicQCMemberInfoDto basicQCMemberInfoDto = basicQCMemberInfoTask.Result.Adapt<BasicQCMemberInfoDto>();

                
                return Task.FromResult(basicQCMemberInfoDto);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return null;
            }            
        }
    }
}
