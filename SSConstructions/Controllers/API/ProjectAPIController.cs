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
    public class ProjectAPIController : Controller
    {
        private readonly IProjectRepo _projectRepo;
        private readonly CustomLogger _logger;
        private readonly IWebHostEnvironment _env;

        public ProjectAPIController(IProjectRepo projectRepo, CustomLogger logger, IWebHostEnvironment env)
        {
            _projectRepo = projectRepo;
            _logger = logger;
            _env = env;
        }
        [Route("getProjects")]
        [HttpGet()]
        public async Task<IActionResult> GetProjects(int accountId)
        {
            try
            {
                _logger.Log("ProjectAPIController.GetProjects Started.");
                return Ok(await _projectRepo.GetProjects(accountId));
                _logger.Log("ProjectAPIController.GetProjects Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
        [Route("getHomeProjects")]
        [HttpGet()]
        public async Task<IActionResult> GetHomeProjects(int accountId)
        {
            try
            {
                _logger.Log("ProjectAPIController.GetHomeProjects Started.");
                return Ok(await _projectRepo.GetHomeProjects(accountId));
                _logger.Log("ProjectAPIController.GetHomeProjects Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
        [Route("InsertUpdateProject")]
        [HttpPost()]
        public async Task<IActionResult> InsertUpdateProject(ProjectModel projectModel)
        {
            try
            {
                _logger.Log("ProjectAPIController.InsertUpdateProject Started.");
                return Ok(await _projectRepo.InsertUpdateProjects(projectModel));
                _logger.Log("ProjectAPIController.InsertUpdateProject Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
        [Route("SaveProject")]
        [HttpPost()]
        public async Task<IActionResult> SaveProject([FromForm] ProjectModel projectModel, IFormFile? imageFile)
        {
            try
            {
                _logger.Log("ProjectAPIController.SaveProject Started.");
                var response = await _projectRepo.SaveProject(projectModel);

                if (response.StatusCode == 1 && imageFile != null && response.Id > 0)
                {
                    var savedFile = await SaveImageAsync(response.Id, imageFile);
                    var url = $"{Request.Scheme}://{Request.Host}/uploads/projects/{savedFile}";
                    await _projectRepo.UpdateProjectImage(response.Id, url);
                    // Always store the canonical URL so a single image is kept per project.
                    response.Message += " · Image saved";
                }
                _logger.Log("ProjectAPIController.SaveProject Completed.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("UploadProjectImage")]
        [HttpPost()]
        public async Task<IActionResult> UploadProjectImage([FromForm] int projectId, [FromForm] IFormFile imageFile)
        {
            try
            {
                _logger.Log("ProjectAPIController.UploadProjectImage Started.");
                if (imageFile == null || imageFile.Length == 0)
                    return BadRequest("No image provided.");

                if (!await _projectRepo.ProjectExistsAsync(projectId))
                    return BadRequest("Project Not Found");

                var savedFile = await SaveImageAsync(projectId, imageFile);
                var url = $"{Request.Scheme}://{Request.Host}/uploads/projects/{savedFile}";

                var update = await _projectRepo.UpdateProjectImage(projectId, url);
                if (update.StatusCode != 1)
                    return BadRequest(update.Message);

                _logger.Log("ProjectAPIController.UploadProjectImage Completed.");
                return Ok(new { StatusCode = 1, ImageUrl = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        private async Task<string> SaveImageAsync(int projectId, IFormFile imageFile)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads", "projects");
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) ext = ".png";
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".webp")
                ext = ".png";

            // Deterministic single-image-per-project filename (overwrites previous).
            var fileName = $"project_{projectId}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Remove any legacy project image variants so only one image remains per project.
            foreach (var legacy in Directory.GetFiles(folder, $"project_{projectId}.*"))
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
