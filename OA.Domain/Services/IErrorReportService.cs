using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IErrorReportService
    {
        Task<ResponseResult> Search(FilterErrorReportVModel model);
        Task<ExportStream> ExportFile(FilterErrorReportVModel model, ExportFileVModel exportModel);
        Task<ResponseResult> GetById(int id);
        Task Create(ErrorReportCreateVModel model);
        Task Update(ErrorReportUpdateVModel model);
        Task Remove(int id);
    }
}
