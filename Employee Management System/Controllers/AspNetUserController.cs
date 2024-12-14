using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Domain.VModels;
using OA.Domain.VModels.Role;
using System.Text.RegularExpressions;
namespace OA.WebApi.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    public class AspNetUserController : Controller
    {
        #region Declaration
        private readonly IAspNetUserService _userService;
        private readonly ILogger _logger;
        private static string _nameController = StringConstants.ControllerName.AspNetUser;
        public AspNetUserController(
            IAspNetUserService userService
            , ILogger<AspNetUserController> logger
            )
        {
            _userService = userService;
            _logger = logger;
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> GetAll(UserFilterVModel model)
        {
            var response = await _userService.GetAll(model);
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeCountByRole()
        {
            var response = await _userService.GetEmployeeCountByRole();
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeCountByDepartment()
        {
            var result = await _userService.GetEmployeeCountByDepartment();

            if (result.Data != null)
            {
                return Ok(result);
            }

            return NotFound(new { Message = "Không có dữ liệu" });
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeCountByAge()
        {
            var result = await _userService.GetEmployeeCountByAge();
            if (result.Data != null)
            {
                return Ok(result);
            }

            return NotFound(new { Message = "Không có dữ liệu" });
        }


        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _userService.GetById(id);
            if (!response.Success)
            {
                _logger.LogWarning(CommonConstants.LoggingEvents.GetItem, MsgConstants.ErrorMessages.ErrorGetById, _nameController);
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetJsonUserHasFunctions(string userId)
        {
            var response = await _userService.GetJsonUserHasFunctions(userId);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CheckValidUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldCanNotEmpty, "userName"));
            }
            await _userService.CheckValidUserName(userName);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CheckValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldCanNotEmpty, "email"));
            }

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return new BadRequestObjectResult(MsgConstants.Error404Messages.InvalidEmail);
            }

            await _userService.CheckValidEmail(email);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ConfirmAccount([FromBody] ConfirmAccount model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                var response = _userService.ConfirmAccount(model);
                if (!response)
                {
                    _logger.LogWarning(CommonConstants.LoggingEvents.CreateItem, MsgConstants.ErrorMessages.ErrorCreate, _nameController);
                }
            }
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                await _userService.Create(model);
            }
            return Created();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoleForUser([FromBody] UpdateRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                await _userService.UpdateRoleForUser(model);
            }
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateJsonUserHasFunctions([FromBody] UpdatePermissionVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _userService.UpdateJsonUserHasFunctions(model);

            return NoContent();
        }

        [HttpPut(CommonConstants.Routes.Id)]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "id"));
            }
            await _userService.ChangeStatus(id);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _userService.Update(model);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _userService.ChangePassword(model);

            return NoContent();
        }

        [HttpDelete(CommonConstants.Routes.Id)]
        public async Task<IActionResult> Remove(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "id"));
            }
            await _userService.Remove(id);

            return NoContent();
        }
    }
}