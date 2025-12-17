using Mockups.Services.MenuItems;
using Mockups.Repositories.MenuItems;
using Mockups.Models.Menu;
using Mockups.Storage;
using Mockups.Services.Carts;
using Moq;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Mockups.Tests
{
    public class MenuItemsServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly Mock<IMenuItemRepository> _mockMenuItemRepository;
        private readonly Mock<ICartsService> _mockCartsService;
        private readonly MenuItemsService _menuItemsService;

        public MenuItemsServiceTests()
        {
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockMenuItemRepository = new Mock<IMenuItemRepository>();
            _mockCartsService = new Mock<ICartsService>();

            _mockEnvironment.Setup(x => x.WebRootPath).Returns("wwwroot");

            _menuItemsService = new MenuItemsService(
                _mockEnvironment.Object,
                _mockMenuItemRepository.Object,
                _mockCartsService.Object);
        }

        [Fact]
        public async Task CreateMenuItem_WithValidModel_CreatesItemSuccessfully()
        {
            // Arrange
            var model = new CreateMenuItemViewModel
            {
                Name = "Test Item",
                Price = 10.99f,
                Description = "Test Description",
                Category = MenuItemCategory.Pizza,
                IsVegan = false
            };

            _mockMenuItemRepository
                .Setup(x => x.GetItemByName("Test Item"))
                .ReturnsAsync((MenuItem)null);

            _mockMenuItemRepository
                .Setup(x => x.AddItem(It.IsAny<MenuItem>()))
                .Returns(Task.CompletedTask);

            // Act
            await _menuItemsService.CreateMenuItem(model);

            // Assert
            _mockMenuItemRepository.Verify(x => x.AddItem(It.IsAny<MenuItem>()), Times.Once);
        }

        [Fact]
        public async Task CreateMenuItem_WithExistingName_ThrowsArgumentException()
        {
            // Arrange
            var model = new CreateMenuItemViewModel
            {
                Name = "Existing Item",
                Price = 10.99f,
                Description = "Test Description",
                Category = MenuItemCategory.Pizza,
                IsVegan = false
            };

            var existingItem = new MenuItem
            {
                Name = "Existing Item",
                Price = 15.99f,
                Description = "Existing Description"
            };

            _mockMenuItemRepository
                .Setup(x => x.GetItemByName("Existing Item"))
                .ReturnsAsync(existingItem);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _menuItemsService.CreateMenuItem(model));
        }

        [Fact]
        public async Task GetAllMenuItems_WithFilters_ReturnsFilteredItems()
        {
            // Arrange
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid(), Name = "Item 1", IsVegan = true, Category = MenuItemCategory.Pizza },
                new MenuItem { Id = Guid.NewGuid(), Name = "Item 2", IsVegan = false, Category = MenuItemCategory.Pizza }
            };

            _mockMenuItemRepository
                .Setup(x => x.GetAllMenuItems(true, new[] { MenuItemCategory.Pizza }))
                .ReturnsAsync(menuItems);

            // Act
            var result = await _menuItemsService.GetAllMenuItems(true, new[] { MenuItemCategory.Pizza });

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Item 1", result[0].Name);
            Assert.Equal("Item 2", result[1].Name);
        }

        [Fact]
        public async Task GetItemModelById_WithValidId_ReturnsItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var menuItem = new MenuItem
            {
                Id = itemId,
                Name = "Test Item",
                Description = "Test Description",
                Price = 10.99f,
                Category = MenuItemCategory.Pizza,
                IsVegan = false,
                PhotoPath = "test.jpg"
            };

            _mockMenuItemRepository
                .Setup(x => x.GetItemById(itemId))
                .ReturnsAsync(menuItem);

            // Act
            var result = await _menuItemsService.GetItemModelById(itemId.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Item", result.Name);
            Assert.Equal(10.99f, result.Price);
        }

        [Fact]
        public async Task GetItemModelById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            _mockMenuItemRepository
                .Setup(x => x.GetItemById(itemId))
                .ReturnsAsync((MenuItem)null);

            // Act
            var result = await _menuItemsService.GetItemModelById(itemId.ToString());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteMenuItem_WithValidId_ReturnsTrue()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var menuItem = new MenuItem
            {
                Id = itemId,
                Name = "Test Item"
            };

            _mockMenuItemRepository
                .Setup(x => x.GetItemById(itemId))
                .ReturnsAsync(menuItem);

            _mockMenuItemRepository
                .Setup(x => x.DeleteItem(menuItem))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _menuItemsService.DeleteMenuItem(itemId.ToString());

            // Assert
            Assert.True(result);
            _mockMenuItemRepository.Verify(x => x.DeleteItem(menuItem), Times.Once);
        }

        [Fact]
        public async Task DeleteMenuItem_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            _mockMenuItemRepository
                .Setup(x => x.GetItemById(itemId))
                .ReturnsAsync((MenuItem)null);

            // Act
            var result = await _menuItemsService.DeleteMenuItem(itemId.ToString());

            // Assert
            Assert.Null(result);
            _mockMenuItemRepository.Verify(x => x.DeleteItem(It.IsAny<MenuItem>()), Times.Never);
        }

        [Fact]
        public async Task GetAddToCartModel_WithValidId_ReturnsModel()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var menuItem = new MenuItem
            {
                Id = itemId,
                Name = "Test Item",
                Description = "Test Description",
                Price = 10.99f,
                Category = MenuItemCategory.Pizza,
                IsVegan = false,
                PhotoPath = "test.jpg"
            };

            _mockMenuItemRepository
                .Setup(x => x.GetItemById(itemId))
                .ReturnsAsync(menuItem);

            // Act
            var result = await _menuItemsService.GetAddToCartModel(itemId.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Item", result.Item.Name);
        }

        [Fact]
        public async Task GetAddToCartModel_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            _mockMenuItemRepository
                .Setup(x => x.GetItemById(itemId))
                .ReturnsAsync((MenuItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _menuItemsService.GetAddToCartModel(itemId.ToString()));
        }
    }
}