using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.WebApi.Controllers;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class SysFunctionController : BaseController<SysFunctionController, SysFunction, SysFunctionCreateVModel, SysFunctionUpdateVModel, SysFunctionGetByIdVModel, SysFunctionGetAllVModel>
    {
        private readonly ISysFunctionService _sysFunctionService;
        private readonly ILogger<SysFunctionController> _logger;
        public SysFunctionController(ISysFunctionService sysFunctionService, ILogger<SysFunctionController> logger) : base(sysFunctionService, logger)
        {
            _sysFunctionService = sysFunctionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterSysFunctionVModel model)
        {
            var response = await _sysFunctionService.GetAll(model);
            if (!response.Success)
            {
                _logger.LogWarning(CommonConstants.LoggingEvents.GetItem, MsgConstants.ErrorMessages.ErrorGetById, _nameController);
            }
            return Ok(response);
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAllAsTree([FromQuery] FilterSysFunctionVModel model)
        {
            var response = await _sysFunctionService.GetAllAsTree(model);
            return Ok(response);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetJsonAPIFunctionbyId([FromQuery] int id, [FromQuery] string type)
        //{
        //    if (id <= 0)
        //    {
        //        return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
        //    }
        //    var response = await _sysFunctionService.GetJsonAPIFunctionId(id, type);
        //    if (!response.Success)
        //    {
        //        _logger.LogWarning(CommonConstants.LoggingEvents.GetItem, MsgConstants.ErrorMessages.ErrorGetById, _nameController);
        //    }
        //    return Ok(response);
        //}

        //[HttpPut]
        //public async Task<IActionResult> UpdateJsonAPIFunctions([FromBody] UpadateJsonAPIFunctionIdVModel model)
        //{
        //    if (model == null)
        //    {
        //        return new BadRequestObjectResult(String.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameController));
        //    }

        //    await _sysFunctionService.UpadateJsonAPIFunctionId(model);

        //    return NoContent();
        //}
    }
}
