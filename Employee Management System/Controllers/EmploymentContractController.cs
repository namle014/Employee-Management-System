using Microsoft.AspNetCore.Mvc;
using OA.Core.Constants;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;

namespace OA.WebAPI.AdminControllers
{
    [Route(CommonConstants.Routes.BaseRouteAdmin)]
    [ApiController]
    public class EmploymentContractController : ControllerBase
    {
        private readonly IEmploymentContractService _EmploymentContractService;
        private readonly ILogger<EmploymentContractController> _logger;

        public EmploymentContractController(IEmploymentContractService EmploymentContractService, ILogger<EmploymentContractController> logger)
        {
            _EmploymentContractService = EmploymentContractService;
            _logger = logger;
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] FilterEmploymentContractVModel model)
        {
            var response = await _EmploymentContractService.Search(model);
            return Ok(response);
        }


        [HttpGet("expiring-soon")]
        public async Task<IActionResult> GetContractsExpiringSoon([FromQuery] FilterEmploymentContractVModel model, int daysUntilExpiration)
        {
            var result = await _EmploymentContractService.GetContractsExpiringSoon(model, daysUntilExpiration);
            return Ok(result);
        }


        [HttpGet("count-by-type")]
        public async Task<IActionResult> GetContractCountByType()
        {
            var response = await _EmploymentContractService.GetContractCountByType();
            return Ok(response);
        }

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetEmployeeStatsByMonthAndYear([FromQuery] int year, [FromQuery] int month)
        {
            if (year <= 0 || month <= 0 || month > 12)
            {
                return BadRequest("Year and month must be valid values.");
            }

            var response = await _EmploymentContractService.GetEmployeeStatsByMonthAndYear(year, month);
            return Ok(response);
        }


        [HttpGet("yearly-stats")]
        public async Task<IActionResult> GetEmployeeStatsByYear([FromQuery] int year)
        {
            if (year <= 0)
            {
                return BadRequest("Year must be a valid value.");
            }

            var response = await _EmploymentContractService.GetEmployeeStatsByYear(year);
            return Ok(response);
        }




        [HttpGet("export")]
        public async Task<IActionResult> ExportFile([FromQuery] FilterEmploymentContractVModel model, [FromQuery] ExportFileVModel exportModel)
        {
            exportModel.Type = exportModel.Type.ToUpper();
            var content = await _EmploymentContractService.ExportFile(model, exportModel);
            return File(content.Stream, content.ContentType, content.FileName);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(String id)
        {
            if ((string.IsNullOrEmpty(id)))
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            var response = await _EmploymentContractService.GetById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmploymentContractCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _EmploymentContractService.Create(model);
            return Created(string.Empty, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EmploymentContractUpdateVModel model)
        {
            if (!ModelState.IsValid || (string.IsNullOrEmpty(model.Id)))
            {
                return BadRequest(ModelState);
            }

            await _EmploymentContractService.Update(model);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if ((string.IsNullOrEmpty(id)))
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }

            await _EmploymentContractService.ChangeStatus(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(String id)
        {
            if ((string.IsNullOrEmpty(id)))
            {
                return BadRequest(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }

            await _EmploymentContractService.Remove(id);
            return NoContent();
        }

       
    }
}
