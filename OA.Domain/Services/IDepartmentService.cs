using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Domain.Services
{
    public interface IDepartmentService : IBaseService<Department, DepartmentCreateVModel, DepartmentUpdateVModel, DepartmentGetByIdVModel, DepartmentGetAllVModel>
    {
        Task<ResponseResult> Search(DepartmentFilterVModel model);
        Task<ExportStream> ExportFile(DepartmentFilterVModel model, ExportFileVModel exportModel);
        Task<ResponseResult> GetAllDepartments();
        Task ChangeStatusMany(DepartmentChangeStatusManyVModel model);
    }
}
