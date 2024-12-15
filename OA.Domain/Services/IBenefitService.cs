using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;

namespace OA.Core.Services
{
    public interface IBenefitService
    {
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> GetAll(FilterBenefitVModel model);
        Task Create(BenefitCreateVModel model);
        Task Update(BenefitUpdateVModel model);
        Task ChangeStatus(string id);
        Task Remove(string id);
        Task ChangeStatusMany(BenefitChangeStatusManyVModel model);


        //Task<ResponseResult> GetAll();
    }
}
