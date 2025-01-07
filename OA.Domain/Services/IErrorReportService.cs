using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IErrorReportService
    {
        Task<ResponseResult> Search(FilterErrorReportVModel model);
        Task<ResponseResult> SearchByUserId(FilterErrorReportVModel model);
        Task<ResponseResult> CountErrorReportsByStatusAndMonth(int year);
        Task<ResponseResult> CountErrorReportsInMonth(int year, int month);
        Task<ResponseResult> CountErrorReportsInMonthUser(int year, int month);
        Task<ResponseResult> CountErrorReportsByTypeAndYear(int year);
        Task<ResponseResult> CountErrorReportsByTypeAndYearUser(int year);
        Task<ExportStream> ExportFile(FilterErrorReportVModel model, ExportFileVModel exportModel);
        Task<ResponseResult> GetById(int id);
        Task Create(ErrorReportCreateVModel model);
        Task Update(ErrorReportUpdateVModel model);
        Task Remove(int id);
    }
}
