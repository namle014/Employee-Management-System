using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface IInsuranceService
    {
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> Search(FilterInsuranceVModel model);
        Task Create(InsuranceCreateVModel model);
        Task Update(InsuranceUpdateVModel model);
        Task ChangeStatus(string id);
        Task Remove(string id);
        Task<ResponseResult> GetAll();
    }
}
