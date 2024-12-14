using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Domain.Services
{
    public interface IWorkShiftService : IBaseService<WorkShifts, WorkShiftCreateVModel, WorkShiftUpdateVModel, WorkShiftGetByIdVModel, WorkShiftGetAllVModel>
    {
        Task ChangeStatusMany(SysConfigurationChangeStatusManyVModel model);
        Task<ResponseResult> Search(FilterWorkShiftVModel model);
        Task<ExportStream> ExportFile(FilterWorkShiftVModel model, ExportFileVModel exportModel);
    }
}
