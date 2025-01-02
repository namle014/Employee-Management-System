using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Service;

namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly IMassageService _messageService;
        private readonly ILogger _logger;
        private static string _nameController = StringConstants.ControllerName.Message;

        public MessageController(IMassageService messageService, ILogger<MessageController> logger)
        {
            _messageService=messageService;
            _logger=logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _messageService.GetAll();
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _messageService.Create(model);
            return Created();
        }
    }
}
