using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Domain.Services
{
    public interface IDisciplineService : IBaseService<Discipline, DisciplineCreateVModel, DisciplineUpdateVModel, DisciplineGetByIdVModel, DisciplineGetAllVModel>
    {
        Task<ResponseResult> Search(DisciplineFilterVModel model);
        Task<ExportStream> ExportFile(DisciplineFilterVModel model, ExportFileVModel exportModel);

        Task<ResponseResult> GetTotalDisciplines(int years, int month);

        Task<ResponseResult> GetTotalDisciplineByEmployeeInMonth(int year, int month);

        Task<ResponseResult> GetDisciplineStatInYear(int year);
        Task<ResponseResult> GetMeDisciplineInfo(DisciplineFilterVModel model, int year);
    }
}
