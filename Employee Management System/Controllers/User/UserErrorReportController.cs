using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Service;

namespace OA.WebApi.Controllers
{
    [Route(CommonConstants.Routes.BaseRouteUser)]
    [ApiController]
    public class UserErrorReportController : ControllerBase
    {
        private readonly IErrorReportService _errorReportService;
        private readonly ILogger<UserErrorReportController> _logger;

        public UserErrorReportController(IErrorReportService errorReportService, ILogger<UserErrorReportController> logger)
        {
            _errorReportService = errorReportService;
            _logger = logger;
        }

        

        [HttpGet]
        public async Task<IActionResult> SearchByUserId([FromQuery] FilterErrorReportVModel model)
        {
            var response = await _errorReportService.SearchByUserId(model);
            return Ok(response);
        }


        

        [HttpGet]
        public async Task<IActionResult> CountErrorReportsInMonthUser([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month <= 0 || month > 12)
            {
                return BadRequest("Year and month must be valid values.");
            }

            var response = await _errorReportService.CountErrorReportsInMonthUser(year, month);
            return Ok(response);
        }

       

        [HttpGet]
        public async Task<IActionResult> CountErrorReportsByTypeAndYearUser(int year)
        {
            if (year <= 0)
            {
                return BadRequest("Year must be a valid value.");
            }

            var response = await _errorReportService.CountErrorReportsByTypeAndYearUser(year);
            return Ok(response);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportFile([FromQuery] FilterErrorReportVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type = exportModel.Type.ToUpper();
            var content = await _errorReportService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _errorReportService.GetById(id);
            return Ok(response);
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ErrorReportCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _errorReportService.Create(model);
            return Created(string.Empty, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ErrorReportUpdateVModel model)
        {
            if (!ModelState.IsValid || model.Id <= 0)
            {
                return BadRequest(ModelState);
            }

            await _errorReportService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0)
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }

            await _errorReportService.Remove(id);
            return NoContent();
        }
    }

}
