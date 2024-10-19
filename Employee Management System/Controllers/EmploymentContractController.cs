using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OA.Core.Constants;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service;
using OA.Service.Helpers;
using OA.WebApi.Controllers;
using System;
using System.Threading.Tasks;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class EmploymentContractController : BaseController<EmploymentContractController, EmploymentContract, EmploymentContractCreateVModel, EmploymentContractUpdateVModel, EmploymentContractGetByIdVModel, EmploymentContractGetAllVModel>
    {
        private readonly IEmploymentContractService _employmentContractService;
        private readonly ILogger<EmploymentContractController> _logger;

        public EmploymentContractController(IEmploymentContractService employmentContractService, ILogger<EmploymentContractController> logger) : base(employmentContractService, logger)
        {
            _employmentContractService = employmentContractService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterEmploymentContractVModel model)
        {
            var response = await _employmentContractService.Search(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile([FromQuery] FilterEmploymentContractVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _employmentContractService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}