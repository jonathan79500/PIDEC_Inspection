using System;
using System.Collections.Generic;
using System.Linq;


namespace Inspection.Application.User
{
    public record CurrentUser(string Id, string Email, IEnumerable<string> Roles)
    {        
        public bool IsInRole(string role) => Roles.Contains(role);
    }
}
