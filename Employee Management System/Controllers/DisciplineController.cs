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
    public class DisciplineController : BaseController<DisciplineController, Discipline, DisciplineCreateVModel, DisciplineUpdateVModel, DisciplineGetByIdVModel, DisciplineGetAllVModel>
    {
        private readonly IDisciplineService _disciplineService;
        private readonly ILogger<DisciplineController> _logger;

        public DisciplineController(IDisciplineService discriplineService, ILogger<DisciplineController> logger) : base(discriplineService, logger)
        {
            _disciplineService = discriplineService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] DisciplineFilterVModel model)
        {
            var response = await _disciplineService.Search(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] DisciplineFilterVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _disciplineService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}
