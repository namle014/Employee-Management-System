using Microsoft.AspNetCore.Mvc;
using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Domain.VModels.Role;
using System.Threading.Tasks;
namespace OA.Core.Services
{
    public interface IAspNetUserService
    {
        Task<ResponseResult> GetAll(UserFilterVModel model);
        Task RequestPasswordReset(string emailUser);
        Task ResetPassword(ResetPasswordModel model);
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> GetJsonUserHasFunctions(string userId);
        Task Create(UserCreateVModel model);
        Task Update(UserUpdateVModel model);
        Task UpdateJsonUserHasFunctions(UpdatePermissionVModel model);
        Task ChangeStatus(string id);
        Task Remove(string id);
        Task UpdateRoleForUser(UpdateRoleModel model);
        Task CheckValidUserName(string userName);
        Task CheckValidEmail(string email);
        Task<ResponseResult> SendMail(string toEmail, string userId);
        bool ConfirmAccount(ConfirmAccount model);
        Task ChangePassword(UserChangePasswordVModel model);
    }
}
