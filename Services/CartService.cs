using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PizzaOnLine.Data;
using PizzaOnLine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaOnLine.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;
        public CartService(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
        }


        public Cart AddToCart(HttpContext httpContext, int id)
        {
            Cart cart = new Cart();
            var dish = _context.Dishes.Include(z => z.DishIngredient).
                ThenInclude(i => i.Ingredient)
                .FirstOrDefault(d => d.DishId == id);

            if (httpContext.Session.GetString("CartSession") != null)
            {
                var cartId = httpContext.Session.GetInt32("CartSession");
                cart = _context.Carts.Where(x => x.CartId == cartId).Include(i => i.Cartitems).SingleOrDefault();
            }

            var cartItem = new CartItem()
            {
                Cart = cart,
                Dish = dish,
                DishId = dish.DishId,
                Quantity = 1,
                DishPrice = dish.Price,
                DishsName = dish.Name
            };

            cart.Cartitems.Add(cartItem);
            if(cart.CartId != 0)
            {
                _context.Carts.Update(cart);
            }
            else
            {
                _context.Carts.Add(cart);
            }

            _context.SaveChanges();
            httpContext.Session.SetInt32("CartSession", cart.CartId);
            return cart;
        }
    }
}
