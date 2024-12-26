using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IJobHistoryService
    {
        Task<ResponseResult> Search(FilterJobHistoryVModel model);
        Task<ExportStream> ExportFile(FilterJobHistoryVModel model, ExportFileVModel exportModel);
        Task<ResponseResult> GetById(int id);
        Task Create(JobHistoryCreateVModel model);
        Task Update(JobHistoryUpdateVModel model);
        Task Remove(int id);
    }
}
