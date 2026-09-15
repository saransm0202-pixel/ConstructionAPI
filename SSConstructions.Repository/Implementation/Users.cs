using Microsoft.EntityFrameworkCore;
using SSConstructions.Repository.Entity;
using SSConstructions.Repository.Entity.EntityClass;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;

namespace SSConstructions.Repository.Implementation
{
    public class Users : IUsers
    {
        private readonly ConstructionDbContext _context;
        private readonly CustomLogger _logger;
        public Users(CustomLogger logger, ConstructionDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<List<RoleModel>> GetRoles()
        {
            List<RoleModel> roles = new List<RoleModel>();
            try
            {
                _logger.Log("Users.GetRoles Started.");
                roles = await (from role in _context.MasterRoles
                               select new RoleModel
                               {
                                   RoleId = role.RoleId,
                                   Role = role.Role
                               }).ToListAsync();
                _logger.Log("Users.GetRoles Completed.");
                return roles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<UserModel>> GetUsers(int accountId)
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                _logger.Log("Users.GetUsers Started.");
                users = await (from user in _context.Users.Where(u => u.AccountId == accountId)
                               select new UserModel
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Email = user.Email,
                                   Phone = user.Phone,
                                   ProfilePic = user.ProfilePic,
                                   RoleId = user.RoleId,
                                   RoleName = user.Role.Role,
                                   IsActive = user.IsActive
                               }).ToListAsync();
                _logger.Log("Users.GetUsers Completed.");
                return users;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<UserModel>> GetAdminUsers(int accountId)
        {
            List<UserModel> users = new List<UserModel>();
            _logger.Log("Users.GetAdminUsers Started.");
            users = await (from user in _context.Users
                           where user.AccountId == accountId && user.IsActive && user.Role.Role.ToLower() == "admin"
                           select new UserModel
                           {
                               Id = user.Id,
                               FirstName = user.FirstName,
                               LastName = user.LastName,
                               Email = user.Email,
                               Phone = user.Phone,
                               RoleId = user.RoleId,
                               RoleName = user.Role.Role
                           }).ToListAsync();
            _logger.Log("Users.GetAdminUsers Completed.");
            return users;
        }
        public async Task<UserModel> GetUserByEmail(int accountId, string email)
        {
            UserModel user = null;
            try
            {
                _logger.Log("Users.GetUserByEmail Started.");
                user = await _context.Users.Where(u => u.AccountId == accountId && u.Email == email)
                                .Select(u => new UserModel
                                {
                                    Id = u.Id,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Email = u.Email,
                                    Phone = u.Phone,
                                    ProfilePic = u.ProfilePic,
                                    RoleId = u.RoleId,
                                    RoleName = u.Role.Role,
                                    IsActive = u.IsActive
                                }).FirstOrDefaultAsync();

                _logger.Log("Users.GetUserByEmail Completed.");

                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseMsg> InsertUpdateUser(UserModel userModel)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("Users.InsertUpdateUser Started.");
                if (userModel != null)
                {
                    if (userModel.Id > 0)
                    {
                        var selectedUser = await _context.Users.Where(a => a.Id == userModel.Id).FirstOrDefaultAsync();
                        selectedUser.FirstName = userModel.FirstName;
                        selectedUser.LastName = userModel.LastName;
                        selectedUser.Email = userModel.Email;
                        selectedUser.Phone = userModel.Phone;
                        selectedUser.ProfilePic = userModel.ProfilePic;
                        selectedUser.IsActive = userModel.IsActive;
                        selectedUser.RoleId = userModel.RoleId;
                        selectedUser.AccountId = userModel.AccountId;
                        responseMsg.Message = "User Updated Successfully";
                    }
                    else
                    {
                        User user = new User
                        {
                            FirstName = userModel.FirstName,
                            LastName = userModel.LastName,
                            Email = userModel.Email,
                            Phone = userModel.Phone,
                            ProfilePic = userModel.ProfilePic,
                            IsActive = userModel.IsActive,
                            RoleId = userModel.RoleId,
                            AccountId = userModel.AccountId
                        };
                        await _context.Users.AddAsync(user);
                        responseMsg.Message = "User Addedd Successfully";
                    }
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                }
                _logger.Log("Users.InsertUpdateUser Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
