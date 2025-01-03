using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;

namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class HolidayController : Controller
    {
        public readonly IHolidayService _holidayService;
        public readonly ILogger _logger;
        private static string _nameController = StringConstants.ControllerName.Holiday;
        public HolidayController(IHolidayService holidayService, ILogger<HolidayController> logger)
        {
            _holidayService = holidayService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] EventFilterVModel model)
        {
            var response = await _holidayService.GetAll(model);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _holidayService.GetById(id);
            if (!response.Success)
            {
                _logger.LogWarning(CommonConstants.LoggingEvents.GetItem, MsgConstants.ErrorMessages.ErrorGetById, _nameController);
            }
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _holidayService.Create(model);
            return Created();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EventUpdateVModel model)
        {
            if (!ModelState.IsValid || (model as dynamic)?.Id <= 0)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _holidayService.Update(model);
            return NoContent();
        }
        [HttpDelete(CommonConstants.Routes.Id)]
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }

            await _holidayService.Remove(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMany([FromBody] HolidayDeleteManyVModel model)
        {
            if (model == null || model.Ids == null || !model.Ids.Any())
            {
                return BadRequest("Danh sách ID cần xóa không được để trống.");
            }
            await _holidayService.DeleteMany(model);
            return NoContent();
        }
    }
}
