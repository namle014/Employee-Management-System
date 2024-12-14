using OA.Core.Models;
using OA.Core.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Domain.Services
{
    public interface ISysFunctionService : IBaseService<SysFunction, SysFunctionCreateVModel, SysFunctionUpdateVModel, SysFunctionGetByIdVModel, SysFunctionGetAllVModel>
    {
        //Task<ResponseResult> GetJsonAPIFunctionId(int id, string type);
        //Task UpadateJsonAPIFunctionId(UpadateJsonAPIFunctionIdVModel model);
        Task<ResponseResult> GetAll(FilterSysFunctionVModel model);
        Task<ResponseResult> GetAllAsTree(FilterSysFunctionVModel model);
    }
}
