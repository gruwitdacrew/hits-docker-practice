using Mockups.Models.Cart;
using Mockups.Models.Menu;
using Mockups.Repositories.Carts;
using Mockups.Repositories.MenuItems;
using Mockups.Storage;
using Mockups.Services.MenuItems;

namespace Mockups.Services.Carts
{
    public class CartsService : ICartsService
    {
        private readonly CartsRepository _cartsRepository;
        private readonly IMenuItemRepository _menuItemRepository;

        public CartsService(IMenuItemRepository menuItemRepository, CartsRepository cartsRepository)
        {
            _menuItemRepository = menuItemRepository;
            _cartsRepository = cartsRepository;
        }

        public async Task AddItemToCart(Guid userId, string itemId, int amount)
        {
            if (string.IsNullOrEmpty(itemId) || !Guid.TryParse(itemId, out Guid itemGuid))
            {
                throw new ArgumentException("Invalid menu item ID format");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than 0");
            }

            var item = await _menuItemRepository.GetItemById(itemGuid);

            if (item == null)
                throw new KeyNotFoundException();

            var cartItem = new CartMenuItem
            {
                MenuItemId = item.Id,
                Amount = amount
            };

            _cartsRepository.AddItemToCart(userId, cartItem);
        }

        public void ClearUsersCart(Guid userId)
        {
            _cartsRepository.ClearUsersCart(userId);
        }

        public void DeleteItemFromCart(Guid userId, Guid itemId)
        {
            _cartsRepository.DeleteItemFromCart(userId, itemId);
        }

        public int GetCartItemCount(Guid userId)
        {
            return _cartsRepository.GetCartItemCount(userId);
        }

        public async Task<CartIndexViewModel> GetUsersCart(Guid userId, bool isForOrder = false)
        {
            var cart = _cartsRepository.GetUsersCart(userId);
            if (isForOrder)
            {
                cart = _cartsRepository.GetUsersCart(userId);
            }

            var vm = new CartIndexViewModel();

            foreach (var cartMenuItem in cart.Items)
            {
                var item = await _menuItemRepository.GetItemById(cartMenuItem.MenuItemId);

                if (item == null)
                {
                    continue;
                }

                var itemAmount = cart.Items.Where(x => x.MenuItemId == item.Id).First().Amount;

                vm.Items.Add(new CartMenuItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Amount = itemAmount,
                });
            }

            return vm;
        }



        //GetUsersCartForOrder
    }
}
