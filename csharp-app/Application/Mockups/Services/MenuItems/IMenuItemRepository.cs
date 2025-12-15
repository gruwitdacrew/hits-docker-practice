using Mockups.Storage;
using Mockups.Models.Menu;

namespace Mockups.Services.MenuItems
{
    public interface IMenuItemRepository
    {
        Task<List<MenuItem>> GetAllMenuItems();
        Task<List<MenuItem>> GetAllMenuItems(MenuItemCategory[] category);
        Task<List<MenuItem>> GetAllMenuItems(bool isVegan, MenuItemCategory[] category);
        Task<List<MenuItem>> GetAllMenuItems(bool isVegan);
        Task<MenuItem?> GetItemByName(string name);
        Task<MenuItem?> GetItemById(Guid id);
        Task AddItem(MenuItem item);
        Task DeleteItem(MenuItem item);
    }
}