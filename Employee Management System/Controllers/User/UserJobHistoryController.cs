using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Service;
using OA.Service.Helpers;

namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteUser)]
    [ApiController]
    public class UserJobHistoryController : ControllerBase
    {
        private readonly IJobHistoryService _JobHistoryService;
        private readonly ILogger<UserJobHistoryController> _logger;

        public UserJobHistoryController(IJobHistoryService JobHistoryService, ILogger<UserJobHistoryController> logger)
        {
            _JobHistoryService = JobHistoryService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> SearchByUser()
        {
            try
            {
                var result = await _JobHistoryService.SearchByUser();
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }

}
