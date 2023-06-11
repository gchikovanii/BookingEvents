using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Domain.ManageAggregate;
using ItAcademy.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ItAcademy.Persistence.DatSeed
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<ItAcademyDbContext>();
            Migrate(database);
            SeedAll(database);
        }
        private static void Migrate(ItAcademyDbContext context)
        {
            context.Database.Migrate();
        }

        private static void SeedAll(ItAcademyDbContext context)
        {
            var seeded = false;
            SeedEvents(context, ref seeded);
            //SeedReservationTime(context, ref seeded);
            //SeedRestrictTime(context, ref seeded);
            if (seeded)
                context.SaveChanges();
        }
        private static void SeedEvents(ItAcademyDbContext context, ref bool seeded)
        {
            var events = new List<Event>()
            {
                new Event
                {
                    Title = "Georgia v Norway",
                    Description = "Football Match between georgia and norway",
                    StartDate = DateTimeOffset.UtcNow.AddDays(5),
                    EndDate = DateTimeOffset.UtcNow.AddDays(5).AddHours(2),
                    Status = true,
                    Approved = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Location = "Batumi Areana",
                    Price = 35,
                    Quantity = 25_000,
                    UserId = "ca5a4711-4526-4e3f-bd31-df2ca988e284",
                    Image = "https://api.sofascore.app/api/v1/event/10752406/share-image/16x9"
                },
                new Event
                {
                    Title = "The Equalizer",
                    Description = "Film in the cinema",
                    StartDate = DateTimeOffset.UtcNow.AddDays(3),
                    EndDate = DateTimeOffset.UtcNow.AddDays(15).AddHours(2),
                    Status = true,
                    Approved = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Location = "Cavea East Point",
                    Price = 25,
                    Quantity = 150,
                     UserId = "ca5a4711-4526-4e3f-bd31-df2ca988e284",
                    Image = "https://m.media-amazon.com/images/M/MV5BMTQ2MzE2NTk0NF5BMl5BanBnXkFtZTgwOTM3NTk1MjE@._V1_.jpg"
                },
                new Event
                {
                    Title = "Tbilisi open air",
                    Description = "The mixed funny event",
                    StartDate = DateTimeOffset.UtcNow.AddDays(2),
                    EndDate = DateTimeOffset.UtcNow.AddDays(3).AddHours(2),
                    Status = true,
                    Approved = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Location = "Dzveli Tbilisi",
                    Price = 225,
                    Quantity = 300,
                    UserId = "ca5a4711-4526-4e3f-bd31-df2ca988e284",
                    Image ="https://netgazeti.ge/wp-content/uploads/2022/06/DSC05683.jpg"
                },
                new Event
                {
                    Title = "Ufc Fight Nigth Georgia",
                    Description = "Ufc matchups in georgia",
                    StartDate = DateTimeOffset.UtcNow.AddDays(7),
                    EndDate = DateTimeOffset.UtcNow.AddDays(7).AddHours(9),
                    Status = true,
                    Approved = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Location = "Dinamo Areana",
                    Price = 300,
                    Quantity = 55_000,
                     UserId = "ca5a4711-4526-4e3f-bd31-df2ca988e284",
                    Image = "https://www.silive.com/resizer/ftePF-BBpQ9wg94vQruFbzWtKPg=/1280x0/smart/cloudfront-us-east-1.images.arcpublishing.com/advancelocal/4WTCS3X5YNBU3PBPVSTKYORRAQ.jpeg"
                },
                new Event
                {
                    Title = "KFC Food Challenge",
                    Description = "Food challenge in kfc",
                    StartDate = DateTimeOffset.UtcNow.AddDays(1),
                    EndDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(8),
                    Status = true,
                    Approved = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Location = "Kfc Paliashvili street",
                    Price = 5,
                    Quantity = 20,
                     UserId = "ca5a4711-4526-4e3f-bd31-df2ca988e284",
                    Image= "https://media-cldnry.s-nbcnews.com/image/upload/t_fit-1500w,f_auto,q_auto:best/msnbc/Components/Photos/061113/061113_kfc_logo_vmed5p.jpg"
                }
            };
            foreach (var ev in events)
            {
                if (context.Events.Any(x => x.Title == ev.Title))
                    continue;
                context.Events.Add(ev);
                seeded = true;
            }
        }
        private static void SeedReservationTime(ItAcademyDbContext context, ref bool seeded)
        {
            var time = new ReserveTime
            {
                Id = 1,
                Minutes = 5
            };
            if (!context.ReserveTimes.Any(x => x.Id == time.Id))
            {
                context.ReserveTimes.Add(time);
                seeded = true;
            }
        }
        private static void SeedRestrictTime(ItAcademyDbContext context, ref bool seeded)
        {
            var time = new RestrictEventTime
            {
                Id = 1,
                Hours = 3
            };
            if (!context.RestrictEventTimes.Any(x => x.Id == time.Id))
            {
                context.RestrictEventTimes.Add(time);
                seeded = true;
            }
        }

    }
}
