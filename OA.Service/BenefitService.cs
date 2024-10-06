using AutoMapper;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Service
{
    public class BenefitService : BaseService<Benefit, BenefitVModel, BenefitUpdateVModel, BenefitGetByIdVModel, BenefitGetAllVModel, BenefitExportVModel>, IBenefitService
    {
        public BenefitService(IBaseRepository<Benefit> repository, IMapper mapper) : base(repository, mapper)
        {

        }

        public Task<ExportStream> ExportFile(FilterBenefitVModel model, ExportFileVModel exportModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseResult> Search(FilterBenefitVModel model)
        {
            throw new NotImplementedException();
        }
    }
}
