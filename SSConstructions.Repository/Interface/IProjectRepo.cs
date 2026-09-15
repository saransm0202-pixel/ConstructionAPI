using SSConstructions.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSConstructions.Repository.Interface
{
    public interface IProjectRepo
    {
        Task<List<ProjectModel>> GetProjects(int accountId);
        Task<List<ProjectModel>> GetHomeProjects(int accountId);
        Task<ResponseMsg> InsertUpdateProjects(ProjectModel projectModel);
        Task<ResponseMsg> SaveProject(ProjectModel projectModel);
        Task<ResponseMsg> UpdateProjectImage(int projectId, string imageUrl);
        Task<bool> ProjectExistsAsync(int projectId);
    }
}
