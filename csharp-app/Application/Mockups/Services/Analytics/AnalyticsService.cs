using Mockups.Storage;
using Microsoft.EntityFrameworkCore;

namespace Mockups.Services.Analytics
{
    public interface IAnalyticsService
    {
        Task<double> GetConversionRateAsync();
        Task<int> GetTotalCartAdditionsAsync();
        Task<int> GetTotalPurchasesAsync();
        Task<int> GetLastMonthOrdersCountAsync();
        Task<decimal> GetLastMonthRevenueAsync();
        Task<(int averageTimeInSeconds, string formattedTime)> GetAverageTimeBetweenCustomerOrdersAsync();
        Task TrackCartAdditionAsync(Guid menuItemId, string? userId = null, string? sessionId = null, string? ipAddress = null);
        Task UpdateUserLastOnlineAsync(Guid userId);
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<double> GetConversionRateAsync()
        {
            var totalCartAdditions = await GetTotalCartAdditionsAsync();
            var totalPurchases = await GetTotalPurchasesAsync();

            return totalCartAdditions > 0 ? (double)totalPurchases / totalCartAdditions * 100 : 0;
        }

        public async Task<int> GetTotalCartAdditionsAsync()
        {
            return await _context.CartAdditions.CountAsync();
        }

        public async Task<int> GetTotalPurchasesAsync()
        {
            return await _context.OrderMenuItems.CountAsync();
        }

        public async Task TrackCartAdditionAsync(Guid menuItemId, string? userId = null, string? sessionId = null, string? ipAddress = null)
        {
            var cartAddition = new CartAddition
            {
                Id = Guid.NewGuid(),
                MenuItemId = menuItemId,
                AdditionDate = DateTime.UtcNow,
                UserId = userId,
                SessionId = sessionId,
                IpAddress = ipAddress
            };

            _context.CartAdditions.Add(cartAddition);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserLastOnlineAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastOnline = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetLastMonthOrdersCountAsync()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            return await _context.Orders
                .Where(o => o.CreationTime >= oneMonthAgo)
                .CountAsync();
        }

        public async Task<decimal> GetLastMonthRevenueAsync()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            return await _context.Orders
                .Where(o => o.CreationTime >= oneMonthAgo)
                .SumAsync(o => (decimal)o.Cost);
        }

        public async Task<(int averageTimeInSeconds, string formattedTime)> GetAverageTimeBetweenCustomerOrdersAsync()
        {
            // Get all orders grouped by user, ordered by creation time
            var userOrders = await _context.Orders
                .Select(o => new { o.UserId, o.CreationTime })
                .OrderBy(o => o.UserId)
                .ThenBy(o => o.CreationTime)
                .ToListAsync();

            var timeDifferences = new List<long>();

            // Group orders by user and calculate time differences between consecutive orders
            var ordersByUser = userOrders.GroupBy(o => o.UserId);

            foreach (var userGroup in ordersByUser)
            {
                var userOrderList = userGroup.ToList();

                // Calculate time difference between consecutive orders for each user
                for (int i = 1; i < userOrderList.Count; i++)
                {
                    var timeDiff = (userOrderList[i].CreationTime - userOrderList[i - 1].CreationTime).TotalSeconds;
                    if (timeDiff > 0) // Only consider positive time differences
                    {
                        timeDifferences.Add((long)timeDiff);
                    }
                }
            }

            if (timeDifferences.Count == 0)
            {
                return (0, "Нет данных");
            }

            var averageSeconds = (int)timeDifferences.Average();
            return (averageSeconds, FormatTimeSpan(TimeSpan.FromSeconds(averageSeconds)));
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 60)
            {
                return $"{(int)timeSpan.TotalSeconds} сек.";
            }
            else if (timeSpan.TotalMinutes < 60)
            {
                return $"{(int)timeSpan.TotalMinutes} мин. {(int)timeSpan.Seconds} сек.";
            }
            else if (timeSpan.TotalHours < 24)
            {
                return $"{(int)timeSpan.TotalHours} ч. {(int)timeSpan.Minutes} мин.";
            }
            else
            {
                int days = (int)timeSpan.TotalDays;
                int hours = (int)timeSpan.Hours;
                if (hours > 0)
                {
                    return $"{days} дн. {hours} ч.";
                }
                else
                {
                    return $"{days} дн.";
                }
            }
        }
    }
}