using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System.Threading.Tasks;

namespace OA.Domain.Services
{
    public interface IBenefitService : IBaseService<Benefit, BenefitVModel, BenefitUpdateVModel, BenefitGetByIdVModel, BenefitGetAllVModel>
    {
        Task<ResponseResult> Search(FilterBenefitVModel model);
        Task<ExportStream> ExportFile(FilterBenefitVModel model, ExportFileVModel exportModel);
    }
}
