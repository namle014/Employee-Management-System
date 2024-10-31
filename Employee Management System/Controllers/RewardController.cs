﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OA.Core.Constants;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service;
using OA.Service.Helpers;
using OA.WebApi.Controllers;
using System;
using System.Threading.Tasks;

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
