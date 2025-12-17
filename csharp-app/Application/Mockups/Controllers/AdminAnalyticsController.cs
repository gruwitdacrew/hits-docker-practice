using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mockups.Services.Analytics;
using Mockups.Models.Analytics;

namespace Mockups.Controllers
{
    [Authorize(Roles = "Администратор")]  // Только администраторы могут получить доступ
    public class AdminAnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public AdminAnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            var conversionRate = await _analyticsService.GetConversionRateAsync();
            var totalCartAdditions = await _analyticsService.GetTotalCartAdditionsAsync();
            var totalPurchases = await _analyticsService.GetTotalPurchasesAsync();
            var lastMonthOrdersCount = await _analyticsService.GetLastMonthOrdersCountAsync();
            var lastMonthRevenue = await _analyticsService.GetLastMonthRevenueAsync();
            var (averageTimeInSeconds, formattedTime) = await _analyticsService.GetAverageTimeBetweenCustomerOrdersAsync();

            var model = new AnalyticsViewModel
            {
                ConversionRateData = new ConversionRateViewModel
                {
                    TotalCartAdditions = totalCartAdditions,
                    TotalPurchases = totalPurchases,
                    ConversionRatePercentage = Math.Round(conversionRate, 2)
                },
                LastMonthData = new LastMonthViewModel
                {
                    OrdersCount = lastMonthOrdersCount,
                    Revenue = lastMonthRevenue
                },
                CustomerBehaviorData = new CustomerBehaviorViewModel
                {
                    AverageTimeBetweenOrdersFormatted = formattedTime,
                    AverageTimeBetweenOrdersInSeconds = averageTimeInSeconds
                }
            };

            return View(model);
        }

        /// <summary>
        /// Calculates the conversion rate from product views to purchases
        /// </summary>
        /// <returns>Conversion rate as a percentage</returns>
        [HttpGet("api/conversion-rate")]
        public async Task<IActionResult> GetConversionRate()
        {
            var conversionRate = await _analyticsService.GetConversionRateAsync();
            var totalCartAdditions = await _analyticsService.GetTotalCartAdditionsAsync();
            var totalPurchases = await _analyticsService.GetTotalPurchasesAsync();

            var result = new
            {
                TotalCartAdditions = totalCartAdditions,
                TotalPurchases = totalPurchases,
                ConversionRatePercentage = Math.Round(conversionRate, 2)
            };

            return Ok(result);
        }

        /// <summary>
        /// Gets analytics data for the last month (orders count and revenue)
        /// </summary>
        /// <returns>Last month analytics data</returns>
        [HttpGet("api/last-month-analytics")]
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
        [HttpGet("api/customer-behavior-analytics")]
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