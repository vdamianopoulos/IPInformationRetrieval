using IPInformationRetrieval.Abstractions;

namespace IPInformationRetrieval.Services
{
    public class JobScheduler : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private PeriodicTimer? _timer = null;
        private readonly ILogger<JobScheduler> _logger;
        private readonly IIpInformationService _ipInformationService;

        public JobScheduler(ILogger<JobScheduler> logger, IIpInformationService ipInformationService)
        {
            _logger = logger;
            _ipInformationService = ipInformationService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Service} is running.", nameof(JobScheduler));

            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await DoWork();
            }

            await Task.CompletedTask;
        }
        private async Task DoWork()
        {
            var count = Interlocked.Increment(ref executionCount);

            await _ipInformationService.UpdateDataInBatchesWithTasksAsync();

            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            Dispose();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer.Dispose();
            _timer = null;
        }
    }
}
