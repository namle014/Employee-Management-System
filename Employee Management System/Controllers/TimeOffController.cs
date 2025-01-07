using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Service.Helpers;

namespace OA.WebAPI.AdminControllers
{
    [Authorize(Policy = CommonConstants.Authorize.CustomAuthorization)]
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

        [HttpGet]
        public async Task<IActionResult> SearchByUserId([FromQuery] FilterTimeOffVModel model)
        {
            var response = await _timeOffService.SearchByUserId(model);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeOffIsAccepted(int year)
        {
            var response = await _timeOffService.GetTimeOffIsAccepted(year);
            return Ok(response);
        }


        [HttpGet("time-off-statistics")]
        public async Task<IActionResult> GetTimeOffStatistics(int year, int month)
        {
            var response = await _timeOffService.CountTimeOffsInMonth(year, month);
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> CountTimeOffsInMonthUser(int year, int month)
        {
            var response = await _timeOffService.CountTimeOffsInMonthUser(year, month);
            return Ok(response);
        }


        [HttpGet("pending-future-timeoffs")]
        public async Task<IActionResult> GetPendingFutureTimeOffs()
        {
            var fromDate = DateTime.Now.Date;
            var response = await _timeOffService.GetPendingFutureTimeOffs(fromDate);
            return Ok(response);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateIsAccepted(int id, bool? isAccepted)
        {
            if (id <= 0)
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }

            if (isAccepted == null)
            {
                return BadRequest(new { Message = "Giá trị IsAccepted không được để trống." });
            }

            try
            {
                var result = await _timeOffService.UpdateIsAcceptedAsync(id, isAccepted);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, $"Không tìm thấy yêu cầu nghỉ phép với Id = {id}.");
                return NotFound(new { Message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Dữ liệu đầu vào không hợp lệ.");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi hệ thống trong quá trình cập nhật IsAccepted.");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống.", Details = ex.Message });
            }
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
