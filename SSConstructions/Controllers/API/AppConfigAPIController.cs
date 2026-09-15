using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SSConstructions.Controllers.API
{
    [ApiController]
    [EnableCors("addCors")]
    [Route("api/[controller]")]
    public class AppConfigAPIController : Controller
    {
        private readonly IAppConfigRepo _appConfigRepo;
        private readonly CustomLogger _logger;
        private readonly IWebHostEnvironment _env;

        public AppConfigAPIController(IAppConfigRepo appConfigRepo, CustomLogger logger, IWebHostEnvironment env)
        {
            _appConfigRepo = appConfigRepo;
            _logger = logger;
            _env = env;
        }
        [Route("getAppConfig")]
        [HttpGet()]
        public async Task<IActionResult> GetAppConfig(int accountId)
        {
            try
            {
                _logger.Log("AppConfigAPIController.GetAppConfig Started.");
                return Ok(await _appConfigRepo.GetAppConfig(accountId));
                _logger.Log("AppConfigAPIController.GetAppConfig Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
        [Route("InsertUpdateConfig")]
        [HttpPost()]
        public async Task<IActionResult> InsertUpdateConfig(AppConfigModel appConfigModel)
        {
            try
            {
                _logger.Log("AppConfigAPIController.InsertUpdateConfig Started.");
                return Ok(await _appConfigRepo.InsertUpdateConfig(appConfigModel));
                _logger.Log("AppConfigAPIController.InsertUpdateConfig Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("UploadAppLogo")]
        [HttpPost()]
        public async Task<IActionResult> UploadAppLogo([FromForm] int accountId, [FromForm] IFormFile logoFile)
        {
            try
            {
                _logger.Log("AppConfigAPIController.UploadAppLogo Started.");
                if (logoFile == null || logoFile.Length == 0)
                    return BadRequest("No logo provided.");

                if (!await _appConfigRepo.ConfigExistsAsync(accountId))
                    return BadRequest("App Config Not Found");

                var savedFile = await SaveLogoAsync(accountId, logoFile);
                var url = $"{Request.Scheme}://{Request.Host}/uploads/logos/{savedFile}";

                var update = await _appConfigRepo.UpdateAppLogo(accountId, url);
                if (update.StatusCode != 1)
                    return BadRequest(update.Message);

                _logger.Log("AppConfigAPIController.UploadAppLogo Completed.");
                return Ok(new { StatusCode = 1, ImageUrl = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        private async Task<string> SaveLogoAsync(int accountId, IFormFile logoFile)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) ext = ".png";
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".webp")
                ext = ".png";

            // Deterministic single-logo-per-account filename (overwrites previous).
            var fileName = $"app_logo_{accountId}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await logoFile.CopyToAsync(stream);
            }

            // Remove any legacy logo variants so only one logo file remains per account.
            foreach (var legacy in Directory.GetFiles(folder, $"app_logo_{accountId}.*"))
            {
                if (!legacy.Equals(fullPath, StringComparison.OrdinalIgnoreCase))
                {
                    try { System.IO.File.Delete(legacy); } catch { }
                }
            }

            return fileName;
        }
    }
}
