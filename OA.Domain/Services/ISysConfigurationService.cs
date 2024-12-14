using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Core.Services
{
    public interface ISysConfigurationService : IBaseService<SysConfiguration, SysConfigurationCreateVModel, SysConfigurationUpdateVModel, SysConfigurationGetByIdVModel, SysConfigurationGetAllVModel>
    {
        Task<ResponseResult> GetByConfigTypeKey(string type, string key);
        Task<ResponseResult> Search(FilterSysConfigurationVModel model);
        Task ChangeStatusMany(SysConfigurationChangeStatusManyVModel model);
        Task<ExportStream> ExportFile(FilterSysConfigurationVModel model, ExportFileVModel exportModel);
    }
}
