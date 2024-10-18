using OA.Core.Models;
using OA.Core.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IInsuranceService
    {
        Task<ResponseResult> GetById(int id);
        Task<ResponseResult> Search(FilterInsuranceVModel model);
        Task Create(InsuranceCreateVModel model);
        Task Update(InsuranceUpdateVModel model);
        Task ChangeStatus(int id);
        Task Remove(int id);
    }
}
