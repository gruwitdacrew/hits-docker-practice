using Microsoft.AspNetCore.Mvc;
using Mockups.Services.Analytics;
using Microsoft.AspNetCore.Authorization;

namespace Mockups.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public IActionResult Index()
        {
            // This could be a page that shows non-sensitive analytics to regular users
            // or simply redirect to admin analytics if user is admin
            return RedirectToAction("Index", "Menu");
        }

        /// <summary>
        /// Gets analytics data for the last month (orders count and revenue)
        /// </summary>
        /// <returns>Last month analytics data</returns>
        [Authorize(Roles = "Администратор")]
        [HttpGet("api/analytics/last-month")]
        public async Task<IActionResult> GetLastMonthAnalytics()
        {
            var lastMonthOrdersCount = await _analyticsService.GetLastMonthOrdersCountAsync();
            var lastMonthRevenue = await _analyticsService.GetLastMonthRevenueAsync();

            var result = new
            {
                LastMonthOrdersCount = lastMonthOrdersCount,
                LastMonthRevenue = lastMonthRevenue
            };

            return Ok(result);
        }

        /// <summary>
        /// Gets the average time between customer orders
        /// </summary>
        /// <returns>Customer behavior analytics data</returns>
        [Authorize(Roles = "Администратор")]
        [HttpGet("api/analytics/customer-behavior")]
        public async Task<IActionResult> GetCustomerBehaviorAnalytics()
        {
            var (averageTimeInSeconds, formattedTime) = await _analyticsService.GetAverageTimeBetweenCustomerOrdersAsync();

            var result = new
            {
                AverageTimeBetweenOrdersInSeconds = averageTimeInSeconds,
                AverageTimeBetweenOrdersFormatted = formattedTime
            };

            return Ok(result);
        }
    }
}