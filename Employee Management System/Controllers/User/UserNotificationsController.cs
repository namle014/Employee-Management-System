using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
namespace OA.WebApi.Controllers
{
    //[Authorize(Policy = CommonConstants.Authorize.CustomAuthorization)]
    [Route(CommonConstants.Routes.BaseRouteUser)]
    [ApiController]
    public class UserNotificationsController : ControllerBase
    {
        private readonly INotificationsService _service;
        private readonly ILogger _logger;
        protected static string _nameController = "Notifications";

        public UserNotificationsController(INotificationsService service, ILogger<UserNotificationsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountIsNew()
        {
            var response = await _service.GetCountIsNew();

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> SearchForUser([FromQuery] FilterNotificationsForUserVModel model)
        {
            var response = await _service.SearchForUser(model);

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateIsNew()
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.UpdateIsNew();

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeRead(NotificationsUpdateReadVModel model)
        {
            await _service.ChangeRead(model);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeStatusForUser()
        {
            await _service.ChangeStatusForUser();

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeAllRead()
        {
            await _service.ChangeAllRead();

            return NoContent();
        }
    }
}