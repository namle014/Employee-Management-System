using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface INotificationsService
    {
        Task Create(NotificationsCreateVModel model);
        Task Update(NotificationsUpdateVModel model);
        Task UpdateIsNew(UserNotificationsUpdateIsNewVModel model);
        Task ChangeStatus(int id);
        Task ChangeRead(NotificationsUpdateReadVModel model);
        Task ChangeStatusForUser(NotificationsUpdateReadVModel model);
        Task ChangeAllRead(NotificationsUpdateAllReadVModel model);
        Task Remove(int id);
        Task<ResponseResult> GetCountIsNew(UserNotificationsUpdateIsNewVModel model);
        Task<ResponseResult> Search(FilterNotificationsVModel model);
        Task<ResponseResult> SearchForUser(FilterNotificationsForUserVModel model);
        Task<ResponseResult> GetById(int id);

    }
}
