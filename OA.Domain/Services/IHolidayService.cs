using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface IHolidayService
    {
        Task Create(EventCreateVModel model);
        Task Update(EventUpdateVModel model);
        Task<ResponseResult> GetAll(EventFilterVModel model);
        Task DeleteMany(HolidayDeleteManyVModel model);
        Task Remove(int id);
        Task<ResponseResult> GetById(int id);
    }
}
