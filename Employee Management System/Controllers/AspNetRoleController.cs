using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels.Role;

namespace OA.WebApi.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    public class AspNetRoleController : Controller
    {
        #region Declaration
        private readonly IAspNetRoleService _roleService;
        private readonly ILogger _logger;
        private static string _nameController = StringConstants.ControllerName.AspNetRole;
        #endregion
        #region Constructor       
        public AspNetRoleController(IAspNetRoleService roleService, ILogger<AspNetRoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Excecute get all by query string
        /// </summary>
        /// <param name="model"> Keyword=admin;OrderBy=Name;OrderDirection=ASC/DESC;</param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetAll(FiltersGetAllByQueryStringRoleVModel model)
        {
            var response = await _roleService.GetAll(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, StringConstants.Validate.Id));
            }
            var response = await _roleService.GetById(id);
            if (!response.Success)
            {
                _logger.LogWarning(CommonConstants.LoggingEvents.GetItem, MsgConstants.ErrorMessages.ErrorGetById, _nameController);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CheckValidRoleName(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldCanNotEmpty, StringConstants.Validate.RoleName));
            }
            await _roleService.CheckValidRoleName(roleName);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetJsonRoleHasFunctions(string roleId)
        {
            var response = await _roleService.GetJsonHasFunctionByRoleId(roleId);
            if (!response.Success)
            {
                _logger.LogWarning(CommonConstants.LoggingEvents.GetItem, MsgConstants.ErrorMessages.ErrorGetById, _nameController);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AspNetRoleCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _roleService.Create(model);

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AspNetRoleUpdateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _roleService.Update(model);

            return NoContent();
        }

        [HttpPut(CommonConstants.Routes.Id)]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(CommonConstants.Validate.idIsInvalid);
            }

            await _roleService.ChangeStatus(id);

            return NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateJsonRoleHasFunctions([FromBody] UpadateJsonHasFunctionByRoleIdVModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return new BadRequestObjectResult(StringConstants.Validate.UserIdCannotEmpty);
            }
            await _roleService.UpadateJsonHasFunctionByRoleId(model);

            return NoContent();
        }

        [HttpDelete(CommonConstants.Routes.Id)]
        public async Task<IActionResult> Remove(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(CommonConstants.Validate.idIsInvalid);
            }
            await _roleService.Remove(id);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> ExportFile(FiltersGetAllByQueryStringRoleVModel model, ExportFileVModel exportModel)
        {
            exportModel.Type.ToUpper();
            var content = await _roleService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }
    }
}