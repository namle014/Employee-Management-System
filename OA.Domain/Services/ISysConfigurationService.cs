using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface ISysConfigurationService : IBaseService<SysConfiguration, SysConfigurationCreateVModel, SysConfigurationUpdateVModel, SysConfigurationGetByIdVModel, SysConfigurationGetAllVModel>
    {
        Task<ResponseResult> GetByConfigTypeKey(string type, string key);
    }
}
