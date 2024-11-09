using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface IDepartmentService : IBaseService<Department, DepartmentCreateVModel, DepartmentUpdateVModel, DepartmentGetByIdVModel, DepartmentGetAllVModel>
    {
        Task<ResponseResult> Search(DepartmentFilterVModel model);
        Task<ExportStream> ExportFile(DepartmentFilterVModel model, ExportFileVModel exportModel);
    }
}
