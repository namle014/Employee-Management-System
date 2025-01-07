using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
namespace OA.WebApi.Controllers
{
    [Authorize(Policy = CommonConstants.Authorize.CustomAuthorization)]
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _service;
        private readonly ILogger _logger;
        protected static string _nameController = "Notifications";

        public NotificationsController(INotificationsService service, ILogger<NotificationsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _service.GetById(id);

            return Ok(response);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetCountIsNew([FromQuery] UserNotificationsUpdateIsNewVModel model)
        //{
        //    var response = await _service.GetCountIsNew(model);

        //    return Ok(response);
        //}

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterNotificationsVModel model)
        {
            var response = await _service.Search(model);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> StatNotificationByMonth([FromQuery] int month, [FromQuery] int year)
        {
            var response = await _service.StatNotificationByMonth(month, year);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> CountNotifyReadByUser([FromQuery] FilterCountNotifyReadByUser model)
        {
            var response = await _service.CountNotifyReadByUser(model);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> StatNotificationByType([FromQuery] int year)
        {
            var response = await _service.StatNotificationByType(year);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> SearchForUser([FromQuery] FilterNotificationsForUserVModel model)
        {
            var response = await _service.SearchForUser(model);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationsCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.Create(model);

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NotificationsUpdateVModel model)
        {
            if (!ModelState.IsValid || (model as dynamic)?.Id <= 0)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.Update(model);

            return NoContent();
        }

        //[HttpPut]
        //public async Task<IActionResult> UpdateIsNew([FromBody] UserNotificationsUpdateIsNewVModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return new BadRequestObjectResult(ModelState);
        //    }

        //    await _service.UpdateIsNew(model);

        //    return NoContent();
        //}


        [HttpPut(CommonConstants.Routes.Id)]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }

            await _service.ChangeStatus(id);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeRead(NotificationsUpdateReadVModel model)
        {
            await _service.ChangeRead(model);

            return NoContent();
        }

        //[HttpPut]
        //public async Task<IActionResult> ChangeStatusForUser(NotificationsUpdateReadVModel model)
        //{
        //    await _service.ChangeStatusForUser(model);

        //    return NoContent();
        //}

        //[HttpPut]
        //public async Task<IActionResult> ChangeAllRead(NotificationsUpdateAllReadVModel model)
        //{
        //    await _service.ChangeAllRead(model);

        //    return NoContent();
        //}

        [HttpDelete(CommonConstants.Routes.Id)]
        public virtual async Task<IActionResult> Remove(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }

            await _service.Remove(id);

            return NoContent();
        }
    }
}