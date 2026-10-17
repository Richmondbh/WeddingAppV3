using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Models;

namespace WeddingAppV3.Data
{
    /// <summary>
    /// The link between the application and the SQLite database.
    /// It now inherits from IdentityDbContext instead of DbContext, which adds the
    /// Identity tables (AspNetUsers, AspNetRoles, AspNetUserClaims and so on)
    /// next to our own tables (Guests, Events, WeddingInfos).
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Guest> Guests { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<WeddingInfo> WeddingInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Must be called first: it configures all the Identity tables.
            base.OnModelCreating(modelBuilder);

            // Sample guests.
            modelBuilder.Entity<Guest>().HasData(
                new Guest
                {
                    Id = 1,
                    Name = "Anna Lindberg",
                    Email = "anna@rich.com",
                    WillAttend = true,
                    NumberOfAttendants = 2,
                    Message = "We would not miss it for the world!",
                    RegisteredOn = new DateTime(2026, 10, 1)
                },
                new Guest
                {
                    Id = 2,
                    Name = "Johan Berg",
                    Email = "johan@rich.com",
                    WillAttend = false,
                    NumberOfAttendants = 0,
                    Message = "Sadly I am abroad that week. Congratulations!",
                    RegisteredOn = new DateTime(2026, 10, 2)
                },
                new Guest
                {
                    Id = 3,
                    Name = "Ama Mensah",
                    Email = "ama.mensah@rich.com",
                    RegisteredOn = new DateTime(2026, 10, 3)
                });

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Wedding Ceremony",
                    Date = "July 14, 2027",
                    Time = "17:00",
                    Location = "Hope Wedding Hall, Östersund",
                    Description = "The exchange of vows. Please be seated by 16:45."
                },
                new Event
                {
                    Id = 2,
                    Title = "Reception Dinner",
                    Date = "July 14, 2027",
                    Time = "19:00",
                    Location = "Hope Wedding Hall, Östersund",
                    Description = "A three-course dinner with speeches from family and friends."
                },
                new Event
                {
                    Id = 3,
                    Title = "Wedding Party",
                    Date = "July 14, 2027",
                    Time = "21:30",
                    Location = "Hope Wedding Hall, Östersund",
                    Description = "Music and dancing until late."
                });

            modelBuilder.Entity<WeddingInfo>().HasData(
                new WeddingInfo
                {
                    Id = 1,
                    Heading = "Welcome",
                    Content = "Richmond and Keziah met in Östersund seven years ago and have "
                        + "been more or less inseparable ever since. We would love for you "
                        + "to celebrate the day with us.",
                    DisplayOrder = 1
                },
                new WeddingInfo
                {
                    Id = 2,
                    Heading = "The venue",
                    Content = "Everything takes place at Hope Wedding Hall in Östersund. "
                        + "There is free parking next to the building.",
                    DisplayOrder = 2
                },
                new WeddingInfo
                {
                    Id = 3,
                    Heading = "Contact",
                    Content = "Questions? Email hej@richkez.se or call +46 76 931 68 71.",
                    DisplayOrder = 3
                });
        }
    }
}
