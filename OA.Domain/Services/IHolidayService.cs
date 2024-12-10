using OA.Core.Models;
using OA.Core.VModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IHolidayService
    {
        Task Create(HolidayCreateVModel model);
        Task Update(HolidayUpdateVModel model);
        Task<ResponseResult> GetAll(HolidayFilterVModel model);
        Task Remove(int id);
        Task<ResponseResult> GetById(int id);
    }
}
