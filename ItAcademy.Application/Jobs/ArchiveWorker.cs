using ItAcademy.Application.Archive.Reposiotires;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;

namespace ItAcademy.Application.Jobs
{
    public class ArchiveWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CrontabSchedule _schedule;
        private static string Schedule => "*/50 * * * * *";
        private DateTime _nextRun;

        public ArchiveWorker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _schedule = CrontabSchedule.Parse(Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ExecuteJob(stoppingToken).ConfigureAwait(false);
        }

        public async Task ExecuteJob(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var now = DateTime.Now;
                _schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var _archiveService = scope.ServiceProvider.GetRequiredService<IArchiveRepository>();
                        await _archiveService.MoveToArchive(token).ConfigureAwait(false);
                        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                    }
                }
            }
        }
    }
}
