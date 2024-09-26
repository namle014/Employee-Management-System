using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
using OA.WebApi.Controllers;
using System.Threading.Tasks;

namespace OA.WebApi.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class SysConfigurationController : BaseController<SysConfigurationController, SysConfiguration, SysConfigurationCreateVModel, SysConfigurationUpdateVModel, SysConfigurationGetByIdVModel, SysConfigurationGetAllVModel>
    {
        private readonly ISysConfigurationService _configService;
        private readonly ILogger _logger;
        public SysConfigurationController(ISysConfigurationService configService, ILogger<SysConfigurationController> logger)
            : base(configService, logger)
        {
            _configService = configService;
            _logger = logger;
        }
    }
}