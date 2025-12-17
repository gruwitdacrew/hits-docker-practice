using Mockups.Storage;
using Mockups.Models.Menu;

namespace Mockups.Tests
{
    public interface IMenuItemRepositoryMock
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

    public class MenuItemRepositoryMockAdapter : IMenuItemRepositoryMock
    {
        private readonly Mockups.Repositories.MenuItems.MenuItemRepository _repository;

        public MenuItemRepositoryMockAdapter(Mockups.Repositories.MenuItems.MenuItemRepository repository)
        {
            _repository = repository;
        }

        public Task<List<MenuItem>> GetAllMenuItems() => _repository.GetAllMenuItems();
        public Task<List<MenuItem>> GetAllMenuItems(MenuItemCategory[] category) => _repository.GetAllMenuItems(category);
        public Task<List<MenuItem>> GetAllMenuItems(bool isVegan, MenuItemCategory[] category) => _repository.GetAllMenuItems(isVegan, category);
        public Task<List<MenuItem>> GetAllMenuItems(bool isVegan) => _repository.GetAllMenuItems(isVegan);
        public Task<MenuItem?> GetItemByName(string name) => _repository.GetItemByName(name);
        public Task<MenuItem?> GetItemById(Guid id) => _repository.GetItemById(id);
        public Task AddItem(MenuItem item) => _repository.AddItem(item);
        public Task DeleteItem(MenuItem item) => _repository.DeleteItem(item);
    }
}