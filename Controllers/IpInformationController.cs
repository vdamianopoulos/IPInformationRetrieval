using Asp.Versioning;
using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Models;
using IPInformationRetrieval.Validators;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IPInformationRetrieval.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class IpInformationController : ControllerBase, IIpInformationController
    {
        private readonly ILogger<IpInformationController> _logger;
        private readonly IIpInformationService _ipInformationService;

        public IpInformationController(ILogger<IpInformationController> logger, IIpInformationService ipInformationService)
        {
            _logger = logger;
            _ipInformationService = ipInformationService;
        }

        [HttpGet("getIpInfo/{ip}")]
        [MapToApiVersion("1.0")]
        [ValidateIpAddress]
        public async Task<IActionResult> GetIpInfo([FromRoute] string ip)
        {
            var results = await _ipInformationService.GetAsync(ip);
            if (results.countryAndIPAddress == null)
                return NotFound("No such IP was found.");

            if (results.resultsSource == ResultsSource.ExternalProvider)
            {
                await _ipInformationService.SetAsync(results.countryAndIPAddress);
            }

            return Ok(JsonSerializer.Serialize(
                new { DataReceivedFrom = results.resultsSource.ToString(), CountryAndIPAddress = results.countryAndIPAddress },
                new JsonSerializerOptions { WriteIndented = true }
            ));
        }

        [HttpPost("updateIpInfoInBatchesWithTasks")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateDataInBatchesWithTasksAsync()
        {
            var result = await _ipInformationService.UpdateDataInBatchesWithTasksAsync();

            return result != null
                ? Ok($"Data changed : {result}")
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("updateIpInfoInBatchesAsAsyncEnumerable")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateDataInBatchesAsAsyncEnumerable()
        {
            var result = await _ipInformationService.UpdateDataInBatchesAsAsyncEnumerable();

            return result != null
                ? Ok($"Data changed : {result}")
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("invalidateCache")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> InvalidateCacheAsync(string key = null, bool invalidateAllCachedData = false)
        {
            await _ipInformationService.InvalidateCache(key, invalidateAllCachedData);
            return Ok();
        }

        [HttpPost("getIpReport")]
        [MapToApiVersion("1.0")]
        public async Task<ReportLastAddressStats[]> GetIpReport([FromBody] string[] parameters)
        {
            var report = await _ipInformationService.GetReportAsync(parameters);
            return report.ToArray();
        }
    }
}
