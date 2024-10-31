using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface IRewardService : IBaseService<Reward, RewardCreateVModel, RewardUpdateVModel, RewardGetByIdVModel, RewardGetAllVModel>
    {
        Task<ResponseResult> Search(RewardFilterVModel model);
        Task<ExportStream> ExportFile(RewardFilterVModel model, ExportFileVModel exportModel);
    }
}
