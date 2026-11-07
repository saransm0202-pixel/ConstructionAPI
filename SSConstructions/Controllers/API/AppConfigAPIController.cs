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
            string? savedPath = null;
            try
            {
                _logger.Log("AppConfigAPIController.UploadAppLogo Started.");
                if (logoFile == null || logoFile.Length == 0)
                    return BadRequest("No logo provided.");

                if (!await _appConfigRepo.ConfigExistsAsync(accountId))
                    return BadRequest("App Config Not Found");

                var saved = await SaveLogoAsync(accountId, logoFile);
                savedPath = saved.FullPath;
                var url = $"{Request.Scheme}://{Request.Host}/uploads/logos/{saved.FileName}";

                var update = await _appConfigRepo.UpdateAppLogo(accountId, url);
                if (update.StatusCode != 1)
                {
                    TryDelete(savedPath);
                    return BadRequest(update.Message);
                }

                // Only clean up the previous logo after the database points at the new one.
                DeleteOtherLogoFiles(accountId, saved.FileName);

                _logger.Log("AppConfigAPIController.UploadAppLogo Completed.");
                return Ok(new { StatusCode = 1, ImageUrl = url });
            }
            catch (Exception ex)
            {
                if (savedPath != null) TryDelete(savedPath);
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        private string LogoFolder
        {
            get
            {
                var webRoot = _env.WebRootPath
                    ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                return Path.Combine(webRoot, "uploads", "logos");
            }
        }

        private async Task<(string FileName, string FullPath)> SaveLogoAsync(int accountId, IFormFile logoFile)
        {
            var folder = LogoFolder;
            Directory.CreateDirectory(folder);

            // Trust the file's signature, not its name: a mislabelled PNG/JPEG/WebP
            // would otherwise be served with the wrong content type.
            var ext = await DetectImageExtensionAsync(logoFile)
                ?? throw new InvalidOperationException(
                    "Unsupported image format. Please upload a PNG, JPG or WebP image.");

            // Fresh name per upload. Reusing app_logo_{accountId}.png lets the host's
            // static-asset cache keep serving the previous length, truncating the image.
            var fileName = $"app_logo_{accountId}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await logoFile.CopyToAsync(stream);
                await stream.FlushAsync();
            }

            if (!IsCompleteImage(fullPath, ext))
            {
                TryDelete(fullPath);
                throw new InvalidOperationException(
                    "The uploaded image is incomplete or corrupted. Please upload it again.");
            }

            return (fileName, fullPath);
        }

        /// <summary>Sniff the real image type from its magic bytes.</summary>
        private static async Task<string?> DetectImageExtensionAsync(IFormFile file)
        {
            var header = new byte[12];
            await using var stream = file.OpenReadStream();
            var read = await stream.ReadAsync(header.AsMemory(0, header.Length));

            if (read >= 8
                && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
                && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
                return ".png";

            if (read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return ".jpg";

            if (read >= 12
                && header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
                && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50)
                return ".webp";

            return null;
        }

        /// <summary>Verify the file ends with the structure its format requires.</summary>
        private static bool IsCompleteImage(string path, string ext)
        {
            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (stream.Length < 12) return false;

                switch (ext)
                {
                    case ".png":
                    {
                        // Walk the chunk chain: every declared chunk must fit inside the
                        // file, and the chain must end at IEND. This catches a PNG that is
                        // cut off anywhere, not just one missing its final bytes.
                        Span<byte> chunk = stackalloc byte[8];
                        stream.Position = 8;
                        var isFirst = true;
                        while (stream.Position + 8 <= stream.Length)
                        {
                            if (stream.Read(chunk) != 8) return false;

                            if (isFirst)
                            {
                                // The first chunk must be IHDR.
                                if (chunk[4] != 0x49 || chunk[5] != 0x48 || chunk[6] != 0x44 || chunk[7] != 0x52)
                                    return false;
                                isFirst = false;
                            }

                            var length = ((uint)chunk[0] << 24) | ((uint)chunk[1] << 16)
                                       | ((uint)chunk[2] << 8) | chunk[3];
                            var isEnd = chunk[4] == 0x49 && chunk[5] == 0x45
                                     && chunk[6] == 0x4E && chunk[7] == 0x44;

                            // Chunk payload plus its CRC must be present.
                            if (stream.Position + length + 4 > stream.Length) return false;
                            if (isEnd) return true;

                            stream.Position += length + 4;
                        }
                        return false;
                    }
                    case ".jpg":
                    {
                        stream.Seek(-2, SeekOrigin.End);
                        return stream.ReadByte() == 0xFF && stream.ReadByte() == 0xD9;
                    }
                    case ".webp":
                    {
                        Span<byte> header = stackalloc byte[12];
                        if (stream.Read(header) != 12) return false;
                        var riffSize = BitConverter.ToUInt32(header.Slice(4, 4));
                        return riffSize + 8 <= stream.Length;
                    }
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void DeleteOtherLogoFiles(int accountId, string keepFileName)
        {
            var folder = LogoFolder;
            foreach (var pattern in new[] { $"app_logo_{accountId}.*", $"app_logo_{accountId}_*.*" })
            {
                foreach (var legacy in Directory.GetFiles(folder, pattern))
                {
                    if (!Path.GetFileName(legacy).Equals(keepFileName, StringComparison.OrdinalIgnoreCase))
                        TryDelete(legacy);
                }
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            catch
            {
                // Best-effort cleanup only.
            }
        }
    }
}
