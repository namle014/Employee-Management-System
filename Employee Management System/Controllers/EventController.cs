using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;

namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class EventController : Controller
    {
        public readonly IEventService _eventService;
        public readonly ILogger _logger;

        public EventController(IEventService eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] EventFilterVModel model)
        {
            var response = await _eventService.GetAll(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }

            var response = await _eventService.GetById(id);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _eventService.Create(model);
            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EventUpdateVModel model)
        {
            if (!ModelState.IsValid || (model as dynamic)?.Id <= 0)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _eventService.Update(model);
            return NoContent();
        }

        [HttpDelete(CommonConstants.Routes.Id)]
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }

            await _eventService.Remove(id);
            return NoContent();
        }
    }
}
