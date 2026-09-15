using SSConstructions.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSConstructions.Repository.Interface
{
    public interface IUsers
    {
        Task<List<RoleModel>> GetRoles();
        Task<List<UserModel>> GetUsers(int accountId);
        Task<List<UserModel>> GetAdminUsers(int accountId);
        Task<ResponseMsg> InsertUpdateUser(UserModel userModel);
        Task<UserModel> GetUserByEmail(int accountId, string email);
    }
}
