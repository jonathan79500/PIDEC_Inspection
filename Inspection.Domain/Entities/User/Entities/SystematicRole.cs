
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.User.Entities
{
    public class SystematicRole:IdentityRole<string>
    {
        public SystematicRole() : base() { }

        [MaxLength(8)]
        public string RoleCode { get; set; }
    }
}
