using System.Threading.Tasks;
using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;

namespace OA.Core.Services
{
    public interface IEmploymentContractService
    {
        Task<ResponseResult> Search(FilterEmploymentContractVModel model);
        Task<ExportStream> ExportFile(FilterEmploymentContractVModel model, ExportFileVModel exportModel);
        Task<ResponseResult> GetById(int id);
        Task Create(EmploymentContractCreateVModel model);
        Task Update(EmploymentContractUpdateVModel model);
        Task ChangeStatus(int id);
        Task Remove(int id);
        
    }
}
