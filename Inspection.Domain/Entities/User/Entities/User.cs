using Inspection.Domain.Entities.Common;
using System;

namespace Inspection.Domain.Entities.User.Entities;

public class User : BaseDomainEntity<Guid>
{
    protected User(Guid id) : base(id)
    { }
}
