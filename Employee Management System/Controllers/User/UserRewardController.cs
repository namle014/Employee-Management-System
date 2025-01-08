using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.WebApi.Controllers;

namespace Employee_Management_System.Controllers.User
{
    [Route(CommonConstants.Routes.BaseRouteUser)]
    [ApiController]
    public class UserRewardController :ControllerBase
    {
        private readonly IRewardService _service;
        private readonly ILogger _logger;
        protected static string _nameController = "Reward";
        public UserRewardController(IRewardService service, ILogger<UserRewardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMeRewardInfo(RewardFilterVModel model, int year)
        {
            var response = await _service.GetMeRewardInfo(model, year);
            return Ok(response);
        }
    }
}
