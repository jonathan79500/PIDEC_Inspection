using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.User.Entities
{
    public class SystematicUserRole : Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>
    {
        public virtual SystematicUser User { get; set; }
        public virtual SystematicRole Role { get; set; }
    }
}
