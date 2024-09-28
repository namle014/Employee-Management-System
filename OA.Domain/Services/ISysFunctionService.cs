using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface ISysFunctionService : IBaseService<SysFunction, SysFunctionCreateVModel, SysFunctionUpdateVModel, SysFunctionGetByIdVModel, SysFunctionGetAllVModel>
    {
        Task<ResponseResult> GetJsonAPIFunctionId(int id, string type);
        Task UpadateJsonAPIFunctionId(UpadateJsonAPIFunctionIdVModel model);
        Task<ResponseResult> GetAll(FilterSysFunctionVModel model);
        Task<ResponseResult> GetAllAsTree(FilterSysFunctionVModel model);
    }
}
