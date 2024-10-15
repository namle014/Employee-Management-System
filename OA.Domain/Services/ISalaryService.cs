using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface ISalaryService
    {
        Task Create(SalaryCreateVModel model);
        Task Update(SalaryUpdateVModel model);
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> GetAll();
    }
}
