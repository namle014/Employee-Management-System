﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;

namespace OA.WebApi.Controllers
{
    [Authorize(Policy = CommonConstants.Authorize.CustomAuthorization)]
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]

    public class SalaryController : Controller
    {
        private readonly ISalaryService _salaryService;
        private readonly ILogger _logger;
        private static string _nameController = StringConstants.ControllerName.Salary;

        public SalaryController(ISalaryService salaryService, ILogger<SalaryController> logger)
        {
            _salaryService = salaryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _salaryService.GetById(id);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SalaryFilterVModel model, string period)
        {
            if (string.IsNullOrEmpty(period))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "period"));
            }
            var response = await _salaryService.GetAll(model, period);
            return Ok(response);
        }
        //[HttpGet]
        //public async Task<IActionResult> Search([FromQuery] FilterSalaryVModel model)
        //{
        //    var response = await _salaryService.Search(model);
        //    return Ok(response);
        //}
        [HttpPost]
        public async Task<IActionResult> Create()
        {

            await _salaryService.Create();
            return Created();
        }
        [HttpPut]
        public async Task<IActionResult> Update(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return new BadRequestObjectResult("Phải có Id");
            }
            await _salaryService.Update(Id);
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
        [HttpPut(CommonConstants.Routes.Id)]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "id"));
            }
            await _salaryService.ChangeStatus(id);
            return NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> PaymentConfirmation(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return new BadRequestObjectResult("Phải có Id");
            }
            await _salaryService.PaymentConfirmation(Id);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetIncomeInMonth(int year, int month)
        {
            if (year < 1 || month < 1 || month > 12)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "year or month"));
            }
            var response = await _salaryService.GetIncomeInMonth(year, month);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetYearIncome(int year)
        {
            if (year < 1)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "year"));
            }
            var response = await _salaryService.GetYearIncome(year);
            return Ok(response);
        }
        [HttpPut]
        public async Task<IActionResult> ChangeStatusMany(SalaryChangeStatusManyVModel model)
        {
            await _salaryService.ChangeStatusMany(model);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetInfoForDepartmentChart()
        {
            var response = await _salaryService.GetInfoForDepartmentChart();
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetSalaryByLevel()
        {
            var response = await _salaryService.GetSalaryByLevel();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetInfoForSalarySummary()
        {
            var response = await _salaryService.GetInfoForSalarySummary();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalIncomeOverTime()
        {
            var response = await _salaryService.GetTotalIncomeOverTime();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetIncomeStructure()
        {
            var response = await _salaryService.GetIncomeStructure();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetPeriod()
        {
            var response = await _salaryService.GetPeriod();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalBySex()
        {
            var response = await _salaryService.GetTotalBySex();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetGrossTotal()
        {
            var response = await _salaryService.GetGrossTotal();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalMaxMin()
        {
            var response = await _salaryService.GetTotalMaxMin();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetDisplayInfo()
        {
            var response = await _salaryService.GetDisplayInfo();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetGrossTotalByDepartments()
        {
            var response = await _salaryService.GetGrossTotalByDepartments();
            if (response.Data != null)
            {
                return Ok(response);
            }
            return NotFound(new { Message = "Không có dữ liệu" });
        }
        [HttpGet]
        public async Task<IActionResult> GetPayrollOfDepartmentOvertime(int year)
        {
            if (year < 1)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "year"));
            }
            var response = await _salaryService.GetPayrollOfDepartmentOvertime(year);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetPayrollReport(int year)
        {
            if (year < 1)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "year"));
            }
            var response = await _salaryService.GetPayrollReport(year);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> PayrollOverview(string period)
        {
            if (string.IsNullOrEmpty(period))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "period"));
            }
            var response = await _salaryService.PayrollOverview(period);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnpaidSalary([FromQuery] SalaryFilterVModel model, int year)
        {
            if (year < 1)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "year"));
            }
            var response = await _salaryService.GetUnpaidSalary(model, year);
            return Ok(response);
        }
        
    }
}
