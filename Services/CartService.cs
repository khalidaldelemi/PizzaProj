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
        private readonly IngredientService _ingredientService;
        public CartService(ApplicationDbContext context, IServiceProvider serviceProvider,IngredientService ingredientService)
        {
            _context = context;
            _ingredientService = ingredientService;
        }


        public Cart AddToCart(HttpContext httpContext, int id)
        {
            var Con = 0;
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
            Con++;
            var inglist = new List<CartItemIngredient>();
            foreach (var item in dish.DishIngredient)
            {
                var cartinger = new CartItemIngredient
                {
                    CartItem = cartItem,
                    CartItemIngredientPrice = item.Ingredient.Price,
                    CartItemId = cartItem.CartItemId,
                    Ingredient = item.Ingredient,
                    IngredeintName = item.Ingredient.Name,
                    IngredientId = item.IngredientId,
                    Enabel = item.Enabel,
                };
                inglist.Add(cartinger);
            }

            cartItem.CartItemIngredient = inglist;

            cart.Cartitems.Add(cartItem);
            if (cart.CartId != 0)
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
        public List<Ingredient> AllIngredient()
        {
            return _context.Ingredients.ToList();
        }
        public bool HasIngerdient(int id, int ingredientid)
        {
            var check = _context.CartItemIngredients.Any(z => z.CartItemId == id && z.IngredientId == ingredientid && z.Enabel);
            return check;
        }

        internal void RemoveIngredientsByDish(int id)
        {

            var dishIng = _context.CartItemIngredients.Where(d => d.CartItemId == id);
            foreach (var item in dishIng)
            {


                _context.Remove(item);

            }


            _context.SaveChanges();

        }

        public string IngredentByCartItem(int id)
        {
            var ing = _context.CartItemIngredients.Include(In => In.Ingredient).Where(In => In.CartItemId == id && In.Enabel);
            var dishIngredents = "";
            foreach (var item in ing)
            {
                dishIngredents += item.Ingredient.Name + " ";
            }
            return dishIngredents;
        }
            public decimal? CalculateCartSum(int cartItemId)
        {
            decimal? totalPrice = 0;

            decimal? price = _context.CartItems.Where(i => i.CartItemId == cartItemId)
                      .Select(q => q.Quantity * q.Dish.Price).Sum();

            foreach (var itemPrice in _context.CartItems)
            {
                totalPrice += itemPrice.Dish.Price;

                totalPrice += itemPrice.CartItemIngredient.Where(s => s.Enabel).Sum(cii => itemPrice.Dish
                    .DishIngredient.Any(di => di.IngredientId == cii.IngredientId) ? 0 : cii.Ingredient.Price);
            }
            return totalPrice;

        }
        public decimal CartItemPrice(int cartItemId)
        {
            return 111;
        }
        public decimal CartItemPrice(int cartItemId, List<CartItemIngredient> cartItemIngredients, int dishID)
        {

            var dishIngredients = _ingredientService.ingredwnt(dishID);
            var ingredient = dishIngredients.Select(x => x.Ingredient).ToList();

            var addedIngredients = cartItemIngredients.Where(c => c.Enabel).Select(v => v.Ingredient).ToList();

            addedIngredients.RemoveAll(a => ingredient.Contains(a));
            var price = 0;
            foreach (var item in addedIngredients)
            {
                price += Convert.ToInt32(item.Price);
            }

            var newPrice = _context.CartItems.Where(d => d.CartItemId == cartItemId).Select(c => c.Dish.Price + price).FirstOrDefault();
            return newPrice;
        }
        public decimal CalculetPrisOrder(int cartitem)
        {
            var totalOrder = _context.CartItems.Where(a => a.CartItemId == cartitem).Sum(x => x.DishPrice);

            return totalOrder;
        }
    }
}
