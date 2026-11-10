using Inspection.Application.DTOs.Inspector;
using MediatR;
using System.Collections.Generic;

namespace Inspection.Application.Features.Inspectors.Queries.GetInspectors;

public class GetInspectorsRequest:IRequest<List<InspectorDto>>
{
}
