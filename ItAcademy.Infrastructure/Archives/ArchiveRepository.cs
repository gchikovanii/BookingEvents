using ItAcademy.Application.Archive.Reposiotires;
using ItAcademy.Domain.ArchiveAggregate;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Infrastructure.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace ItAcademy.Infrastructure.Archives
{
    public class ArchiveRepository : IArchiveRepository
    {
        private readonly IBaseRepository<Archive> _archiveRepository;
        private readonly IBaseRepository<Event> _eventRepository;

        public ArchiveRepository(IBaseRepository<Archive> archiveRpository, IBaseRepository<Event> eventRepository)
        {
            _archiveRepository = archiveRpository;
            _eventRepository = eventRepository;
        }
        public async Task MoveToArchive(CancellationToken token)
        {
            var evs =  _eventRepository.GetQuery();
            var totalCount = await evs.CountAsync(token).ConfigureAwait(false);
            var data = await evs.ToListAsync(token).ConfigureAwait(false);
            var numberOfChunks = Math.Ceiling(Convert.ToDecimal(totalCount) / 50);
            for (var i = 0; i < numberOfChunks; i++)
            {
                foreach (var ad in data.Skip((i - 1) * 50).Take(50))
                {
                    if (ad.EndDate <= DateTimeOffset.UtcNow)
                    {
                        _eventRepository.Delete(ad);
                        await _archiveRepository.Create(token, new Archive()
                        {
                            Title = ad.Title,
                            Description = ad.Description,
                            StartDate = ad.StartDate,
                            EndDate = ad.EndDate,
                            Status = ad.Status,
                            CreatedAt = ad.CreatedAt,
                            Location = ad.Location,
                            Image = ad.Image,
                            Price = ad.Price,
                            Quantity = ad.Quantity,
                            UserId = ad.UserId,
                            Approved = ad.Approved
                        }).ConfigureAwait(false);
                    }
                }
                await _eventRepository.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }
    }
}
