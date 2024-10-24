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
    }
}
