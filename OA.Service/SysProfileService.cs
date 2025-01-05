using Microsoft.AspNetCore.Http;
using OA.Core.Services;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Repository;

namespace OA.Service
{
    public class SysProfileService : GlobalVariables, ISysProfileService
    {
        private readonly IAspNetUserService _userService;
        public SysProfileService(IAspNetUserService userService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _userService = userService;
        }

        public Task ChangePasswordProfile(UserChangePasswordVModel model)
        {
            return _userService.ChangePassword(model);
        }

        public Task UpdateProfile(UserUpdateVModel model)
        {
            model.Id = GlobalUserId ?? string.Empty;
            return _userService.Update(model);
        }
    }
}
