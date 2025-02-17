using DigitalWallet.Core.DTOs;
using DigitalWallet.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DigitalWallet.Core.Services;
using DigitalWallet.Core.DTOs;
using AutoMapper;
using DigitalWallet.Core.Models;

namespace DigitalWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly WalletService _walletService;
        private readonly IMapper _mapper;

        public WalletController(WalletService walletService, IMapper mapper)
        {
            _walletService = walletService;
            _mapper = mapper;
        }

        [HttpGet("balance")]
        public async Task<ActionResult<decimal>> GetBalance()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _walletService.GetBalanceAsync(userId);
            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<TransferResponse>> Transfer([FromBody] TransferRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _walletService.TransferAsync(userId, request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error });
            }

            return Ok(result.Data);
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactions()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var transactions = await _walletService.GetTransactionsAsync(userId);
            var transactionDtos = _mapper.Map<List<TransactionDto>>(transactions);
            return Ok(transactionDtos);
        }
    }
}