using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;
using OA.WebApi.Controllers;

namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]

    public class SalaryController : Controller
    {
        private readonly ISalaryService _salaryService;
        private readonly ILogger _logger;
        private static string _nameController = StringConstants.ControllerName.Salary;

        public SalaryController(ISalaryService salaryService, ILogger<SalaryController> logger)
        {
            _salaryService=salaryService;
            _logger=logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _salaryService.GetById(id);
            if (!response.Success)
            {
                _logger.LogWarning(CommonConstants.LoggingEvents.GetItem, MsgConstants.ErrorMessages.ErrorGetById, _nameController);
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _salaryService.GetAll();
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterSalaryVModel model)
        {
            var response = await _salaryService.Search(model);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalaryCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _salaryService.Create(model);
            return Created();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SalaryUpdateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _salaryService.Update(model);
            return NoContent();
        }
        [HttpDelete(CommonConstants.Routes.Id)]
        public async Task<IActionResult> Remove(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "id"));
            }
            await _salaryService.Remove(id);
            return NoContent();
        }
    }
}
