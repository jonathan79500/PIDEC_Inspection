
using Inspection.Application.DTOs.Inspector;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Inspectors.Queries.GetInspectors
{
    public class GetInspectorsRequestHandler : IRequestHandler<GetInspectorsRequest, List<InspectorDto>>
    {

        public GetInspectorsRequestHandler() { }


        Task<List<InspectorDto>> IRequestHandler<GetInspectorsRequest, List<InspectorDto>>.Handle(GetInspectorsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
