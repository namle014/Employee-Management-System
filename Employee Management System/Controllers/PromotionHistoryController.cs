using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Service;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class PromotionHistoryController : ControllerBase
    {
        private readonly IPromotionHistoryService _service;
        private readonly ILogger<PromotionHistoryController> _logger;
        protected static string? _nameController = "PromotionHistory";

        public PromotionHistoryController(IPromotionHistoryService promotionService, ILogger<PromotionHistoryController> logger)
        {
            _service = promotionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            if (id == null)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _service.GetByUserId(id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPromotionHistory model)
        {
            var response = await _service.GetAll(model);

            return Ok(response);
        }

       

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePromotionHistory model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.Create(model);

            //return Created();
            return StatusCode(StatusCodes.Status201Created); // Trả về status code 201 khi thành công

        }

        

        

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePromotionHistory model)
        {

            await _service.Update(model);

            return NoContent();
        }

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetPromotionHistoryByMonthAndYear([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month <= 0 || month > 12)
            {
                return BadRequest("Year and month must be valid values.");
            }

            var response = await _service.GetPromotionHistoryByMonthAndYear(year, month);
            return Ok(response);
        }











    }
}
