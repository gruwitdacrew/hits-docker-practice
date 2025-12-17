using Mockups.Storage;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Mockups.Configs
{
    public static class ConfigureIdentity
    {
        public static async Task ConfigureIdentityAsync(this WebApplication app) // ключевое слово this в аргументе метода обозначает, что это будет метод расширения
        {
            using var serviceScope = app.Services.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>(); // регистрация пользователей проходит при помощи UserManager
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>(); // создание ролей происходит через RoleManager
            var config = app.Configuration.GetSection("AdminCreds");
            var adminRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Administrator); // Пытаемся найти роль админа
            if (adminRole == null) // Если ее еще нет в БД, создаем
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = ApplicationRoleNames.Administrator,
                    Type = RoleType.Administrator
                });
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {ApplicationRoleNames.Administrator} role.");
                }
                adminRole = await roleManager.FindByNameAsync(ApplicationRoleNames.Administrator); // После создания получаем роль еще раз
            }
            var adminUser = await userManager.FindByNameAsync(config["Email"]); // Достаем из БД пользователя админа
            if (adminUser == null) // Если нет, создаем
            {
                var userResult = await userManager.CreateAsync(new User
                {
                    UserName = config["Email"],
                    Email = config["Email"],
                    Name = "Cist Yana",
                    BirthDate = new DateTime(1990, 1, 1),
                    Phone = "88005553535"
                }, config["Password"]); // Стандарный метод регистрации пользователя через создание сущности пользователя и его пароля
                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {config["Email"]} user");
                }
                adminUser = await userManager.FindByNameAsync(config["Email"]);
            }
            if (!await userManager.IsInRoleAsync(adminUser, adminRole.Name)) // Если пользователь админ еще не был назначен на роль админа, назначаем
            {
                await userManager.AddToRoleAsync(adminUser, adminRole.Name); // назначение на роль происходит через сущность пользователя и название роли (строка). Если при обычной процедуре регистрации нужно назначить роль обычного пользователя к новому зарегистрированному, необходимо будет после регистрации использовать данный метод для назначения пользователя на роль.
            }
            var userRole = await roleManager.FindByNameAsync(ApplicationRoleNames.User);
            if (userRole == null) // создание роли обычного пользователя
            {
                var roleResult = await roleManager.CreateAsync(new Role
                {
                    Name = ApplicationRoleNames.User,
                    Type = RoleType.User
                });
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {ApplicationRoleNames.User} role.");
                }
            }

            // Add sample menu items if none exist
            await AddSampleMenuItemsAsync(serviceScope);
        }

        private static async Task AddSampleMenuItemsAsync(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            // Check if there are any menu items already
            if (!context.MenuItems.Any())
            {
                var sampleItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = Guid.NewGuid(),
                        Name = "Борщ",
                        Price = 150.0f,
                        Description = "Традиционный украинский борщ с капустой, свеклой и мясом",
                        Category = MenuItemCategory.Soup,
                        IsVegan = false,
                        PhotoPath = "/images/borsch.jpg"
                    },
                    new MenuItem
                    {
                        Id = Guid.NewGuid(),
                        Name = "Овощной суп",
                        Price = 120.0f,
                        Description = "Вегетарианский суп из свежих овощей",
                        Category = MenuItemCategory.Soup,
                        IsVegan = true,
                        PhotoPath = "/images/vegetable_soup.jpg"
                    },
                    new MenuItem
                    {
                        Id = Guid.NewGuid(),
                        Name = "Стейк из говядины",
                        Price = 450.0f,
                        Description = "Сочный стейк из говядины премиум класса",
                        Category = MenuItemCategory.WOK,
                        IsVegan = false,
                        PhotoPath = "/images/beef_steak.jpg"
                    },
                    new MenuItem
                    {
                        Id = Guid.NewGuid(),
                        Name = "Паста Карбонара",
                        Price = 320.0f,
                        Description = "Итальянская паста с беконом и соусом карбонара",
                        Category = MenuItemCategory.WOK,
                        IsVegan = false,
                        PhotoPath = "/images/carbonara.jpg"
                    },
                    new MenuItem
                    {
                        Id = Guid.NewGuid(),
                        Name = "Фруктовый салат",
                        Price = 200.0f,
                        Description = "Свежие фрукты в сезонном ассортименте",
                        Category = MenuItemCategory.Dessert,
                        IsVegan = true,
                        PhotoPath = "/images/fruit_salad.jpg"
                    }
                };

                await context.MenuItems.AddRangeAsync(sampleItems);
                await context.SaveChangesAsync();
            }
        }

    }
}
