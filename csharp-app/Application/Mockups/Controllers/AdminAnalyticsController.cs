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

            var model = new AnalyticsViewModel
            {
                ConversionRateData = new ConversionRateViewModel
                {
                    TotalCartAdditions = totalCartAdditions,
                    TotalPurchases = totalPurchases,
                    ConversionRatePercentage = Math.Round(conversionRate, 2)
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
    }
}