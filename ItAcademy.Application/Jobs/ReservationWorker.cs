using ItAcademy.Application.Orders.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;

namespace ItAcademy.Application.Jobs
{
    public class ReservationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CrontabSchedule _schedule;
        private static string Schedule => "*/30 * * * * *";
        private DateTime _nextRun;
        public ReservationWorker(IServiceScopeFactory serviceScopeFactory)
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
                        var _archiveService = scope.ServiceProvider.GetRequiredService<IReserveRepository>();
                        await _archiveService.TerminateOrder(token).ConfigureAwait(false);
                        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                    }
                }
            }
        }
    }
}
