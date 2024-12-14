using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Infrastructure.EF.Entities;
namespace OA.WebApi.Controllers
{
    //[Authorize(Policy = CommonConstants.Authorize.CustomAuthorization)]
    public abstract class BaseController<TController, TEntity, TCreateVModel, TUpdateVModel, TGetByIdVModel, TGetAllVModel> : ControllerBase
        where TController : ControllerBase
        where TEntity : BaseEntity
    {
        private readonly IBaseService<TEntity, TCreateVModel, TUpdateVModel, TGetByIdVModel, TGetAllVModel> _service;
        private readonly ILogger _logger;
        protected static string? _nameController;
        protected BaseController(IBaseService<TEntity, TCreateVModel, TUpdateVModel, TGetByIdVModel, TGetAllVModel> service, ILogger<TController> logger)
        {
            _service = service;
            _logger = logger;
            _nameController = GetControllerName(typeof(TController).Name);
        }
        private string GetControllerName(string input)
        {
            return input.Substring(0, input.Length - 10).ToLower();
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _service.GetById(id);

            return Ok(response);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.Create(model);

            return Created();
        }

        [HttpPut]
        public virtual async Task<IActionResult> Update([FromBody] TUpdateVModel model)
        {
            if (!ModelState.IsValid || (model as dynamic)?.Id <= 0)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _service.Update(model);

            return NoContent();
        }

        [HttpPut(CommonConstants.Routes.Id)]
        public virtual async Task<IActionResult> ChangeStatus(int id)
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