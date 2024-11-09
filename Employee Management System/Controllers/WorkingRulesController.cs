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
    public class WorkingRulesController : BaseController<WorkingRulesController, WorkingRules, WorkingRulesCreateVModel, WorkingRulesUpdateVModel, WorkingRulesGetByIdVModel, WorkingRulesGetAllVModel>
    {
        private readonly IWorkingRulesService _workingrulesService;
        private readonly ILogger<WorkingRulesController> _logger;

        public WorkingRulesController(IWorkingRulesService workingrulesService, ILogger<WorkingRulesController> logger) : base(workingrulesService, logger)
        {
            _workingrulesService = workingrulesService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] WorkingRulesFilterVModel model)
        {
            var response = await _workingrulesService.Search(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] WorkingRulesFilterVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _workingrulesService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}
