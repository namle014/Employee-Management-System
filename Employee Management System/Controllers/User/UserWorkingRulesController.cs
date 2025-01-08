using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Infrastructure.EF.Entities;
namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteUser)]
    [ApiController]

    public class WorkingRulesController : ControllerBase
    {
        private readonly IWorkingRulesService _workingrulesService;
        private readonly ILogger<WorkingRulesController> _logger;

        public WorkingRulesController(IWorkingRulesService workingrulesService, ILogger<WorkingRulesController> logger)
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