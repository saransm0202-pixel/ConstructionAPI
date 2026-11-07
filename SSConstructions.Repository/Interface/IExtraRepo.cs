using SSConstructions.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSConstructions.Repository.Interface
{
    public interface IExtraRepo
    {
        Task<List<ExtraModel>> GetExtras(int accountId);
        Task<ResponseMsg> SaveExtra(ExtraModel extraModel);
        Task<ResponseMsg> DeleteExtra(int extrasId);
    }
}
