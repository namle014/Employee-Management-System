using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteUser)]
    [ApiController]
    public class UserAttendanceController : ControllerBase
    {
        private readonly ITimekeepingService _service;
        private readonly ILogger _logger;
        protected static string? _nameController = "Timekeeping";

        public UserAttendanceController(ITimekeepingService service, ILogger<UserAttendanceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> SearchForUser([FromQuery] FilterTimekeepingForUserVModel model)
        {
            var response = await _service.SearchForUser(model);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetByDate([FromQuery] FilterTimekeepingGetByDateVModel model)
        {
            var response = await _service.GetByDate(model);

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Checkout([FromBody] CheckOutVModel model)
        {
            await _service.CheckOut(model);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] TimekeepingCreateUserVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var response = await _service.CreateUser(model);

            return Ok(response);
        }
    }
}