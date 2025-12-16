using Mockups.Storage;
using Microsoft.EntityFrameworkCore;

namespace Mockups.Services.Analytics
{
    public interface IAnalyticsService
    {
        Task<double> GetConversionRateAsync();
        Task<int> GetTotalCartAdditionsAsync();
        Task<int> GetTotalPurchasesAsync();
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

    }
}