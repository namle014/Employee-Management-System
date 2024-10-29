using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.WebApi.Controllers 
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class HolidayController : Controller
    {
        public readonly IHolidayService _holidayService;
        public readonly ILogger _logger;
        public HolidayController(IHolidayService holidayService, ILogger<HolidayController> logger)
        {
            _holidayService = holidayService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _holidayService.GetAll();
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HolidayCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _holidayService.Create(model);
            return Created();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] HolidayUpdateVModel model)
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
            if(id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }

            await _holidayService.Remove(id);
            return NoContent();
        }
    }
}
