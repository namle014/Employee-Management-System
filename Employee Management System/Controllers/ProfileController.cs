using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Domain.Services;
using OA.Domain.VModels;

namespace OA.WebAPI.Controllers
{
    [Route(CommonConstants.Routes.BaseRoute)]
    public class ProfileController : Controller
    {
        private readonly ISysProfileService _profileService;
        public ProfileController(ISysProfileService profileService, ILogger<AuthController> logger)
        {
            _profileService = profileService;
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ChangePasswordProfile([FromBody] UserChangePasswordVModel model)
        {
            await _profileService.ChangePasswordProfile(model);
            return NoContent();
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateVModel model)
        {
            await _profileService.UpdateProfile(model);
            return NoContent();
        }
    }
}
