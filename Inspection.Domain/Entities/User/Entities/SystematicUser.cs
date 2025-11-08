using Inspection.Domain.Entities.Contacts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Domain.Entities.User.Entities
{
    public class SystematicUser:IdentityUser
    {

        public SystematicUser():base() { }

        public string Token { get; set; }

        public int DomainId { get; set; }

        public bool IsAuthenticated { get; set; }

        public int Use_Id { get; set; }

        public bool IsActive {  get; set; }

        public DateTime activationStartDate { get; set; }

        public DateTime activationEndDate { get; set; }

        public int UserType {  get; set; }

        // Navigation property to user roles
        public virtual ICollection<SystematicUserRole> UserRoles { get; set; }

        // Optional: Convenience property to access roles directly
        public virtual ICollection<SystematicRole> Roles { get; set; }

        //     public List<Contact> contacts { get; set; }

        public void SetToken(string token)
        { this.Token = token; }
    }
}
