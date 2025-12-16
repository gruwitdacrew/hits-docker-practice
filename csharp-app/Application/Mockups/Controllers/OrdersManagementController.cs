using Mockups.Models.OrdersManagement;
using Mockups.Services.Orders;
using Mockups.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mockups.Services.Analytics;

namespace Mockups.Controllers
{
    [Authorize(Roles = ApplicationRoleNames.Administrator)]
    public class OrdersManagementController : Controller
    {
        private readonly IOrdersService _ordersService;
        private readonly IAnalyticsService _analyticsService;

        public OrdersManagementController(IOrdersService ordersService, IAnalyticsService analyticsService)
        {
            _ordersService = ordersService;
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _ordersService.GetAllOrders();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int intId = int.Parse(id);

            var model = await _ordersService.GetOrderInfo(intId);

            return View(model);
        }

        [HttpGet]
        [ActionName("Edit")]
        public async Task<IActionResult> EditGet(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int intId = int.Parse(id);

            var model = await _ordersService.GetEditModel(intId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(OrderEditViewModel model)
        {
            await _ordersService.EditOrder(model.PostModel);

            return RedirectToAction("Details", new {id = model.PostModel.orderId });
        }

        [HttpGet]
        public async Task<IActionResult> Analytics()
        {
            var conversionRate = await _analyticsService.GetConversionRateAsync();
            var totalCartAdditions = await _analyticsService.GetTotalCartAdditionsAsync();
            var totalPurchases = await _analyticsService.GetTotalPurchasesAsync();

            var analyticsViewModel = new Models.OrdersManagement.AnalyticsViewModel
            {
                TotalCartAdditions = totalCartAdditions,
                TotalPurchases = totalPurchases,
                ConversionRatePercentage = Math.Round(conversionRate, 2)
            };

            return View(analyticsViewModel);
        }
    }
}
