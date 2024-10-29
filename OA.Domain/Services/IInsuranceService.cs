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
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> Search(FilterInsuranceVModel model);
        Task Create(InsuranceCreateVModel model);
        Task Update(InsuranceUpdateVModel model);
        Task ChangeStatus(string id);
        Task Remove(string id);
        Task<ResponseResult> GetAll();
    }
}
