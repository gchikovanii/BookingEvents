using ItAcademy.Domain.ArchiveAggregate;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Domain.ManageAggregate;
using ItAcademy.Domain.OrderAggregate;
using ItAcademy.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ItAcademy.Persistence.DataContext
{
    public class ItAcademyDbContext : IdentityDbContext<AppUser>
    {
        public ItAcademyDbContext(DbContextOptions<ItAcademyDbContext> options) : base(options)
        {
        }
        
        public DbSet<Event> Events { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ReserveTime> ReserveTimes { get; set; }
        public DbSet<RestrictEventTime> RestrictEventTimes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Archive> Archives { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItAcademyDbContext).Assembly);
        }
    }
}
