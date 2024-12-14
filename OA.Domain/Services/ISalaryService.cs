using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface ISalaryService
    {
        Task Create(SalaryCreateVModel model);
        Task Update(SalaryUpdateVModel model);
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> GetAll();
        Task<ResponseResult> Search(FilterSalaryVModel model);
        Task Remove(string id);
        Task ChangeStatus(string id);
        Task<ResponseResult> GetIncomeInMonth(int year, int month);
        Task<ResponseResult> GetYearIncome(int year);
    }
}
