using System;


namespace Inspection.Application.DTOs.Common;

public class ContactDto
{
    public ContactDto() { }

    public Guid? Id { get; set; }
    public string? Name { get;  set; }= string.Empty;
    public string? UserName { get; set; } = string.Empty;
    public int? DomainId { get;  set; } = int.MaxValue;
    public AddressDto? Address { get;  set; } = new AddressDto();
    public string? Phone { get; set; } = string.Empty;
    public string? Fax { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? HomePage { get; set; } = string.Empty;
    public string? Note { get; set; } = string.Empty;
    public string? Cc { get; set; } = string.Empty;
    public string? Mobile { get; set; } = string.Empty;
    public int? ContactType { get; set; }
    public string? ContactTypeName { get; set; }      // "PidecInspector", "TPInspector" ...
    public bool? IsVisible { get; set; }
    public string? UserId {  get; set; }
    public string? Password { get; set; } = string.Empty;
    public Guid? ProjectContactId { get; set; }
}
