using Dapper;
using Inspection.Application.Common.Interfaces.Authentication;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.Models;
using Inspection.Application.User;
using Inspection.Domain.Constants;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.EDMS;
using Inspection.Domain.Entities.InspectionNotification.Enums;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.User.Enums;
using Inspection.Domain.Entities.ValueObjects;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Formats.Asn1.AsnWriter;

namespace Inspection.Persistence.Repositories
{
    public class UserManagerRepository : IUserManagerRepository
    {
        UserManager<SystematicUser> _userManager;
        SignInManager<SystematicUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRandomPasswordGenerator _randomPasswordGenerator;
        private IPasswordHasher<SystematicUser> passwordHasher { get; }
        private IServiceScopeFactory _serviceProvider;
        private readonly InspectionDbContext _dbcontext;
        private RoleManager<SystematicRole> _roleManager;
        private IConfiguration _configuration;

        public UserManagerRepository(InspectionDbContext dbContext, UserManager<SystematicUser> userManager, RoleManager<SystematicRole> roleManager, SignInManager<SystematicUser> signInManager, IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher<SystematicUser> _passwordHasher, IServiceScopeFactory serviceProvider, IConfiguration configuration,IRandomPasswordGenerator randomPasswordGenerator ) {

            _dbcontext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            passwordHasher = _passwordHasher;
            _serviceProvider = serviceProvider;
            _roleManager = roleManager;
            _configuration = configuration;
            _randomPasswordGenerator = randomPasswordGenerator;
        }

        public async Task<SystematicUser?> UpdateUserPassword_Token(SystematicUser user, string password, IList<string>? roles) {
            string userToken = "";
            try
            {
                using (var scope2 = _serviceProvider.CreateScope())
                {
                    var passwordHasher = new PasswordHasher<SystematicUser>();

                    var hashedPassword = passwordHasher.HashPassword(user, password);
                    //user.PasswordHash = hashedPassword;


                    var tokenGenerator = scope2.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
                    var userManager = scope2.ServiceProvider.GetRequiredService<UserManager<SystematicUser>>();

                    var trackedUser = await userManager.FindByIdAsync(user.Id);
                    if (trackedUser != null)
                    {
                        trackedUser.PasswordHash = hashedPassword;
                        userToken = tokenGenerator.GenerateToken(trackedUser!, roles!.ToList());
                        user!.SetToken(userToken);
                        await userManager.UpdateAsync(trackedUser);
                        await userManager.SetAuthenticationTokenAsync(trackedUser!, "bearer", "bearerToken", userToken);
                    }
                }
                return user;
            } catch (Exception exp)
            {
                return null;
            }
        }

        public async Task<Contact> SaveContact(Contact contact) {

            try
            {
                Contact existedContact = null;
               

                //if(contact.Id != Guid.Empty && contact.Id != null)
                existedContact = _dbcontext.Contacts.Where(ct => ct.ContactId == contact.ContactId).FirstOrDefault();
                if (existedContact == null)
                {
                    if (contact.UserId == "")
                        contact.UserId = null;
                    Contact newcnt = _dbcontext.Add(contact).Entity;
                    _dbcontext.SaveChanges();
                    return await Task.FromResult(contact);
                }
                else
                    return await Task.FromResult(existedContact);                    
            }
            catch (Exception ex) {
                return null;
            }
        }

        public Task<bool> SaveProjectContacts(Project project, List<Contact> contacts)
        {
            try {
                
                ProjectContact newProjectContact;
                ProjectContact projContact = null;
                Guid currentContactId;
                Guid currentProjectId;

                currentProjectId = (project.Id == null ? project.ProjectId : project.Id);
                for (int i = 0; i < contacts.Count; i++)
                {

                    //currentContactId = (contacts[i].Id == null ? contacts[i].ContactId.Value : contacts[i].Id);
                    projContact = _dbcontext.ProjectContacts.Where(pc => pc.ContactId == contacts[i].ContactId  && pc.ProjectId== currentProjectId).FirstOrDefault();
                    if (projContact == null)
                    {
                        newProjectContact = new ProjectContact();
                        newProjectContact.Contact = contacts[i];
                        newProjectContact.ContactId = contacts[i].ContactId;
                        newProjectContact.ProjectId = project.ProjectId ?? project.Id;
                        _dbcontext.ProjectContacts.Add(newProjectContact);
                        _dbcontext.SaveChanges();
                    }
                    projContact = null;
                }
                return Task.FromResult(true);
            }
            catch (Exception ex) { 
                return Task.FromResult(false);
            }
        }

        public async Task<(SystematicUser?, IList<string>?)> Login(string userName, string password, int domainId)
        {
            string userToken = "";
            try
            {

                using (var scope2 = _serviceProvider.CreateScope())
                {

                    var tokenGenerator = scope2.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
                    var userManager = scope2.ServiceProvider.GetRequiredService<UserManager<SystematicUser>>();
                    var dbContext = scope2.ServiceProvider.GetRequiredService<InspectionDbContext>();

                    var user = userManager.Users.Where(user => user.UserName.ToLower() == userName.ToLower()).FirstOrDefault();

                    //user.PasswordHash = passwordHasher.HashPassword(user,password);

                    if (user != null)
                    {

                        //await userManager.UpdateSecurityStampAsync(user);
                        // List<string> roles = GetUserRoles(user).Result.ToList<string>();
                        IList<string> roles = await userManager.GetRolesAsync(user);
                        if (roles.Contains("QC_Lead") || roles.Contains("QC_Manager"))
                        {
                            if (await userManager.CheckPasswordAsync(user, password) == true)
                            {

                                userToken = tokenGenerator.GenerateToken(user, roles.ToList());
                                user.IsAuthenticated = true;
                                user.SetToken(userToken);
                                await userManager.SetAuthenticationTokenAsync(user, "bearer", "bearerToken", userToken);

                            }
                            else {
                                user.IsAuthenticated = false;

                            }

                            return (user, roles);
                        }
                        else if (roles.Contains("Vendor"))
                        {
                            if (user.PasswordHash != null)
                            {
                                if (await userManager.CheckPasswordAsync(user, password) == true)
                                {

                                    userToken = tokenGenerator.GenerateToken(user, roles.ToList());
                                    user.IsAuthenticated = true;
                                    user.SetToken(userToken);
                                    await userManager.SetAuthenticationTokenAsync(user, "bearer", "bearerToken", userToken);
                                    return (user, roles);

                                }
                                else
                                {
                                    user.IsAuthenticated = false;
                                    return (user, roles);
                                }
                            }
                            else
                            {
                                user.IsAuthenticated = false;
                                return (user, roles);
                            }
                        }
                        else if (roles.Contains("PIDECDepartment_Lead"))
                        {
                            if (user.PasswordHash != null)
                            {
                                if (await userManager.CheckPasswordAsync(user, password) == true)
                                {

                                    userToken = tokenGenerator.GenerateToken(user, roles.ToList());
                                    user.IsAuthenticated = true;
                                    user.SetToken(userToken);
                                    await userManager.SetAuthenticationTokenAsync(user, "bearer", "bearerToken", userToken);
                                    return (user, roles);

                                }
                                else
                                {
                                    user.IsAuthenticated = false;
                                    return (user, roles);
                                }
                            }
                            else
                            {
                                user.IsAuthenticated = false;
                                return (user, roles);
                            }
                        }
                        else if (roles.Contains("MC"))
                        {
                            if (user.PasswordHash != null)
                            {
                                if (await userManager.CheckPasswordAsync(user, password) == true)
                                {

                                    userToken = tokenGenerator.GenerateToken(user, roles.ToList());
                                    user.IsAuthenticated = true;
                                    user.SetToken(userToken);
                                    await userManager.SetAuthenticationTokenAsync(user, "bearer", "bearerToken", userToken);
                                    return (user, roles);

                                }
                                else
                                {
                                    user.IsAuthenticated = false;
                                    return (user, roles);
                                }
                            }
                            else
                            {
                                user.IsAuthenticated = false;
                                return (user, roles);
                            }
                        }
                        else if (roles.Contains("PIDEC_Inspector") || roles.Contains("ThirdPart_Inspector"))
                        {
                            if (user.PasswordHash != null)
                            {
                                if (await userManager.CheckPasswordAsync(user, password) == true)
                                {

                                    userToken = tokenGenerator.GenerateToken(user, roles.ToList());
                                    user.IsAuthenticated = true;
                                    user.SetToken(userToken);
                                    await userManager.SetAuthenticationTokenAsync(user, "bearer", "bearerToken", userToken);
                                    return (user, roles);

                                }
                                else
                                {
                                    user.IsAuthenticated = false;
                                    return (user, roles);
                                }
                            }
                            else
                            {
                                user.IsAuthenticated = false;
                                return (user, roles);
                            }
                        }
                        //if (await _userManager.CheckPasswordAsync(user, password) == true)
                        //{

                        //   // List<string> roles = GetUserRoles(user).Result.ToList<string>();


                        //    //_userManager.UpdateAsync(user);      

                        //}
                    }
                }
                return (null, null);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return (null, null);
            }

        }

        public async Task<SystematicUser> SetUserToken(SystematicUser currentUser, string token)
        {


            try
            {
                if (currentUser != null)
                {
                    var result = await _userManager.GenerateUserTokenAsync(currentUser, currentUser.PasswordHash, "login");
                    if (result.ToString().Equals("succeed"))
                        return await Task.FromResult(currentUser);
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public async Task<List<SystematicUser>> GetUsersInfoByProjectId(string projectId) {


            var conn = _dbcontext.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();

            // enlist EF Core’s current transaction (if any) so Dapper and EF share the same TX
            var adoTx = _dbcontext.Database.CurrentTransaction?.GetDbTransaction();

            var userDictionary = new Dictionary<string, SystematicUser>();

            // NOTE: no using(conn) here—let DI/DbContext own the lifecycle
            var rows = conn.Query<
                SystematicUser,     // the first type
                SystematicRole,     // the second type
                IdentityRole,       // the third type
                SystematicUser      // your return type
            >(
                sql: "GetProjectUsers_ProjectId",
                map: (u, r, aspRole) =>
                {
                    if (!userDictionary.TryGetValue(u.Id, out var existing))
                    {
                        existing = u;
                        userDictionary[u.Id] = existing;
                    }

                    // build your domain‐model role from the ASP IdentityRole
                    var sr = new SystematicRole
                    {
                        Id = aspRole.Id,
                        Name = aspRole.Name
                        // …etc
                    };

                    if (existing.Roles == null)
                    {
                        existing.Roles = new List<SystematicRole>();
                    }

                    existing.Roles.Add(sr);

                    return existing;
                },
                param: new { projectId },
                transaction: adoTx,
                commandType: CommandType.StoredProcedure,
                splitOn: "UserId,Id" // see note below
            );

            var users = userDictionary.Values.ToList();


            //try
            //{
            //    using (var multi = connection.QueryMultiple(
            //"GetProjectUsers_ProjectId",
            //new { projectId = new Guid(projectId) },
            //commandType: CommandType.StoredProcedure,
            //transaction: adoTransaction))
            //    {

            //        users = multi.Read().Select(u => new SystematicUser()
            //        {
            //            AccessFailedCount = u.AccessFailedCount,
            //            activationEndDate = u.ActivationEndDate,
            //            activationStartDate = u.ActivationStartDate,
            //            ConcurrencyStamp = u.ConcurrencyStamp,
            //            contacts = u.Contacts,
            //            DomainId = u.DomainId,
            //            Email = u.Email,
            //            EmailConfirmed = u.EmailConfirmed,
            //            Id = u.Id,
            //            IsActive = u.IsActive,
            //            IsAuthenticated = u.IsAuthenticated,
            //            LockoutEnabled = u.LockoutEnabled,
            //            LockoutEnd = u.LockoutEnd,
            //            NormalizedEmail = u.NormalizedEmail,
            //            NormalizedUserName = u.NormalizedUserName,
            //            PasswordHash = u.PasswordHash,
            //            PhoneNumber = u.PhoneNumber,
            //            PhoneNumberConfirmed = u.PhoneNumberConfirmed,
            //            SecurityStamp = u.SecurityStamp,
            //            Token = u.Token,
            //            TwoFactorEnabled = u.TwoFactorEnabled,
            //            UserName = u.UserName,
            //            UserType = u.UserType,
            //            Use_Id = u.UseId
            //        }).ToList();
            //    }

            //        //              orderInspectionSubjects = multi.Read<, InspectionSubject, OrderInspectionSubject>(
            //        //(orderInspectionSubject, inspectionSubject) =>
            //        //{
            //        //    orderInspectionSubject.InspectionSubject = inspectionSubject;
            //        //    if (orderInspectionSubject?.OrderId! != null)
            //        //    {
            //        //        orderInspectionSubject.OrderId = OrderId.Create(new Guid(orderInspectionSubject.OrderId.Value.ToString()));
            //        //    }
            //        //    orderInspectionSubject!.InspectionSubjectId = inspectionSubject.InspectionSubjectId;
            //        //    orderInspectionSubject.Id = orderInspectionSubject.Id;
            //        //    return orderInspectionSubject;
            //        //},
            //        //splitOn: "InspectionSubjectId"
            //        //          }
            //    }
            //catch (Exception ex) { }
            return users;
        }

        public async Task<SystematicRole> AddNewRole(string RoleName, string RoleCode)
        {
            SystematicRole createdRole = new SystematicRole();
            IdentityResult identityResult = IdentityResult.Failed();

            try
            {
                if (RoleName != null)
                {
                    if (!await _roleManager.RoleExistsAsync(RoleName))
                    {
                        createdRole.RoleCode = RoleCode;
                        createdRole.Name = RoleName;
                        createdRole.Id = Guid.NewGuid().ToString();
                        identityResult = await _roleManager.CreateAsync(createdRole);

                        if (identityResult.Succeeded)
                            return await Task.FromResult(createdRole);
                        else
                            return null;
                    }
                    else
                    {
                        Task<SystematicRole?> task = _roleManager.FindByNameAsync(RoleName);
                        return await task;
                    }
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public async Task<bool> AddUserToRole(SystematicUser user, string roleName)
        {
            IdentityResult identityResult = IdentityResult.Failed();
            try
            {
                if (user != null)
                {

                    var identityResult1 = await _userManager.AddToRoleAsync(user, roleName);
                    if (identityResult1.Succeeded)
                        return await Task<bool>.FromResult(true);
                    else
                        return false;
                }
            }
            catch (Exception exp)
            {
                return await Task<bool>.FromResult(false);
            }
            return await Task<bool>.FromResult(false);
        }

        public async Task<SystematicUser> GetActiveUserByUserName_DomainId(string userName, int domainId)
        {
           var result =_dbcontext.Users.Where(usr => usr.UserName == userName && usr.DomainId == domainId && usr.IsActive == true).ToList();
            if(result.Count >0 )
                return await Task.FromResult(result[0]);
            return null;
        }

        public async Task<(SystematicUser,string )> RegisterNewUser(SystematicUser newUser, List<string> RoleNames)
        {
            SystematicUser createdUser;

            try
            {
                if (newUser != null)
                {                  

                    newUser.IsAuthenticated = true;
                    newUser.IsActive = true;
                    newUser.Use_Id = 0;
                    newUser.activationStartDate = DateTime.Now;
                    newUser.activationEndDate = DateTime.Now.AddYears(2);
                    newUser.TwoFactorEnabled = false;
                    newUser.PhoneNumberConfirmed = true;
                    newUser.AccessFailedCount = 5;
                    newUser.EmailConfirmed = true;
                    newUser.LockoutEnabled = true;
                    if (newUser.Token == null)
                        newUser.Token = _jwtTokenGenerator.GenerateToken(newUser,RoleNames).ToString();


                    var result = await _userManager.CreateAsync(newUser);
                    if (result.Succeeded)
                    {
                        var resetPasswordtoken = await _userManager.GeneratePasswordResetTokenAsync(newUser);
                        var UserId = await _userManager.GetUserIdAsync(newUser);
                        var encodedToken = WebUtility.UrlEncode(resetPasswordtoken);
                        var roleAddedResult = await _userManager.AddToRolesAsync(newUser, RoleNames);
                        if (roleAddedResult.Succeeded)
                        {
                            var callbackUrl = $"https://yourdomain.com/Account/SetPassword?userId={UserId}&token={encodedToken}";


                            return await Task.FromResult((newUser, callbackUrl));
                        }
                        else
                            return (null, "");
                    }
                    else
                        return (null,"");
                }
                else
                    return (null, "");
            }
            catch (Exception exp) {
                return (null, "");
            }
        }

        public string GetContactTypeName_Code(int code)
        {          

            switch (code)
            {
                case 0: return "Undefined";
                case 1: return "Owner";
                case 2: return "Vendor";
                case 3: return "MC";
                case 4: return "QC_Manager";
                case 5: return "Supplier";
                case 6: return "SubSupplier";
                case 7: return "QC_Lead";
                case 8: return "PIDEC_Inspector";
                case 9: return "ThirdPart_Inspector";
                case 10: return "Department_Lead";
                case 11: return "Department_Originator";
                default: return "";
            }

        }
        public int GetRoleCode_RoleName(string RoleName) {
            return int.Parse(_dbcontext.Roles.Where(r => r.Name == RoleName).FirstOrDefault().RoleCode);
        }

        public async Task<(SystematicUser, string)> RegisterNewUserByRandomPass(SystematicUser newUser, List<string> RoleNames)
        {
            SystematicUser createdUser;

            try
            {
                var existedUser = _userManager.Users.Where(usr => usr.Email == newUser.Email && usr.UserName == newUser.UserName).ToList();//&& usr.UserName == vendorContact.UserName).ToList();
                
                if (newUser != null && existedUser.Count == 0)
                {

                    newUser.IsAuthenticated = true;
                    newUser.IsActive = true;
                    newUser.Use_Id = 0;
                    newUser.activationStartDate = DateTime.Now;
                    newUser.activationEndDate = DateTime.Now.AddYears(2);
                    newUser.TwoFactorEnabled = false;
                    newUser.PhoneNumberConfirmed = true;
                    newUser.AccessFailedCount = 5;
                    newUser.EmailConfirmed = true;
                    newUser.LockoutEnabled = true;
                    if (newUser.Token == null)
                        newUser.Token = _jwtTokenGenerator.GenerateToken(newUser, RoleNames).ToString();

                    string randomPassword = _randomPasswordGenerator.GenerateSecurePassword();
                    var result = await _userManager.CreateAsync(newUser,randomPassword);
                    if (result.Succeeded)
                    {
                        var roleresult = await _userManager.AddToRolesAsync(newUser, RoleNames);
                        if (roleresult.Succeeded)
                            return await Task.FromResult((newUser, randomPassword));
                        //    string baseURL = _configuration["BaseUrl"].ToString();
                        //var callbackUrl = baseURL + $"/SetPassword?userId={newUser.Id}&p={randomPassword}";
                        else
                            return (null, "");

                    }
                    else
                        return (null, "");
                }
                else
                    return (null, "");
            }
            catch (Exception exp)
            {
                return (null, "");
            }
        }

        public async Task<(SystematicUser, string)> RegisterNewUserByRandomPassFromContactData(Contact contactData, List<string> RoleNames)
        {
            SystematicUser createdUser = new SystematicUser();

            try
            {
                var existedUser = _userManager.Users.Where(usr => usr.Email == contactData.Email && usr.UserName == contactData.UserName).ToList();//&& usr.UserName == vendorContact.UserName).ToList();

                if (contactData != null && existedUser.Count == 0)
                {

                    createdUser.Id = Guid.NewGuid().ToString();
                    createdUser.AccessFailedCount = 0;
                    createdUser.activationEndDate = DateTime.Now.AddYears(2);
                    createdUser.activationStartDate = DateTime.Now;
                    createdUser.ConcurrencyStamp = Guid.NewGuid().ToString();
                    createdUser.DomainId = 1;
                    createdUser.EmailConfirmed = true;
                    createdUser.IsActive = true;
                    createdUser.IsAuthenticated = true;
                    createdUser.LockoutEnabled = true;
                    createdUser.LockoutEnd = DateTime.Now.AddSeconds(600);
                    createdUser.NormalizedEmail = contactData.Email;
                    createdUser.NormalizedUserName = contactData.UserName;
                    createdUser.PhoneNumber = contactData.Phone;
                    createdUser.PhoneNumberConfirmed = true;
                   // createdUser.Token = "1";
                    createdUser.TwoFactorEnabled = true;
                    createdUser.UserType = GetRoleCode_RoleName(RoleNames.First());
                    createdUser.Use_Id = 3;
                    createdUser.UserName = contactData.UserName;//prjContact.Name,
                    createdUser.Email = contactData.Email;
                    createdUser.IsAuthenticated = true;
                    createdUser.IsActive = true;
                    createdUser.Use_Id = 0;
                    createdUser.activationStartDate = DateTime.Now;
                    createdUser.activationEndDate = DateTime.Now.AddYears(2);
                    createdUser.TwoFactorEnabled = false;
                    createdUser.PhoneNumberConfirmed = true;
                    createdUser.AccessFailedCount = 5;
                    createdUser.EmailConfirmed = true;
                    createdUser.LockoutEnabled = true;
                    if (createdUser.Token == null)
                        createdUser.Token = _jwtTokenGenerator.GenerateToken(createdUser, RoleNames).ToString();

                    string randomPassword = _randomPasswordGenerator.GenerateSecurePassword();
                    var result = await _userManager.CreateAsync(createdUser, randomPassword);
                    if (result.Succeeded)
                    {
                      var  roleresult=  await _userManager.AddToRolesAsync(createdUser, RoleNames);
                        if (roleresult.Succeeded)
                            return await Task.FromResult((createdUser, randomPassword));
                        //string baseURL = _configuration["BaseUrl"].ToString();
                        //var callbackUrl = baseURL + $"/SetPassword?userId={createdUser.Id}&p={randomPassword}";
                        else
                            return (null, "");

                    }
                    else
                        return (null, "");
                }
                else
                    return (existedUser[0], "");
            }
            catch (Exception exp)
            {
                return (null, "");
            }
        }

        public async Task<string> GetResetPasswordLink(string userId)
        {
            try
            {
                
                var founduser = await _userManager.FindByIdAsync(userId);

                var resetPasswordtoken = await _userManager.GeneratePasswordResetTokenAsync(founduser);
                var UserId = await _userManager.GetUserIdAsync(founduser);
                var encodedToken = WebUtility.UrlEncode(resetPasswordtoken);
                string baseURL = _configuration.GetValue<string>("BaseUrl").ToString();
                var callbackUrl = baseURL+$"/Account/SetPassword?userId={UserId}&token={encodedToken}";


                return await Task.FromResult(callbackUrl);
            }
            catch (Exception exp) {
                return "";
            }
        }

        async Task<bool> IUserManagerRepository.IsUserEmailExist(string Email)
        {
           var emailExist =  await _userManager.FindByEmailAsync(Email);
            if(emailExist != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IList<string>> GetUserRoles(SystematicUser currentUser)
        {
            try
            {
                IList<string> result = await _userManager.GetRolesAsync(currentUser);
                return await Task.FromResult(result);
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        public async Task<bool> ResetPassword(SystematicUser user, string newPassword,string oldPassword) {
            
            var founduser = await _userManager.FindByIdAsync(user.Id);
            if (founduser == null)
            {
                return false;
            }
            var resetPasswordToken = WebUtility.UrlDecode(founduser.Token);
            var result = await _userManager.ResetPasswordAsync(founduser, resetPasswordToken, newPassword);
            if (result.Succeeded)
            {
                // Optionally sign the user in or redirect to login
                return true;
            }
            else
            {
                result = await _userManager.ChangePasswordAsync(founduser,oldPassword,newPassword);
                if (result.Succeeded)
                {
                    // Optionally sign the user in or redirect to login
                    return true;
                }
            }

                return false;
        }


    }
}
