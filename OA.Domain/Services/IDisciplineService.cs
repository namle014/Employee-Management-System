using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface IDisciplineService : IBaseService<Discipline, DisciplineCreateVModel, DisciplineUpdateVModel, DisciplineGetByIdVModel, DisciplineGetAllVModel>
    {
        Task<ResponseResult> Search(DisciplineFilterVModel model);
        Task<ExportStream> ExportFile(DisciplineFilterVModel model, ExportFileVModel exportModel);
    }
}
