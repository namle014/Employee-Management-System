using OA.Core.Models;
using OA.Core.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IBenefitService
    {
        Task<ResponseResult> GetById(int id);
        Task<ResponseResult> Search(FilterBenefitVModel model);
        Task Create(BenefitCreateVModel model);
        Task Update(BenefitUpdateVModel model);
        Task ChangeStatus(int id);
        Task Remove(int id);
    }
}
