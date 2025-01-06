using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface IEventService
    {
        Task Create(EventCreateVModel model);
        Task<ResponseResult> StatEventByYear(int year);
        Task<ResponseResult> TotalEventsByMonth(int month, int year);
        Task Update(EventUpdateVModel model);
        Task<ResponseResult> GetAll(EventFilterVModel model);
        //Task DeleteMany(EventDeleteManyVModel model);
        Task Remove(int id);
        Task<ResponseResult> GetById(int id);
    }
}
