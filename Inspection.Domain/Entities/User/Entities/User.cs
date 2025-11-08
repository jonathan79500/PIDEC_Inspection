using Inspection.Domain.Entities.Common;
using System;

namespace Inspection.Domain.Entities.User.Entities;

public class User : BaseDomainEntity<Guid>
{
    public Guid Id { get; private set; } = Guid.Empty;
    public string Name { get; private set; } = string.Empty;
    public string UserName { get; private set; }
    public int domainId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public int Use_Id { get; private set; }
    public string SAMAccountName { get; private set; }
    public int UserType { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsAuthenticated { get; private set; }

    public string Token { get; set; }

    public User() { }

    protected User(Guid id) : base(id)
    { }

    public void SetToken(string token)
    { this.Token = token; }
}
