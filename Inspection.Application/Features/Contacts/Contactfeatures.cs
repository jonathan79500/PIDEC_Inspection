using Inspection.Application.Contracts.Persistence;
using Inspection.Application.DTOs;
using Inspection.Application.DTOs.Common;
using Inspection.Application.DTOs.Inspector;
using Inspection.Domain.Entities.Contacts;    // Contact: adjust to your namespace
using Inspection.Domain.Entities.Enums;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inspection.Application.Features.Contacts;

// ===================== queries =====================
public record GetContactsByProjectQuery(Guid ProjectId) : IRequest<List<ContactDto>>;
public record GetInspectorContactsQuery : IRequest<List<ContactDto>>;

// ===================== handlers =====================
public class GetContactsByProjectHandler : IRequestHandler<GetContactsByProjectQuery, List<ContactDto>>
{
    private readonly IContactRepository _repository;
    public GetContactsByProjectHandler(IContactRepository repository) => _repository = repository;

    public async Task<List<ContactDto>> Handle(GetContactsByProjectQuery request, CancellationToken ct)
    {
        var contacts = await _repository.GetByProjectAsync(request.ProjectId,  ct);
        return contacts.Adapt<List<ContactDto>>();
    }
}

// "Inspector" contacts are the two inspector contact types
public class GetInspectorContactsHandler : IRequestHandler<GetInspectorContactsQuery, List<ContactDto>>
{
    private static readonly int[] InspectorTypes =
    {
        (int)ContactType.PIDEC_Inspector,
        (int)ContactType.ThirdPart_Inspector
    };

    private readonly IContactRepository _repository;
    public GetInspectorContactsHandler(IContactRepository repository) => _repository = repository;

    public async Task<List<ContactDto>> Handle(GetInspectorContactsQuery request, CancellationToken ct)
    {
        var contacts = await _repository.GetByTypesAsync(InspectorTypes, ct);
        return contacts.Adapt<List<ContactDto>>();
    }
}

public class GetContactOptionsQuery : IRequest<List<ContactOptionDto>> {
    public IReadOnlyCollection<int> ContactTypes { get; set; } = [0, 1, 2,3,4,5,6,7,8,9,10,11];
}

public class GetContactOptionsQueryHandler : IRequestHandler<GetContactOptionsQuery, List<ContactOptionDto>>
{
    private readonly IContactRepository _repository;
    public GetContactOptionsQueryHandler(IContactRepository contactRepository)
    {
        _repository =contactRepository;
    }
    public Task<List<ContactOptionDto>> Handle(GetContactOptionsQuery request, CancellationToken cancellationToken)
    {
        //var dict = Enum.GetValues<ContactType>()
        // .ToDictionary(e => (int)e, e => e.ToString());

        IReadOnlyCollection<int> contactTypeCodes = request.ContactTypes;
        var contacts = _repository.GetByTypesAsync(contactTypeCodes,cancellationToken);
        List<ContactOptionDto> contactOptionDtos = contacts.Result.Adapt<List<ContactOptionDto>>();
        return Task.FromResult(contactOptionDtos);
    }
}
/// <summary>
/// /////////////////Filter Contacts by both Project and ContactTypes/////////////////////////////
/// </summary>

public class GetContactOptionsQueryByProject_ContactTypes : IRequest<List<ContactOptionDto>>
{
    public Guid ProjectId { get; set; }
    public IReadOnlyCollection<int> ContactTypes { get; set; } = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
}

public class GetContactOptionsQueryByProject_ContactTypesHandler : IRequestHandler<GetContactOptionsQueryByProject_ContactTypes, List<ContactOptionDto>>
{
    private readonly IContactRepository _repository;
    public GetContactOptionsQueryByProject_ContactTypesHandler(IContactRepository contactRepository)
    {
        _repository = contactRepository;
    }
    public async Task<List<ContactOptionDto>> Handle(GetContactOptionsQueryByProject_ContactTypes request, CancellationToken cancellationToken)
    {
        //var dict = Enum.GetValues<ContactType>()
        // .ToDictionary(e => (int)e, e => e.ToString());

        IReadOnlyCollection<int> contactTypeCodes = request.ContactTypes;
        Guid projectId = request.ProjectId;
        var contacts = await _repository.GetByProject_TypesAsync(projectId,contactTypeCodes, cancellationToken);
        List<ContactOptionDto> contactOptionDtos = contacts.Adapt<List<ContactOptionDto>>();
        return contactOptionDtos;
    }
}

/// <summary>
/// ////////////////Filter Contacts By contactTypes only////////////////////////////////
/// </summary>

public class GetContactOptionsQueryByContactTypes : IRequest<List<ContactOptionDto>>
{
    public IReadOnlyCollection<int> ContactTypes { get; set; } = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
}

public class GetContactOptionsQueryByContactTypesHandler : IRequestHandler<GetContactOptionsQueryByContactTypes, List<ContactOptionDto>>
{
    private readonly IContactRepository _repository;
    public GetContactOptionsQueryByContactTypesHandler(IContactRepository contactRepository)
    {
        _repository = contactRepository;
    }
    public Task<List<ContactOptionDto>> Handle(GetContactOptionsQueryByContactTypes request, CancellationToken cancellationToken)
    {
        //var dict = Enum.GetValues<ContactType>()
        // .ToDictionary(e => (int)e, e => e.ToString());

        IReadOnlyCollection<int> contactTypeCodes = request.ContactTypes;      
        var contacts = _repository.GetByTypesAsync( contactTypeCodes, cancellationToken);
        List<ContactOptionDto> contactOptionDtos = contacts.Result.Adapt<List<ContactOptionDto>>();
        return Task.FromResult(contactOptionDtos);
    }
}
