﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
using OA.WebApi.Controllers;
using System.Threading.Tasks;

namespace OA.WebApi.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class SysConfigurationController : BaseController<SysConfigurationController, SysConfiguration, SysConfigurationCreateVModel, SysConfigurationUpdateVModel, SysConfigurationGetByIdVModel, SysConfigurationGetAllVModel>
    {
        private readonly ISysConfigurationService _sysConfigService;
        private readonly ILogger _logger;
        public SysConfigurationController(ISysConfigurationService sysConfigService, ILogger<SysConfigurationController> logger)
            : base(sysConfigService, logger)
        {
            _sysConfigService = sysConfigService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterSysConfigurationVModel model)
        {
            var result = await _sysConfigService.Search(model);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetByConfigTypeKey(string type, string key)
        {
            var result = await _sysConfigService.GetByConfigTypeKey(type, key);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] FilterSysConfigurationVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _sysConfigService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}