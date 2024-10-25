using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface ITimeOffService : IBaseService<TimeOff, TimeOffCreateVModel, TimeOffUpdateVModel, TimeOffGetByIdVModel, TimeOffGetAllVModel>
    {
        Task<ResponseResult> Search(FilterTimeOffVModel model);
        Task<ExportStream> ExportFile(FilterTimeOffVModel model, ExportFileVModel exportModel);

    }
}
