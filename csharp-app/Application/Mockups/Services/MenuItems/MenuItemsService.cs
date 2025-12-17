using Mockups.Models.Menu;
using Mockups.Storage;
using Mockups.Repositories.MenuItems;
using Mockups.Models.Cart;
using Mockups.Services.Carts;

namespace Mockups.Services.MenuItems
{
    public class MenuItemsService : IMenuItemsService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ICartsService _cartsService;
        
        private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };

        public MenuItemsService(IWebHostEnvironment environment, IMenuItemRepository menuItemRepository, ICartsService cartsService)
        {
            _environment = environment;
            _menuItemRepository = menuItemRepository;
            _cartsService = cartsService;
        }

        public async Task CreateMenuItem(CreateMenuItemViewModel model)
        {
            var sameMenuItem = await _menuItemRepository.GetItemByName(model.Name);
            if (sameMenuItem != null)
            {
                throw new ArgumentException($"Menu item with same name ({model.Name}) already exists");
            }

            var isFileAttached = model.File != null;
            var fileNameWithPath = string.Empty;
            if (isFileAttached)
            {
                // Validate file extension
                var extension = Path.GetExtension(model.File.FileName).Replace(".", "").ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                {
                    throw new ArgumentException("Attached file's extension is not supported");
                }

                // Validate file size (max 5MB)
                if (model.File.Length > 5 * 1024 * 1024)
                {
                    throw new ArgumentException("File size cannot exceed 5MB");
                }

                // Validate file content type
                var contentType = model.File.ContentType.ToLowerInvariant();
                if (!contentType.Contains("image/"))
                {
                    throw new ArgumentException("Only image files are allowed");
                }

                fileNameWithPath = $"files/{Guid.NewGuid()}-{model.File.FileName}";

                // Ensure WebRootPath is not null
                var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var fullPath = Path.Combine(webRootPath, fileNameWithPath);

                // Ensure directory exists
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fs);
                }
            }

            var newMenuItem = new MenuItem
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                Category = model.Category,
                IsVegan = model.IsVegan,
                PhotoPath = fileNameWithPath
            };

            await _menuItemRepository.AddItem(newMenuItem);
        }

        public async Task<List<MenuItemViewModel>> GetAllMenuItems(bool? isVegan, MenuItemCategory[]? category)
        {
            var itemVMs = new List<MenuItemViewModel>();

            var items = new List<MenuItem>();

            if (isVegan != null && category.Any())
            {
                items = await _menuItemRepository.GetAllMenuItems((bool)isVegan, category);
            }
            else if (category.Any())
            {
                items = await _menuItemRepository.GetAllMenuItems(category);
            }
            else if (isVegan != null)
            {
                items = await _menuItemRepository.GetAllMenuItems((bool)isVegan);
            }
            else
            {
                items = await _menuItemRepository.GetAllMenuItems();
            }
            foreach (var item in items)
            {
                itemVMs.Add(new MenuItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    Category = item.Category,
                    IsVegan = item.IsVegan,
                    PhotoPath = item.PhotoPath
                });
            }

            return itemVMs;
        }

        public async Task<bool?> DeleteMenuItem(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new ArgumentException("Invalid menu item ID format");
            }

            var item = await _menuItemRepository.GetItemById(guid);

            if (item == null)
            {
                return null;
            }

            await _menuItemRepository.DeleteItem(item);

            return true;
        }

        public async Task<MenuItemViewModel?> GetItemModelById(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                return null;
            }

            var item = await _menuItemRepository.GetItemById(guid);

            if (item == null)
            {
                return null;
            }

            return new MenuItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Category = item.Category,
                IsVegan = item.IsVegan,
                PhotoPath = item.PhotoPath
            };
        }

        public async Task AddItemToCart(Guid userID, string itemId, int amount)
        {
            await _cartsService.AddItemToCart(userID, itemId, amount);
        }

        public async Task<AddToCartViewModel> GetAddToCartModel(string itemId)
        {
            if (!Guid.TryParse(itemId, out Guid itemGuid))
            {
                throw new ArgumentException("Invalid menu item ID format");
            }

            var item = await _menuItemRepository.GetItemById(itemGuid);

            if (item == null)
            {
                throw new KeyNotFoundException();
            }

            return new AddToCartViewModel
            {
                Item = new MenuItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    Category = item.Category,
                    IsVegan = item.IsVegan,
                    PhotoPath = item.PhotoPath
                },
            };
        }

        public async Task<string?> GetItemNameById(Guid itemId)
        {
            return (await _menuItemRepository.GetItemById(itemId))?.Name;
        }
    }
}
