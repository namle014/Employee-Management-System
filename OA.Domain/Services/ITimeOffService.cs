using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;

namespace OA.Core.Services
{
    public interface ITimeOffService
    {
        Task<ResponseResult> Search(FilterTimeOffVModel model);
        Task<ResponseResult> CountTimeOffsInMonth(int year, int month);
        Task<ResponseResult> GetPendingFutureTimeOffs(DateTime fromDate);
        Task<ExportStream> ExportFile(FilterTimeOffVModel model, ExportFileVModel exportModel);
        Task<ResponseResult> GetById(int id);
        Task Create(TimeOffCreateVModel model);
        Task Update(TimeOffUpdateVModel model);
        Task ChangeStatus(int id);
        Task Remove(int id);
        
    }
}
