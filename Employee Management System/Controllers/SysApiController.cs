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
    public class SysApiController : BaseController<SysApiController, SysApi, SysApiCreateVModel, SysApiUpdateVModel, SysApiGetByIdVModel, SysApiGetAllVModel>
    {
        private readonly ISysApiService _sysApiService;
        private readonly ILogger<SysApiController> _logger;

        public SysApiController(ISysApiService sysApiService, ILogger<SysApiController> logger) : base(sysApiService, logger)
        {
            _sysApiService = sysApiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterSysAPIVModel model)
        {
            var response = await _sysApiService.Search(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] FilterSysAPIVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _sysApiService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}
