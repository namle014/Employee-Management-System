using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteUser)]
    [ApiController]
    public class UserConfigurationController : ControllerBase
    {
        private readonly ISysConfigurationService _service;
        private readonly ILogger _logger;
        protected static string? _nameController = "Configuration";

        public UserConfigurationController(ISysConfigurationService service, ILogger<UserConfigurationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTime()
        {
            var response = await _service.GetTime();

            return Ok(response);
        }
    }
}