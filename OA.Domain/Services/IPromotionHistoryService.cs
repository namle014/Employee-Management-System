using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;

namespace OA.Core.Services
{
    public interface IPromotionHistoryService
    {
        Task<ResponseResult> GetAll(GetAllPromotionHistory model);
        Task Create(CreatePromotionHistory model);
        Task Update(UpdatePromotionHistory model);
        //Task ChangeStatus(int id);
        Task<ResponseResult> GetByUserId(string id);

        Task<ResponseResult> GetPromotionHistoryByMonthAndYear(int year, int month);

        //Task Remove(string id);
        //Task ChangeStatusMany(BenefitChangeStatusManyVModel model);
        //Task<ResponseResult> GetAllBenefitType();
        //Task CreateBenefitType(BenefitTypeCreateVModel model);
        //Task CreateBenefitUser(CreateBenefitUser model);
        //Task UpdateBenefitType(BenefitTypeUpdateVModel model);
        //Task RemoveBenefitType(string id);
        //Task<ResponseResult> GetAll();
        //Task<ResponseResult> GetAllBenefitUser(GetAllBenefitUser model);
        //Task<ResponseResult> GetTotalBenefitAndEmployeeByMonthAndYear(int year, int month);
        //Task<ResponseResult> GetBenefitStatsByYears(int year);


    }
}
