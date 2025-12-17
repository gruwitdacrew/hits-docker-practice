using Mockups.Services.Analytics;
using Mockups.Storage;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Mockups.Tests
{
    public class AnalyticsServiceTests
    {
        [Fact]
        public async Task UpdateUserLastOnlineAsync_WithValidUser_UpdatesLastOnlineField()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_UpdateUserLastOnline")
                .Options;

            using var context = new ApplicationDbContext(options);
            var service = new AnalyticsService(context);

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                Name = "Test User",
                Phone = "1234567890",
                BirthDate = DateTime.Now.AddYears(-25)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            await service.UpdateUserLastOnlineAsync(userId);

            // Assert
            var updatedUser = await context.Users.FindAsync(userId);
            Assert.NotNull(updatedUser.LastOnline);
            Assert.True(updatedUser.LastOnline >= DateTime.UtcNow.AddMinutes(-5)); // Within 5 minutes
        }

    }
}