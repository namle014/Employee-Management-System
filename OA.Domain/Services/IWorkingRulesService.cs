using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Domain.Services
{
    public interface IWorkingRulesService : IBaseService<WorkingRules, WorkingRulesCreateVModel, WorkingRulesUpdateVModel, WorkingRulesGetByIdVModel, WorkingRulesGetAllVModel>
    {
        Task<ResponseResult> Search(WorkingRulesFilterVModel model);
        Task<ExportStream> ExportFile(WorkingRulesFilterVModel model, ExportFileVModel exportModel);
    }
}
