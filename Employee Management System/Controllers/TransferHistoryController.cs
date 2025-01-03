using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Service;

namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]

    public class TransferHistoryController: Controller
    {
        private readonly ITransferHistoryService _transferHistoryService;
        private readonly ILogger _logger;

        public TransferHistoryController(ITransferHistoryService transferHistoryService, ILogger logger)
        {
            _transferHistoryService=transferHistoryService;
            _logger=logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _transferHistoryService.GetAll();
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransferHistoryCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            await _transferHistoryService.Create(model);
            return Created();
        }
    }
}
