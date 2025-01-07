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

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetTotalRewardByEmployeeInMonth([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month <= 0 || month > 12)
            {
                return BadRequest("Year and month must be valid values.");
            }

            var response = await _rewardService.GetTotalRewardByEmployeeInMonth(year, month);
            return Ok(response);
        }

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetTotalRewards([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month <= 0 || month > 12)
            {
                return BadRequest("Year and month must be valid values.");
            }

            var response = await _rewardService.GetTotalRewards(year, month);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetRewardStatInYear([FromQuery] int year)
        {
            if (year <= 0)
            {
                return BadRequest("Year must be a valid value.");
            }

            var response = await _rewardService.GetRewardStatInYear(year);
            return Ok(response);
        }

    }
}
