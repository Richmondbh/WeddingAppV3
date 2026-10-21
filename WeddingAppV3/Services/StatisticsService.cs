using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.ViewModels;

namespace WeddingAppV3.Services
{
    /// <summary>
    /// Service layer for RSVP statistics. The calculations live here, so both the
    /// MVC AdminController (dashboard page) and the StatisticsApiController (JSON)
    /// can reuse the same code:
    ///     AdminController         -> StatisticsService -> Database
    ///     StatisticsApiController -> StatisticsService -> Database
    /// Registered in Program.cs with AddScoped.
    /// </summary>
    public class StatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Totals, percentages and averages. Uses Count(), Sum() and Average().
        public async Task<StatisticsViewModel> GetSummaryAsync()
        {
            int totalGuests = await _context.Guests.CountAsync();
            int attending = await _context.Guests.CountAsync(g => g.WillAttend == true);
            int declined = await _context.Guests.CountAsync(g => g.WillAttend == false);

            int totalAttendants = await _context.Guests
                .Where(g => g.WillAttend == true)
                .SumAsync(g => g.NumberOfAttendants);

            // Average() fails on an empty list, so only call it when someone attends.
            double averageAttendants = 0;
            if (attending > 0)
            {
                averageAttendants = await _context.Guests
                    .Where(g => g.WillAttend == true)
                    .AverageAsync(g => g.NumberOfAttendants);
            }

            return new StatisticsViewModel
            {
                TotalGuests = totalGuests,
                AttendingGuests = attending,
                DeclinedGuests = declined,
                NoResponseGuests = totalGuests - attending - declined,
                TotalAttendants = totalAttendants,
                AttendancePercentage = Percentage(attending, totalGuests),
                ResponseRate = Percentage(attending + declined, totalGuests),
                AverageAttendantsPerGuest = Math.Round(averageAttendants, 1),
                NumberOfEvents = await _context.Events.CountAsync()
            };
        }

        // Groups the guests by their RSVP answer with GroupBy().
        public async Task<List<AttendanceGroupViewModel>> GetAttendanceGroupsAsync()
        {
            int totalGuests = await _context.Guests.CountAsync();

            // (in the database): one row per answer (true, false or null).
            var groups = await _context.Guests
                .GroupBy(g => g.WillAttend)
                .Select(group => new
                {
                    Answer = group.Key,
                    Guests = group.Count(),
                    Attendants = group.Sum(g => g.NumberOfAttendants)
                })
                .ToListAsync();

            // (in memory): turn each row into a ViewModel with a readable status.
            return groups
                .Select(group => new AttendanceGroupViewModel
                {
                    Status = group.Answer == true ? "Attending"
                        : group.Answer == false ? "Declined"
                        : "No response",
                    NumberOfGuests = group.Guests,
                    NumberOfAttendants = group.Attendants,
                    Percentage = Percentage(group.Guests, totalGuests)
                })
                .OrderByDescending(group => group.NumberOfGuests)
                .ToList();
        }

        // part / total as a percentage with one decimal, e.g. 70.8. Avoids division by zero.
        private static double Percentage(int part, int total)
        {
            if (total == 0)
            {
                return 0;
            }

            return Math.Round(part * 100.0 / total, 1);
        }
    }
}
