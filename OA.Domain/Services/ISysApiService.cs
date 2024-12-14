using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Domain.Services
{
    public interface ISysApiService : IBaseService<SysApi, SysApiCreateVModel, SysApiUpdateVModel, SysApiGetByIdVModel, SysApiGetAllVModel>
    {
        Task<ResponseResult> Search(FilterSysAPIVModel model);
        Task<ExportStream> ExportFile(FilterSysAPIVModel model, ExportFileVModel exportModel);
    }
}
