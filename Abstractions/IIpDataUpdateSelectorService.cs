namespace IPInformationRetrieval.Abstractions
{
    public interface IIpDataUpdateSelectorService
    {
        Task<int?> UpdateDataInBatchesWithTasksAsync();
        Task<int?> UpdateDataInBatchesAsAsyncEnumerable();
    }
}