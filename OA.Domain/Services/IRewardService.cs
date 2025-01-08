using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Domain.Services
{
    public interface IRewardService : IBaseService<Reward, RewardCreateVModel, RewardUpdateVModel, RewardGetByIdVModel, RewardGetAllVModel>
    {
        Task<ResponseResult> Search(RewardFilterVModel model);
        Task<ExportStream> ExportFile(RewardFilterVModel model, ExportFileVModel exportModel);

        Task<ResponseResult> GetTotalRewards(int years, int month);

        Task<ResponseResult> GetTotalRewardByEmployeeInMonth(int year, int month);

        Task<ResponseResult> GetRewardStatInYear(int year);
        Task<ResponseResult> GetMeRewardInfo(RewardFilterVModel model, int year);
    }
}
