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
            var cartId = httpContext.Session.GetInt32("Cart");
            Cart cart;
            var Dish = _context.Dishes.Include(z => z.DishIngredient).
                ThenInclude(i => i.Ingredient)
                .FirstOrDefault(d => d.DishId == id);
            if (httpContext.Session.GetString("Cart") == null)
            {
                var newCart = new Cart
                {
                    Cartitems = new List<CartItem>(),
                };
                _context.Carts.Add(newCart);

                httpContext.Session.SetInt32("Cart", newCart.CartId);
                cartId = newCart.CartId;
            }
            cart = _context.Carts.Include(c => c.Cartitems)
                .ThenInclude(ci => ci.Dish)
                .ThenInclude(cd => cd.DishIngredient)
                .SingleOrDefault(c => c.CartId == cartId);

          CartItem newcartItem = new CartItem
            {
                Dish = Dish,
                Cart = cart,
                CartId = cart.CartId,
                DishPrice = Dish.Price,
                DishsName = Dish.Name,
                Quantity = 1,

            };

            var inglist = new List<CartItemIngredient>();
            foreach (var item in Dish.DishIngredient)
            {
                var cartinger = new CartItemIngredient
                {
                    CartItem = newcartItem,
                    CartItemIngredientPrice = item.Ingredient.Price,
                    CartItemId = newcartItem.CartItemId,
                    Ingredient = item.Ingredient,
                    IngredeintName = item.Ingredient.Name,
                    IngredientId = item.IngredientId,
                    Enabel = item.Enabel,
                };
                inglist.Add(cartinger);

            }
            newcartItem.CartItemIngredient = inglist;
            cart.Cartitems.Add(newcartItem);
            //var toSession = JsonConvert.SerializeObject(newCart);
            //httpContext.Session.SetString("Cart", toSession);



            return cart;

            //        else
            //        {

            //            var existcart = 

            //            var newcartItem = new CartItem
            //            {
            //                Dish = Dish,
            //                DishPrice = Dish.Price,
            //                DishsName = Dish.Name,

            //            };
            //            //var inglist = new List<CartItemIngredient>();
            //            //foreach (var item in Dish.DishIngredient)
            //            //{
            //            //    var cartinger = new CartItemIngredient
            //            //    {
            //            //        CartItem = newcartItem,
            //            //        CartItemIngredientPrice = item.Ingredient.Price,
            //            //        CartItemId = newcartItem.CartItemId,
            //            //        Ingredient = item.Ingredient,
            //            //        IngredeintName = item.Ingredient.Name,
            //            //        IngredientId = item.IngredientId,
            //            //        Enabel = true,
            //            //    };
            //            //    inglist.Add(cartinger);

            //            //}
            //            //newcartItem.CartItemIngredient = inglist;
            //            existcart.Cartitems.Add(newcartItem);
            //            _context.SaveChanges();
            //            return existcart;
            //        }

            //    }
            //    //public List<CartItem> GetCart (HttpContext httpContext)
            //    //{
            //    //    List<CartItem> _cartItem = new List<CartItem>();
            //    //    var cartSession = httpContext.Session.GetInt32("Cart");

            //    //    if (cartSession ==null)
            //    //    {
            //    //        var cart = _context.Carts.ToList();
            //    //        var id = httpContext.Session.GetInt32("Cart");
            //    //        id = cart.Count + 1;
            //    //        _cartItem = new List<CartItem>();
            //    //        return null;

            //    //    }
            //    //    else
            //    //    {
            //    //        Cart cart = new Cart();
            //    //        var cartId = httpContext.Session.GetInt32("Cart");
            //    //        cart = _context.Carts.Include(c => c.Cartitems).
            //    //            ThenInclude(s => s.Dish).SingleOrDefault(x => x.CartId == cartId);
            //    //        _cartItem = cart.Cartitems;


            //    //    }
            //    //    return _cartItem;
            //    //}
            //}
        }
    }
}
