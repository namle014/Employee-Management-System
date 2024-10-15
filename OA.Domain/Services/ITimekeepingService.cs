using OA.Core.Models;
using OA.Core.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface ITimekeepingService
    {
        Task<ResponseResult> GetById(int id);
        Task<ResponseResult> Search(FilterTimekeepingVModel model);
        Task Create(TimekeepingCreateVModel model);
        Task Update(TimekeepingUpdateVModel model);
        Task ChangeStatus(int id);
        Task Remove(int id);
    }
}
