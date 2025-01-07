using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Infrastructure.EF.Entities;
using OA.Service;
using OA.WebApi.Controllers;

namespace OA.WebAPI.AdminControllers
{
    [Authorize(Policy = CommonConstants.Authorize.CustomAuthorization)]
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

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetTotalDisciplineByEmployeeInMonth([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month <= 0 || month > 12)
            {
                return BadRequest("Year and month must be valid values.");
            }

            var response = await _disciplineService.GetTotalDisciplineByEmployeeInMonth(year, month);
            return Ok(response);
        }

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetTotalDisciplines([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month <= 0 || month > 12)
            {
                return BadRequest("Year and month must be valid values.");
            }

            var response = await _disciplineService.GetTotalDisciplines(year, month);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetDisciplineStatInYear([FromQuery] int year)
        {
            if (year <= 0)
            {
                return BadRequest("Year must be a valid value.");
            }

            var response = await _disciplineService.GetDisciplineStatInYear(year);
            return Ok(response);
        }
    }
}
