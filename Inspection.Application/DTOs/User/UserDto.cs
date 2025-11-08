using Inspection.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.DTOs.User
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }=string.Empty;
        public string ?UserName { get; set; }
        public int? DomainId { get; set; }
        public string? Email { get; set; } = string.Empty;   
        public string? Password { get; set; } = string.Empty;
        public int? Use_Id { get; set; }
        public string? SAMAccountName { get; set; }
       public int? UserType { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAuthenticated { get; set; }
        public string? Token { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword {get;set;}
        public UserDto() { }
    }
}
