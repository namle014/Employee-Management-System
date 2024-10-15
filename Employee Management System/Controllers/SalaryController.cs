using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;
using OA.WebApi.Controllers;

namespace Employee_Management_System.Controllers
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
    }
}
