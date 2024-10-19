using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface IEmploymentContractService : IBaseService<EmploymentContract, EmploymentContractCreateVModel, EmploymentContractUpdateVModel, EmploymentContractGetByIdVModel, EmploymentContractGetAllVModel>
    {
        Task<ResponseResult> Search(FilterEmploymentContractVModel model);
        Task<ExportStream> ExportFile(FilterEmploymentContractVModel model, ExportFileVModel exportModel);
    }
}
