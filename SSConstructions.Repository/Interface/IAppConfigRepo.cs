using SSConstructions.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSConstructions.Repository.Interface
{
    public interface IAppConfigRepo
    {
        Task<AppConfigModel> GetAppConfig(int accountId);
        Task<AppConfigModel> InsertUpdateConfig(AppConfigModel config);
        Task<bool> ConfigExistsAsync(int accountId);
        Task<ResponseMsg> UpdateAppLogo(int accountId, string logoUrl);
    }
}
