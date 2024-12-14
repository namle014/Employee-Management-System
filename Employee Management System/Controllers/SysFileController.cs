using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
using OA.WebApi.Controllers;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class SysFileController : BaseController<SysFileController, SysFile, SysFileCreateVModel, SysFileUpdateVModel, SysFileGetByIdVModel, SysFileGetAllVModel>
    {
        private readonly ISysFileService _sysFileService;
        private readonly ILogger<SysFileController> _logger;
        public SysFileController(ISysFileService sysFileService, ILogger<SysFileController> logger) : base(sysFileService, logger)
        {
            _sysFileService = sysFileService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile([FromBody] SysFileCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            var result = await _sysFileService.CreateFile(model);
            return Ok(result);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)] // Ẩn API này khỏi Swagger
        public override async Task<IActionResult> Create([FromBody] SysFileCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _sysFileService.Create(model);

            return Created();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageBase64([FromBody] SysFileCreateBase64VModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _sysFileService.CreateBase64(model);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> FileChunks([FromForm] FileChunk fileChunk)
        {
            if (string.IsNullOrWhiteSpace(fileChunk.FileName))
            {
                throw new BadRequestException(CommonConstants.Validate.inputInvalid);
            }

            var result = await _sysFileService.FileChunks(fileChunk);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterSysFileVModel model)
        {
            var result = await _sysFileService.GetAll(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar([FromForm] FileChunk fileChunk)
        {
            if (string.IsNullOrWhiteSpace(fileChunk.FileName))
            {
                return new BadRequestObjectResult(CommonConstants.Validate.inputInvalid);
            }

            await _sysFileService.UploadAvatar(fileChunk);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveByUrl([FromQuery] string url)
        {
            await _sysFileService.RemoveByUrl(url);
            return NoContent();
        }
    }
}
