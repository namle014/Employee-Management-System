using OA.Core.Models;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IUnitService : IBaseService<Unit, UnitCreateVModel, UnitUpdateVModel, UnitGetByIdVModel, UnitGetAllVModel>
    {
        Task<ResponseResult> Search(FilterGetAllVModel model);
        Task<ExportStream> ExportFile(FilterGetAllVModel model, ExportFileVModel exportModel);
    }
}
