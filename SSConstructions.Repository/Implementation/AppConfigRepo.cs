using Microsoft.EntityFrameworkCore;
using SSConstructions.Repository.Entity;
using SSConstructions.Repository.Entity.EntityClass;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;

namespace SSConstructions.Repository.Implementation
{
    public class AppConfigRepo : IAppConfigRepo 
    {
        private readonly ConstructionDbContext _context;
        private readonly CustomLogger _logger;
        public AppConfigRepo(ConstructionDbContext context, CustomLogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<AppConfigModel> GetAppConfig(int accountId)
        {
            AppConfigModel appConfig = new AppConfigModel();
            try
            {
                _logger.Log("AppAccountRepo.GetAppConfig Started.");
                var config = await _context.AppConfigs.FirstOrDefaultAsync(x => x.AccountId == accountId);
                if (config != null)
                {
                    appConfig.AppName = config.AppName;
                    appConfig.AppConfigId = config.AppConfigId;
                    appConfig.AppDescription = config.AppDescription;
                    appConfig.ContactMail = config.ContactMail;
                    appConfig.ContactNumber = config.ContactNumber;
                    appConfig.EnableFacebook = config.EnableFacebook;
                    appConfig.EnableInstagram = config.EnableInstagram;
                    appConfig.EnableWhatsapp = config.EnableWhatsapp;
                    appConfig.EnableYoutube = config.EnableYoutube;
                    appConfig.FacebookLink = config.FacebookLink;
                    appConfig.InstagramLink = config.InstagramLink;
                    appConfig.WhatsappLink = config.WhatsappLink;
                    appConfig.YoutubeLink = config.YoutubeLink;
                    appConfig.AppLogo = config.AppLogo;
                    appConfig.AccountAddress = config.AccountAddress;
                    appConfig.LocationLink = config.LocationLink;
                }
                return appConfig;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<AppConfigModel> InsertUpdateConfig(AppConfigModel config)
        {
            var existing = _context.AppConfigs.FirstOrDefault(x => x.AccountId == config.AccountId);

            if (existing == null)
            {
                AppConfig appConfig = new AppConfig()
                {
                    AppName = config.AppName,
                    AppConfigId = config.AppConfigId,
                    AccountId = config.AccountId,
                    AppDescription = config.AppDescription,
                    ContactMail = config.ContactMail,
                    ContactNumber = config.ContactNumber,
                    EnableFacebook = config.EnableFacebook,
                    EnableInstagram = config.EnableInstagram,
                    EnableWhatsapp = config.EnableWhatsapp,
                    EnableYoutube = config.EnableYoutube,
                    FacebookLink = config.FacebookLink,
                    InstagramLink = config.InstagramLink,
                    WhatsappLink = config.WhatsappLink,
                    YoutubeLink = config.YoutubeLink,
                    AppLogo = config.AppLogo,
                    AccountAddress = config.AccountAddress,
                    LocationLink = config.LocationLink,
                    CreatedDate = DateTime.Now,
                    CreatedBy = 1
                };
                _context.AppConfigs.Add(appConfig);
            }
            else
            {
                existing.AppName = config.AppName;
                existing.AppConfigId = config.AppConfigId;
                existing.AppDescription = config.AppDescription;
                existing.ContactMail = config.ContactMail;
                existing.ContactNumber = config.ContactNumber;
                existing.EnableFacebook = config.EnableFacebook;
                existing.EnableInstagram = config.EnableInstagram;
                existing.EnableWhatsapp = config.EnableWhatsapp;
                existing.EnableYoutube = config.EnableYoutube;
                existing.FacebookLink = config.FacebookLink;
                existing.InstagramLink = config.InstagramLink;
                existing.WhatsappLink = config.WhatsappLink;
                existing.YoutubeLink = config.YoutubeLink;
                existing.AppLogo = config.AppLogo;
                existing.AccountAddress = config.AccountAddress;
                existing.LocationLink = config.LocationLink;
            }

            _context.SaveChanges();
            return config;
        }
        public async Task<bool> ConfigExistsAsync(int accountId)
        {
            return await _context.AppConfigs.AnyAsync(x => x.AccountId == accountId);
        }
        public async Task<ResponseMsg> UpdateAppLogo(int accountId, string logoUrl)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("AppConfigRepo.UpdateAppLogo Started.");
                var existing = _context.AppConfigs.FirstOrDefault(x => x.AccountId == accountId);
                if (existing != null)
                {
                    existing.AppLogo = logoUrl;
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                    responseMsg.Id = existing.AppConfigId;
                    responseMsg.Message = "App Logo Updated Successfully";
                }
                else
                {
                    responseMsg.Message = "App Config Not Found";
                }
                _logger.Log("AppConfigRepo.UpdateAppLogo Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
