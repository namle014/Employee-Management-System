using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels.Role;
namespace OA.Core.Services
{
    public interface IAspNetRoleService
    {
        Task<ResponseResult> GetAll(FiltersGetAllByQueryStringRoleVModel model);
        Task<ResponseResult> GetById(string id);
        Task CheckValidRoleName(string roleName);
        Task<ResponseResult> GetJsonHasFunctionByRoleId(string id);
        Task UpadateJsonHasFunctionByRoleId(UpadateJsonHasFunctionByRoleIdVModel model);
        Task Create(AspNetRoleCreateVModel model);
        Task Update(AspNetRoleUpdateVModel model);
        Task ChangeStatus(string id);
        Task Remove(string id);
        Task<ExportStream> ExportFile(FiltersGetAllByQueryStringRoleVModel model, ExportFileVModel exportModel);
    }
}
