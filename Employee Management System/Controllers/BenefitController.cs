using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service;
using OA.Service.Helpers;
using OA.WebApi.Controllers;
using System;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Search([FromQuery] FilterBenefitVModel model)
        {
            var response = await _service.Search(model);

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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BenefitUpdateVModel model)
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

    }
}
