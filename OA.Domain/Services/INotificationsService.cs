using OA.Core.Models;
using OA.Core.VModels;

namespace OA.Core.Services
{
    public interface INotificationsService
    {
        Task Create(NotificationsCreateVModel model);
        Task<ResponseResult> StatNotificationByMonth(int month, int year);
        Task Update(NotificationsUpdateVModel model);
        Task UpdateIsNew();
        Task ChangeStatus(int id);
        Task ChangeRead(NotificationsUpdateReadVModel model);
        Task ChangeStatusForUser();
        Task ChangeAllRead();
        Task Remove(int id);
        Task<ResponseResult> GetCountIsNew();
        Task<ResponseResult> Search(FilterNotificationsVModel model);
        Task<ResponseResult> SearchForUser(FilterNotificationsForUserVModel model);
        Task<ResponseResult> GetById(int id);
        Task<ResponseResult> StatNotificationByType(int year);
        Task<ResponseResult> CountNotifyReadByUser(FilterCountNotifyReadByUser model);
    }
}
