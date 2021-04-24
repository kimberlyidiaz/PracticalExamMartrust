using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PracticalExamMartrust.Core.DTOs;
using PracticalExamMartrust.Core.Models;
using PracticalExamMartrust.Core.Services;

namespace PracticalExamMartrust.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FXCurrencyController : ControllerBase
    {
        private readonly ILogger<FXCurrencyController> _logger;
        public FXCurrencyController(ILogger<FXCurrencyController> logger)
        {
            _logger = logger;
        }

        ///<summary>
        /// This gets all of the currency and will be used for currency selection
        /// </summary>
        [HttpGet("GetCurrencies")]
        public List<Currencies> Get()
        {
            List<Currencies> currencies = new List<Currencies>();
            FXCurrencyServices forexExchangeService = new FXCurrencyServices();
            currencies = forexExchangeService.GetCurrencyList();
            return currencies;
        }

        ///<summary>
        /// This POST API computes for the Forex Exchange
        /// Mode : input 1 if buying, 2 if selling
        /// </summary>
        [HttpPost("Compute")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public FXCurrencyDTO Compute(string buyCurrency, string buyAmount, string sellCurrency, string sellAmount, string mode)
        {
            FXCurrencyDTO response = new FXCurrencyDTO();
            FXCurrencyServices fxCurrencyService = new FXCurrencyServices();
            response = fxCurrencyService.MyProperty(buyCurrency, buyAmount, sellCurrency, sellAmount, mode);
            return response;
        }
    }
}
