using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PizzaOnLine.Models;

namespace PizzaOnLine.Data
{
    public static class DbIntialize
    {
        public static void Initialize(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            //var aUser = new ApplicationUser
            //{
            //    UserName = "Test@test.com",
            //    Email = "Test@test.com"
            //};
            //var userResult = userManager.CreateAsync(aUser, "Pa$$w0rd").Result;

            //var adminRole = new IdentityRole { Name = "Admin" };
            //var roleResult = roleManager.CreateAsync(adminRole).Result;

            //var adminUser = new ApplicationUser
            //{
            //    UserName = "admin@test.com",
            //    Email = "admin@test.com"
            //};
            //var adminUserResult = userManager.CreateAsync(adminUser, "Pa$$w0rd").Result;

            //var roleAddedResult = userManager.AddToRoleAsync(adminUser, "Admin").Result;
            CreateUser(userManager, roleManager);
            NewAdmin(userManager, roleManager);

            if (!context.Dishes.Any())
            {
                var pizza = new Category { Name = "Pizza" };
                var pasta = new Category { Name = "Pasta" };
                var salad = new Category { Name = "Salad" };

                
                var cheese = new Ingredient { Name = "Cheese" ,Price= 5 };
                var tomato = new Ingredient { Name = "Tomato" ,Price=5};
                var ham = new Ingredient { Name = "Ham" ,Price=5 };
                var bannan = new Ingredient { Name = "Bannan", Price = 5 };
                var mashrom = new Ingredient { Name = "Mashroom", Price =5};
                var isberg = new Ingredient { Name = "romansalad", Price = 5};
        
                var capricciosa = new Dish { Name = "Capricciosa", Price = 80 ,Category=pizza };
                var margarita = new Dish { Name = "Margarita", Price = 70, Category = pizza };
                var hawaii = new Dish { Name = "Hawaii", Price = 100, Category = pizza };
                var sAlad = new Dish { Name = "Salad", Price = 50, Category = salad };

                var margaritaCheese = new DishIngredient { Dish = margarita, Ingredient = cheese, Enabel = true };
                var margaritaTomato= new DishIngredient { Dish = margarita, Ingredient = tomato , Enabel = true };
                var romanSalad = new DishIngredient { Dish = sAlad, Ingredient = isberg , Enabel = true };

                var hawaiiHam = new DishIngredient { Dish = hawaii, Ingredient = ham, Enabel = true };
                var hawaiiBannan = new DishIngredient { Dish = hawaii, Ingredient = bannan, Enabel = true };
                var hawaiiTomato = new DishIngredient { Dish = hawaii, Ingredient = tomato, Enabel = true };
                var hawaiiCheese = new DishIngredient { Dish = hawaii, Ingredient = cheese, Enabel = true };

                var capricciosaCheese = new DishIngredient { Dish = capricciosa, Ingredient = cheese , Enabel = true };
                var capricciosaHam = new DishIngredient { Dish = capricciosa, Ingredient = ham, Enabel = true };
                var capricciosaTomato = new DishIngredient { Dish = capricciosa, Ingredient = tomato, Enabel = true };
                pizza.dishes = new List<Dish>
                 {
                     capricciosa,margarita,hawaii
                 };

                capricciosa.DishIngredient = new List<DishIngredient> {capricciosaCheese, capricciosaTomato,capricciosaHam};
                sAlad.DishIngredient = new List<DishIngredient> { romanSalad };

                salad.dishes = new List<Dish> { sAlad };
                margarita.DishIngredient = new List<DishIngredient>{ margaritaCheese,margaritaTomato};
                hawaii.DishIngredient = new List<DishIngredient> { hawaiiCheese, hawaiiHam, hawaiiBannan, hawaiiTomato };

                context.AddRange(tomato, ham, cheese, isberg,mashrom);

                context.AddRange(pizza, pasta, salad);

                
                context.DishIngredients.AddRange(margaritaCheese, margaritaTomato,
                                                  hawaiiBannan, hawaiiTomato, hawaiiCheese, hawaiiHam,
                                                 capricciosaHam, capricciosaCheese, capricciosaTomato, romanSalad);
                context.AddRange(capricciosa, margarita, hawaii,sAlad);
                context.SaveChanges();
            }
        }
        public static void NewAdmin(UserManager<ApplicationUser> userManager ,RoleManager<IdentityRole> roleManager)
        {
            var admin = new IdentityRole { Name = "Admin" };
            var roleResult = roleManager.CreateAsync(admin).Result;
            var adminUser = new ApplicationUser
            {
                UserName = "admin@test.se",
                Email = "admin.test.se",
                FirstName = "khalid",
                LastName = "Ali",
                City = "Stockholm",
                Address = "servitutsvägen 13",
                PostalCode = "14559",
                PhoneNumber = "123456",


            };
            var adminResult = userManager.CreateAsync(adminUser, "Password12").Result;
            userManager.AddPasswordAsync(adminUser, "Admin");
        }
        private static void CreateUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var userRole = new IdentityRole { Name = "User" };
            var userResult = roleManager.CreateAsync(userRole).Result;
            var user = new ApplicationUser
            {
                UserName = "student@test.com",
                Email = "student@test.com",
                PhoneNumber = "123456",
                Address = "storgaten 1",
                PostalCode = "12345",
                City = "Stockholm",
                FirstName = "Student",
                LastName = "Test"
            };

            var userUserResult = userManager.CreateAsync(user, "Test1234!").Result;
            userManager.AddToRoleAsync(user, "User");
        }

    }
}
