using Inspection.Domain.Entities.User.Entities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Inspection.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
  string GenerateToken(SystematicUser user,List<string> UserRoles);
}