using Inspection.Domain.Constants;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Persistence
{
    public interface IUserManagerRepository
    {
        public Task<bool> IsUserEmailExist(string Email);

        public Task<(SystematicUser,string)> RegisterNewUser(SystematicUser newUser,List<string> RoleNames);

        public Task<(SystematicUser, string)> RegisterNewUserByRandomPass(SystematicUser newUser, List<string> RoleNames);

        public Task<(SystematicUser, string)> RegisterNewUserByRandomPassFromContactData(Contact contactData, List<string> RoleNames);

        public Task<SystematicRole> AddNewRole(string RoleName, string RoleCode);

        public Task<(SystematicUser?,IList<String>?)> Login(string userName,string password,int domainId);

        public Task<SystematicUser> SetUserToken(SystematicUser newUser,string token);

        public Task<IList<string>> GetUserRoles(SystematicUser user);

        public Task<SystematicUser?> UpdateUserPassword_Token(SystematicUser user,string password,IList<string> userRoles);

        public Task<List<SystematicUser>> GetUsersInfoByProjectId(string projectId);

        public Task<bool> AddUserToRole(SystematicUser user, string roleName);

        public Task<SystematicUser> GetActiveUserByUserName_DomainId(string userName, int domainId);

        public Task<bool> ResetPassword(SystematicUser user,string newPassword, string oldPassword);

        public string GetContactTypeName_Code(int RoleHashCode);

        public Task<Contact> SaveContact(Contact contact);

        public Task<bool> SaveProjectContacts(Project project, List<Contact> contacts);
    }
}
