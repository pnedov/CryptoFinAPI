using CryptoFinAPI.Models;
using CryptoFinAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoFinAPI.Controllers;

[ApiController]
[Route("prices")]
public class PricesController : ControllerBase
{
    private IPriceService _service;
    private ILogger<PricesController> _logger;
    public PricesController(IPriceService service, ILogger<PricesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Get price at a specific time.
    /// </summary>
    /// <param name="request">Query request params</param>
    /// <param name="currencyPair">Currency pair</param>
    /// <param name="limit">Limitation response result</param>
    /// <param name="step">API client service param for step</param>
    /// <param name="start">Time point</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns the price point.</returns>
    [HttpGet("price-at")]
    public async Task<ActionResult<PricePoint>> GetPriceAtTimeAsync([FromQuery] GetPriceAtTimeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var price = await _service.GetPriceAtTimeAsync(request.CurrencyPair, request.Start, request.Step, request.Limit, cancellationToken);

            if (price == 0)
            {
                return UnprocessableEntity("The price could not be calculated due to insufficient data.");
            }

            return Ok(price);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting price at time for {currencyPair}, {interval}, {at}", request.CurrencyPair, "Interval", request.Start);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Get price for a specific time range.
    /// </summary>
    /// <param name="currencyPair">Currency pair</param>
    /// <param name="limit">Limitation response result</param>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="step">API client service param for step</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns a list of price points.</returns>
    [HttpGet("prices-for-range")]
    public async Task<ActionResult<PricePoint>> GetPriceForTimeRangeAsync(string currencyPair, string limit, string start, string end, string step, CancellationToken cancellationToken)
    {
        try
        {
            var prices = await _service.GetPriceForTimeRangeAsync(currencyPair, limit, start, end, step, cancellationToken);

            return Ok(prices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting price at time for {currencyPair}, {limit}, {start}, {end}",
                currencyPair, limit, start, end);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
