using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Service;
using OA.WebApi.Controllers;

namespace Employee_Management_System.Controllers.User
{
    [Route(CommonConstants.Routes.BaseRouteUser)]
    [ApiController]
    public class UserSalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;
        private readonly ILogger _logger;
        protected static string _nameController = "Notifications";
        public UserSalaryController(ISalaryService service, ILogger<UserSalaryController> logger)
        {
            _salaryService = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMeInfo()
        {
            var response = await _salaryService.GetMeInfo();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetMeInfoCycle(int year)
        {
            if (year < 1)
            {
                return new BadRequestObjectResult($"Năm {year} không hợp lệ");
            }
            var response = await _salaryService.GetMeInfoCycle(year);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetIncomeByYear(int year)
        {
            if (year < 1)
            {
                return new BadRequestObjectResult($"Năm {year} không hợp lệ");
            }
            var response = await _salaryService.GetIncomeByYear(year);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetMeSalaryInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult($"ID {id} này không tồn tại");
            }
            var response = await _salaryService.GetMeSalaryInfo(id);
            return Ok(response);
        }
    }
}
