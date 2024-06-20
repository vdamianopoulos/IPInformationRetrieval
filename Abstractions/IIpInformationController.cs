using IPInformationRetrieval.Models;
using Microsoft.AspNetCore.Mvc;

namespace IPInformationRetrieval.Abstractions
{
    public interface IIpInformationController
    {
        Task<IActionResult> GetIpInfo([FromRoute] string ip);
        Task<IActionResult> UpdateDataInBatchesWithTasksAsync();
        Task<IActionResult> UpdateDataInBatchesAsAsyncEnumerable();
        Task<ReportLastAddressStats[]> GetIpReport([FromBody] string[] parameters);

    }
}
