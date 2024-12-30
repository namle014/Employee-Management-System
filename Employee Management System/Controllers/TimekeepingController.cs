using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
namespace OA.WebApi.Controllers
{
    [Authorize(Policy = CommonConstants.Authorize.CustomAuthorization)]
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class TimekeepingController : ControllerBase
    {
        private readonly ITimekeepingService _service;
        private readonly ILogger _logger;
        protected static string? _nameController = "Timekeeping";

        public TimekeepingController(ITimekeepingService service, ILogger<TimekeepingController> logger)
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

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterTimekeepingVModel model)
        {
            var response = await _service.Search(model);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var response = await _service.GetAllDepartments();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimekeepingCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            string? ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                   ?? HttpContext.Connection.RemoteIpAddress?.ToString();

            // Lấy thông tin User-Agent từ header
            string? userAgent = HttpContext.Request.Headers["User-Agent"];

            model.CheckInIP = ipAddress;
            model.UserAgent = userAgent;

            await _service.Create(model);

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TimekeepingUpdateVModel model)
        {
            if (!ModelState.IsValid || (model as dynamic)?.Id <= 0)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.Update(model);

            return NoContent();
        }

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