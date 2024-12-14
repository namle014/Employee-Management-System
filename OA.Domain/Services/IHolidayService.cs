using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface IHolidayService
    {
        Task Create(HolidayCreateVModel model);
        Task Update(HolidayUpdateVModel model);
        Task<ResponseResult> GetAll(HolidayFilterVModel model);
        Task DeleteMany(HolidayDeleteManyVModel model);
        Task Remove(int id);
        Task<ResponseResult> GetById(int id);
    }
}
