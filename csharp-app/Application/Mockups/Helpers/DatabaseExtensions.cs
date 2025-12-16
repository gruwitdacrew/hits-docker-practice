using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mockups.Storage;

namespace Mockups.Helpers
{
    public static class DatabaseExtensions
    {
        public static void ApplyDatabaseMigrations(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            try
            {
                // Apply database migrations
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while applying database migrations.");
                throw;
            }
        }
    }
}