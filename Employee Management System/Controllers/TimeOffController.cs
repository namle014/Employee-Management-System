using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class TimeOffController : ControllerBase
    {
        private readonly ITimeOffService _timeOffService;
        private readonly ILogger<TimeOffController> _logger;

        public TimeOffController(ITimeOffService timeOffService, ILogger<TimeOffController> logger)
        {
            _timeOffService = timeOffService;
            _logger = logger;
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] FilterTimeOffVModel model)
        {
            var response = await _timeOffService.Search(model);
            return Ok(response);
        }

        [HttpGet("time-off-statistics")]
        public async Task<IActionResult> GetTimeOffStatistics(int year, int month)
        {
            var response = await _timeOffService.CountTimeOffsInMonth(year, month);
            return Ok(response);
        }


        [HttpGet("pending-future-timeoffs")]
        public async Task<IActionResult> GetPendingFutureTimeOffs()
        {
            var fromDate = DateTime.Now.Date;
            var response = await _timeOffService.GetPendingFutureTimeOffs(fromDate);
            return Ok(response);
        }




        [HttpGet("export")]
        public async Task<IActionResult> ExportFile([FromQuery] FilterTimeOffVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type = exportModel.Type.ToUpper();
            var content = await _timeOffService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _timeOffService.GetById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimeOffCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _timeOffService.Create(model);
            return Created(string.Empty, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TimeOffUpdateVModel model)
        {
            if (!ModelState.IsValid || model.Id <= 0)
            {
                return BadRequest(ModelState);
            }

            await _timeOffService.Update(model);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            if (id <= 0)
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }

            await _timeOffService.ChangeStatus(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0)
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }

            await _timeOffService.Remove(id);
            return NoContent();
        }

      
    }
}
