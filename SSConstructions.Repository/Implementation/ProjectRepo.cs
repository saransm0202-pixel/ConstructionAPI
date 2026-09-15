using Microsoft.EntityFrameworkCore;
using SSConstructions.Repository.Entity;
using SSConstructions.Repository.Entity.EntityClass;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;

namespace SSConstructions.Repository.Implementation
{
    public class ProjectRepo : IProjectRepo
    {
        private readonly ConstructionDbContext _context;
        private readonly CustomLogger _logger;
        public ProjectRepo(ConstructionDbContext context, CustomLogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProjectModel>> GetProjects(int accountId)
        {
            List<ProjectModel> projects = new List<ProjectModel>();
            try
            {
                _logger.Log("Users.GetProjects Started.");
                projects = await (from project in _context.Projects.Where(p => p.AccountId == accountId)
                                   select new ProjectModel
                                   {
                                       ProjectId = project.ProjectId,
                                       ProjectName = project.ProjectName,
                                       Description = project.Description,
                                       ImageUrl = project.ImageUrl,
                                       ProjectStatus = project.ProjectStatus,
                                       ProjectLocation = project.ProjectLocation,
                                       ProjectType = project.ProjectType,
                                       ProjectArea = project.ProjectArea,
                                       IsActive = project.IsActive
                                   }).ToListAsync();
                _logger.Log("Users.GetProjects Completed.");
                return projects;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<ProjectModel>> GetHomeProjects(int accountId)
        {
            List<ProjectModel> projects = new List<ProjectModel>();
            try
            {
                _logger.Log("Projects.GetHomeProjects Started.");
                projects = await (from project in _context.Projects
                                   .Where(p => p.AccountId == accountId && p.IsActive == true)
                                   .OrderByDescending(p => p.ProjectId)
                                   .Take(3)
                                   select new ProjectModel
                                   {
                                       ProjectId = project.ProjectId,
                                       ProjectName = project.ProjectName,
                                       Description = project.Description,
                                       ImageUrl = project.ImageUrl,
                                       ProjectStatus = project.ProjectStatus,
                                       ProjectLocation = project.ProjectLocation,
                                       ProjectType = project.ProjectType,
                                       ProjectArea = project.ProjectArea,
                                       IsActive = project.IsActive
                                   }).ToListAsync();
                _logger.Log("Projects.GetHomeProjects Completed.");
                return projects;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseMsg> InsertUpdateProjects(ProjectModel projectModel)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("Projects.InsertUpdateProject Started.");
                if (projectModel != null)
                {
                    if (projectModel.ProjectId > 0)
                    {
                        var selectedProject = await _context.Projects.Where(p => p.ProjectId == projectModel.ProjectId).FirstOrDefaultAsync();
                        selectedProject.ProjectName = projectModel.ProjectName;
                        selectedProject.Description = projectModel.Description;
                        selectedProject.ImageUrl = projectModel.ImageUrl;
                        selectedProject.ProjectStatus = projectModel.ProjectStatus;
                        selectedProject.ProjectLocation = projectModel.ProjectLocation;
                        selectedProject.ProjectType = projectModel.ProjectType;
                        selectedProject.ProjectArea = projectModel.ProjectArea;
                        selectedProject.IsActive = projectModel.IsActive;
                        selectedProject.AccountId = projectModel.AccountId;
                        selectedProject.CreatedDate = DateTime.Now;
                        responseMsg.Message = "Project Updated Successfully";
                    }
                    else
                    {
                        Project project = new Project
                        {
                            ProjectName = projectModel.ProjectName,
                            Description = projectModel.Description,
                            ImageUrl = projectModel.ImageUrl,
                            ProjectStatus = projectModel.ProjectStatus,
                            ProjectLocation = projectModel.ProjectLocation,
                            ProjectType = projectModel.ProjectType,
                            ProjectArea = projectModel.ProjectArea,
                            IsActive = projectModel.IsActive,
                            AccountId = projectModel.AccountId,
                            ModifiedDate = DateTime.Now
                        };
                        await _context.Projects.AddAsync(project);
                        responseMsg.Message = "Project Added Successfully";
                    }
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                }
                _logger.Log("Projects.InsertUpdateProject Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseMsg> SaveProject(ProjectModel projectModel)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("Projects.SaveProject Started.");
                if (projectModel != null)
                {
                    if (projectModel.ProjectId > 0)
                    {
                        var selectedProject = await _context.Projects.Where(p => p.ProjectId == projectModel.ProjectId).FirstOrDefaultAsync();
                        if (selectedProject != null)
                        {
                            selectedProject.ProjectName = projectModel.ProjectName;
                            selectedProject.Description = projectModel.Description;
                            selectedProject.ImageUrl = projectModel.ImageUrl;
                            selectedProject.ProjectStatus = projectModel.ProjectStatus;
                            selectedProject.ProjectLocation = projectModel.ProjectLocation;
                            selectedProject.ProjectType = projectModel.ProjectType;
                            selectedProject.ProjectArea = projectModel.ProjectArea;
                            selectedProject.IsActive = projectModel.IsActive;
                            selectedProject.AccountId = projectModel.AccountId;
                            responseMsg.Id = selectedProject.ProjectId;
                            responseMsg.Message = "Project Updated Successfully";
                        }
                    }
                    else
                    {
                        Project project = new Project
                        {
                            ProjectName = projectModel.ProjectName,
                            Description = projectModel.Description,
                            ImageUrl = projectModel.ImageUrl,
                            ProjectStatus = projectModel.ProjectStatus,
                            ProjectLocation = projectModel.ProjectLocation,
                            ProjectType = projectModel.ProjectType,
                            ProjectArea = projectModel.ProjectArea,
                            IsActive = projectModel.IsActive,
                            AccountId = projectModel.AccountId
                        };
                        await _context.Projects.AddAsync(project);
                        await _context.SaveChangesAsync();
                        responseMsg.Id = project.ProjectId;
                        responseMsg.Message = "Project Added Successfully";
                    }
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                }
                _logger.Log("Projects.SaveProject Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    public async Task<ResponseMsg> UpdateProjectImage(int projectId, string imageUrl)
        {
            ResponseMsg responseMsg = new ResponseMsg();
            try
            {
                _logger.Log("Projects.UpdateProjectImage Started.");
                var selectedProject = await _context.Projects.Where(p => p.ProjectId == projectId).FirstOrDefaultAsync();
                if (selectedProject != null)
                {
                    selectedProject.ImageUrl = imageUrl;
                    await _context.SaveChangesAsync();
                    responseMsg.StatusCode = 1;
                    responseMsg.Id = selectedProject.ProjectId;
                    responseMsg.Message = "Project Image Updated Successfully";
                }
                else
                {
                    responseMsg.Message = "Project Not Found";
                }
                _logger.Log("Projects.UpdateProjectImage Completed.");
                return responseMsg;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    public async Task<bool> ProjectExistsAsync(int projectId)
        {
            return await _context.Projects.AnyAsync(p => p.ProjectId == projectId);
        }
    }
}
