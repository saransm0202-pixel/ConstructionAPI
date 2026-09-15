using Microsoft.EntityFrameworkCore;
using SSConstructions.Repository.Entity;
using SSConstructions.Repository.Entity.EntityClass;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;

namespace SSConstructions.Repository.Implementation
{
    public class LoginRepo : ILoginRepo
    {
        private readonly ConstructionDbContext _context;
        private readonly CustomLogger _logger;
        public LoginRepo(ConstructionDbContext context, CustomLogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<string> GetAccountName(int accountId)
        {
            if (accountId > 0)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
                if (account != null)
                {
                    return account.AccountName;
                }
            }
            return string.Empty;
        }
        public async Task<UserModel> LoginUser(UserModel userModel)
        {
            UserModel logedInUser = new UserModel();
            try
            {
                if (userModel != null && !string.IsNullOrEmpty(userModel.Email))
                {
                    User user = await _context.Users.Include(u => u.Role).Where(a => a.AccountId == userModel.AccountId && a.Email.Trim() == userModel.Email.Trim()).FirstOrDefaultAsync();

                    if (user != null) {
                        logedInUser.Id = user.Id;
                        logedInUser.FirstName = user.FirstName;
                        logedInUser.LastName = user.LastName;
                        logedInUser.Email = user.Email;
                        logedInUser.Phone = user.Phone;
                        logedInUser.RoleId = user.RoleId;
                        logedInUser.Phone = user.Phone;
                        logedInUser.RoleName = user.Role.Role;
                        logedInUser.ProfilePic = user.ProfilePic;
                    }
                    else
                    {
                        logedInUser.Id = -1;
                        logedInUser.LoginMsg = "UnAuthorized User Access, Please Contact Administra tor.";
                    }

                }
                return logedInUser;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void updateTokenInfo(int userId, string refreshToken, DateTime expiryTime, int accountId)
        {
            if (userId > 0)
            {
                var user = _context.Users.Where(a => a.Id == userId && a.AccountId == accountId).FirstOrDefault();
                if (user != null)
                {
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiry = expiryTime;
                    _context.SaveChanges();
                }
            }
        }
    }
}
