using IRecharge.Core.Application.Interface;
using IRecharge.Core.Application.WalletServices;
using Microsoft.AspNetCore.Mvc;

namespace IRecharge.WalletAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost("{walletId}/add-funds")]
        public async Task<IActionResult> AddFunds([FromBody] FundWallet model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _walletService.AddFunds(model);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("/Vend/{validationRef}/pay")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest model, string validationRef)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _walletService.ProcessPayment(model);
            return StatusCode(result.StatusCode, result);
        }
    }
}
