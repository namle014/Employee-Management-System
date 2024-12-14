using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.WebApi.Controllers;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class WorkShiftController : BaseController<WorkShiftController, WorkShifts, WorkShiftCreateVModel, WorkShiftUpdateVModel, WorkShiftGetByIdVModel, WorkShiftGetAllVModel>
    {
        private readonly IWorkShiftService _workShiftService;
        private readonly ILogger<WorkShiftController> _logger;

        public WorkShiftController(IWorkShiftService workShiftService, ILogger<WorkShiftController> logger) : base(workShiftService, logger)
        {
            _workShiftService = workShiftService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterWorkShiftVModel model)
        {
            var response = await _workShiftService.Search(model);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> ChangeStatusMany(SysConfigurationChangeStatusManyVModel model)
        {
            await _workShiftService.ChangeStatusMany(model);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] FilterWorkShiftVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _workShiftService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}
