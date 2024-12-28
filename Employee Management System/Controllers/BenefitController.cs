using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class BenefitController : ControllerBase
    {
        private readonly IBenefitService _service;
        private readonly ILogger<BenefitController> _logger;
        protected static string? _nameController = "Insurance";

        public BenefitController(IBenefitService benefitService, ILogger<BenefitController> logger)
        {
            _service = benefitService;
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
        public async Task<IActionResult> GetAll([FromQuery] FilterBenefitVModel model)
        {
            var response = await _service.GetAll(model);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBenefitType()
        {
            var response = await _service.GetAllBenefitType();

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBenefitUser([FromQuery] GetAllBenefitUser model)
        {
            var response = await _service.GetAllBenefitUser(model);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BenefitCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.Create(model);

            //return Created();
            return StatusCode(StatusCodes.Status201Created); // Trả về status code 201 khi thành công

        }

        [HttpPost]
        public async Task<IActionResult> CreateBenefitType([FromBody] BenefitTypeCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.CreateBenefitType(model);

            //return Created();
            return StatusCode(StatusCodes.Status201Created); // Trả về status code 201 khi thành công

        }

        [HttpPost]
        public async Task<IActionResult> CreateBenefitUser([FromBody] CreateBenefitUser model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.CreateBenefitUser(model);

            //return Created();
            return StatusCode(StatusCodes.Status201Created); // Trả về status code 201 khi thành công

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BenefitUpdateVModel model)
        {

            await _service.Update(model);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBenefitType([FromBody] BenefitTypeUpdateVModel model)
        {

            await _service.UpdateBenefitType(model);

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

        [HttpDelete(CommonConstants.Routes.Id)]
        public virtual async Task<IActionResult> RemoveBenefitType(string id)
        {
            if (id == null)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }

            await _service.RemoveBenefitType(id);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeStatusMany(BenefitChangeStatusManyVModel model)
        {
            await _service.ChangeStatusMany(model);
            return NoContent();
        }



    }
}
