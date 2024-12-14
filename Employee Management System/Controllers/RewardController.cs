using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Infrastructure.EF.Entities;
using OA.WebApi.Controllers;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class RewardController : BaseController<RewardController, Reward, RewardCreateVModel, RewardUpdateVModel, RewardGetByIdVModel, RewardGetAllVModel>
    {
        private readonly IRewardService _rewardService;
        private readonly ILogger<RewardController> _logger;

        public RewardController(IRewardService sysApiService, ILogger<RewardController> logger) : base(sysApiService, logger)
        {
            _rewardService = sysApiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] RewardFilterVModel model)
        {
            var response = await _rewardService.Search(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] RewardFilterVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _rewardService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}
