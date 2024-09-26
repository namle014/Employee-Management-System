using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
using OA.WebApi.Controllers;
using System.Net;
using System.Threading.Tasks;

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

        [HttpGet]
        public async Task<IActionResult> GetAllByType([FromQuery] string fileType, [FromQuery] int pageSize = CommonConstants.ConfigNumber.pageSizeDefault, [FromQuery] int pageNumber = 1)
        {
            ObjectResult objecrResult;
            if (string.IsNullOrWhiteSpace(fileType))
            {
                objecrResult = new BadRequestObjectResult(CommonConstants.Validate.inputInvalid);
            }
            else
            {
                ResponseResult response = await _sysFileService.GetAllByType(fileType, pageSize, pageNumber);
                if (response.Success)
                {
                    objecrResult = new ObjectResult(response)
                    {
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
                else
                {
                    objecrResult = new ObjectResult(response)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                    _logger.LogWarning(CommonConstants.LoggingEvents.CreateItem,
                        string.Format(MsgConstants.ErrorMessages.ErrorCreate));
                }
            }
            return objecrResult;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageBase64([FromBody] SysFileCreateBase64VModel model)
        {
            ObjectResult result;
            if (!ModelState.IsValid)
            {
                result = new BadRequestObjectResult(ModelState);
            }
            else
            {
                var responseResult = await _sysFileService.CreateBase64(model);
                result = new ObjectResult(responseResult);
            }
            return result;
        }

        [HttpPost]
        public async Task FileChunks([FromForm] FileChunk fileChunk)
        {
            if (string.IsNullOrWhiteSpace(fileChunk.FileName))
            {
                throw new BadRequestException(CommonConstants.Validate.inputInvalid);
            }
            else
            {
                await _sysFileService.FileChunks(fileChunk);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterSysFileVModel model)
        {
            var response = await _sysFileService.GetAll(model);
            var result = new ObjectResult(response);
            if (!response.Success)
            {
                _logger.LogWarning(CommonConstants.LoggingEvents.GetItem, MsgConstants.ErrorMessages.ErrorGetById, _nameController);
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar([FromForm] FileChunk fileChunk)
        {
            if (string.IsNullOrWhiteSpace(fileChunk.FileName))
            {
                return new BadRequestObjectResult(CommonConstants.Validate.inputInvalid);
            }

            var response = await _sysFileService.UploadAvatar(fileChunk);
            var result = new ObjectResult(response);

            if (response.Success)
            {
                return result;
            }
            else
            {
                _logger.LogWarning(CommonConstants.LoggingEvents.CreateItem, string.Format(MsgConstants.ErrorMessages.ErrorCreate));
                return new BadRequestObjectResult(response);
            }
        }

        [HttpDelete(CommonConstants.Routes.Url)]
        public async Task<IActionResult> RemoveByUrl([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new BadRequestObjectResult(CommonConstants.Validate.inputInvalid);
            }
            var response = await _sysFileService.RemoveByUrl(url);
            return new ObjectResult(response);
        }
    }
}
