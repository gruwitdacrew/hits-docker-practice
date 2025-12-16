using Mockups.Models;
using Mockups.Models.Menu;
using Mockups.Services.Analytics;
using Mockups.Services.Carts;
using Mockups.Services.MenuItems;
using Mockups.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Mockups.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuItemsService _menuItemsService;
        private readonly ICartsService _cartsService;
        private readonly IAnalyticsService _analyticsService;

        public MenuController(IMenuItemsService menuItemsService, ICartsService cartsService, IAnalyticsService analyticsService)
        {
            _menuItemsService = menuItemsService;
            _cartsService = cartsService;
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] MenuItemCategory[]? filterCategory, [FromQuery] bool? filterIsVegan = null)
        {
            var items = await _menuItemsService.GetAllMenuItems(filterIsVegan, filterCategory);

            return View(items);
        }

        [HttpGet]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMenuItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await _menuItemsService.CreateMenuItem(model);
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                // Handle expected business logic errors
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception)
            {
                // Handle unexpected errors without exposing internal details
                ModelState.AddModelError("", "An error occurred while creating the menu item. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        [ActionName("Delete")]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public async Task<IActionResult> DeleteGet(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _menuItemsService.GetItemModelById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public async Task<IActionResult> DeletePost(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _menuItemsService.DeleteMenuItem(id);

            if (result == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("AddToCart")]
        [Authorize]
        public async Task<IActionResult> AddToCart(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _menuItemsService.GetAddToCartModel(id);

            return View(model);
        }

        [HttpPost]
        [ActionName("AddToCart")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToCartPost(string id, int amount)
        {
            // Validate amount
            if (amount <= 0)
            {
                ModelState.AddModelError(nameof(amount), "Amount must be greater than 0.");
                var model = await _menuItemsService.GetAddToCartModel(id);
                return View(model);
            }

            // Validate and parse user ID safely
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized();
            }

            try
            {
                // Track cart addition for analytics
                if (Guid.TryParse(id, out Guid menuItemId))
                {
                    await _analyticsService.TrackCartAdditionAsync(menuItemId, userIdClaim.Value, null, HttpContext.Connection.RemoteIpAddress?.ToString());
                }

                await _menuItemsService.AddItemToCart(userId, id, amount);
                return RedirectToAction("Index", "Menu");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                // Log the actual error internally while showing a generic message
                ModelState.AddModelError("", "An error occurred while adding the item to cart. Please try again.");
                var model = await _menuItemsService.GetAddToCartModel(id);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var model = await _menuItemsService.GetItemModelById(id);
            if (model == null)
            {
                return NotFound();
            }

            // Track the product view
            if (Guid.TryParse(id, out Guid menuItemId))
            {
                string? userId = null;
                var userIdClaim = User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    userId = userIdClaim.Value;
                }

                await _analyticsService.TrackCartAdditionAsync(menuItemId, userId, null, HttpContext.Connection.RemoteIpAddress?.ToString());
            }

            return View(model);
        }
    }
}