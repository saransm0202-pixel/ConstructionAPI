using SSConstructions.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSConstructions.Repository.Interface
{
    public interface ILoginRepo
    {
        Task<string> GetAccountName(int accountId);
        Task<UserModel> LoginUser(UserModel userModel);
        void updateTokenInfo(int userId, string refreshToken, DateTime expiryTime, int accountId);
    }
}
