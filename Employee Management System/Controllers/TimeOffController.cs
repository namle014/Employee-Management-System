using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    public class TimeOffController : BaseController<TimeOffController, TimeOff, TimeOffCreateVModel, TimeOffUpdateVModel, TimeOffGetByIdVModel, TimeOffGetAllVModel>
    {
        private readonly ITimeOffService _TimeOffService;
        private readonly ILogger<TimeOffController> _logger;

        public TimeOffController(ITimeOffService TimeOffService, ILogger<TimeOffController> logger) : base(TimeOffService, logger)
        {
            _TimeOffService = TimeOffService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterTimeOffVModel model)
        {
            var response = await _TimeOffService.Search(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] FilterTimeOffVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _TimeOffService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}