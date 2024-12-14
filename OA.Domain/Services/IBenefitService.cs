using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface IBenefitService
    {
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> Search(FilterBenefitVModel model);
        Task Create(BenefitCreateVModel model);
        Task Update(BenefitUpdateVModel model);
        Task ChangeStatus(string id);
        Task Remove(string id);
        Task<ResponseResult> GetAll();
    }
}
