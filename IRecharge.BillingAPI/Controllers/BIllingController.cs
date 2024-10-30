using IRecharge.Core.Application.BillingServices.BillDto;
using IRecharge.Core.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IRecharge.BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BIllingController : ControllerBase
    {
        private readonly IBillingService _billService;

        public BIllingController(IBillingService billService)
        {
            _billService = billService;
        }

        [HttpPost("electricity/verify")]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _billService.CreateBill(model);
            return StatusCode(result.StatusCode, result);
        }
    }
}
