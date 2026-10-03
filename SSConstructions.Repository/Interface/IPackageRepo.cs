using SSConstructions.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSConstructions.Repository.Interface
{
    public interface IPackageRepo
    {
        Task<List<PackageModel>> GetPackages(int accountId);
        Task<List<ConstructionTypeModel>> GetConstructionTypes();
        Task<List<PackageSpecTypeModel>> GetPackageSpecTypes();
        Task<ResponseMsg> SavePackage(PackageModel packageModel);
        Task<ResponseMsg> DeletePackage(int packageId);
    }
}