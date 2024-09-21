using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;
using OA.WebApi.Controllers;

namespace OA.WebAPI.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class UnitController : BaseController<UnitController, Unit, UnitCreateVModel, UnitUpdateVModel, UnitGetByIdVModel, UnitGetAllVModel>
    {
        private readonly IUnitService _unitService;
        private readonly ILogger<UnitController> _logger;

        public UnitController(IUnitService unitService, ILogger<UnitController> logger) : base(unitService, logger)
        {
            _unitService = unitService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterGetAllVModel model)
        {
            var response = await _unitService.Search(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] FilterGetAllVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type = exportModel.Type?.ToUpper();

            var content = await _unitService.ExportFile(model, exportModel);

            if (content?.Stream == null || string.IsNullOrEmpty(content.ContentType) || string.IsNullOrEmpty(content.FileName))
            {
                return BadRequest("File content is invalid or not found.");
            }

            return File(content.Stream, content.ContentType, content.FileName);
        }

    }
}
