using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
namespace OA.WebApi.Controllers
{
    //[Authorize(Policy = CommonConstants.Authorize.CustomAuthorization)]
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceService _service;
        private readonly ILogger _logger;
        protected static string? _nameController = "Insurance";

        public InsuranceController(IInsuranceService service, ILogger<InsuranceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            if (id == null)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _service.GetById(id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] FilterInsuranceVModel model)
        {
            var response = await _service.Search(model);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InsuranceCreateVModel model)
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
        public async Task<IActionResult> Update([FromBody] InsuranceUpdateVModel model)
        {
            //if (!ModelState.IsValid || (model as dynamic)?.Id <= 0)
            //{
            //    return new BadRequestObjectResult(ModelState);
            //}
            await _service.Update(model);

            return NoContent();
        }

        [HttpPut(CommonConstants.Routes.Id)]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (id == null)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }

            await _service.ChangeStatus(id);

            return NoContent();
        }

        [HttpDelete(CommonConstants.Routes.Id)]
        public virtual async Task<IActionResult> Remove(string id)
        {
            if (id == null)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }

            await _service.Remove(id);

            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAll();
            return Ok(response);
        }
    }
}