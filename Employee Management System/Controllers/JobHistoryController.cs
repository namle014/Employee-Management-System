using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Service;
using OA.Service.Helpers;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class JobHistoryController : ControllerBase
    {
        private readonly IJobHistoryService _JobHistoryService;
        private readonly ILogger<JobHistoryController> _logger;

        public JobHistoryController(IJobHistoryService JobHistoryService, ILogger<JobHistoryController> logger)
        {
            _JobHistoryService = JobHistoryService;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] FilterJobHistoryVModel model)
        {
            var response = await _JobHistoryService.Search(model);
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> SearchByUser(string id)
        {
            try
            {
                var result = await _JobHistoryService.SearchByUser(id);
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


        [HttpGet("export")]
        public async Task<IActionResult> ExportFile([FromQuery] FilterJobHistoryVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type = exportModel.Type.ToUpper();
            var content = await _JobHistoryService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _JobHistoryService.GetById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JobHistoryVModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _JobHistoryService.Create(model);
            return Created(string.Empty, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] JobHistoryUpdateVModel model)
        {
            if (!ModelState.IsValid || model.Id <= 0)
            {
                return BadRequest(ModelState);
            }

            await _JobHistoryService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0)
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }

            await _JobHistoryService.Remove(id);
            return NoContent();
        }
    }

}
