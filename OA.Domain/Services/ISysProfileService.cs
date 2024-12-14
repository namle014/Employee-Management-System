using OA.Domain.VModels;

namespace OA.Domain.Services
{
    public interface ISysProfileService
    {
        Task ChangePasswordProfile(UserChangePasswordVModel model);
        Task UpdateProfile(UserUpdateVModel model);
    }
}
