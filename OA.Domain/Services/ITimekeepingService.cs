using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface ITimekeepingService
    {
        Task CheckOut(CheckOutVModel model);
        Task<ResponseResult> GetById(int id);
        Task<ResponseResult> Search(FilterTimekeepingVModel model);
        Task<ResponseResult> GetAllDepartments();
        Task Create(TimekeepingCreateVModel model);
        Task Update(TimekeepingUpdateVModel model);
        Task ChangeStatus(int id);
        Task Remove(int id);
        Task<ResponseResult> SearchForUser(FilterTimekeepingForUserVModel model);
        Task<ResponseResult> CreateUser(TimekeepingCreateUserVModel model);
        Task<ResponseResult> GetByDate(FilterTimekeepingGetByDateVModel model);
    }
}
