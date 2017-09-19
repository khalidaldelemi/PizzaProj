using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaOnLine.Data;
using PizzaOnLine.Models;
using PizzaOnLine.Services;
using Microsoft.AspNetCore.Http;

namespace PizzaOnLine.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly DishService _dishService;
        private readonly IngredientService _ingredientService;

        public CartItemsController(ApplicationDbContext context, CartService cartService, DishService dishService, IngredientService ingredientService)
        {
            _context = context;
            _cartService = cartService;
            _dishService = dishService;
            _ingredientService = ingredientService;
        }

        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetInt32("CartSession");
            var applicationDbContext = _context.Carts.Include(c => c.Cartitems).
                ThenInclude(c => c.Dish)
                .Include(x => x.Cartitems)
                .ThenInclude(v => v.CartItemIngredient).
                ThenInclude(i => i.Ingredient)
                .FirstOrDefaultAsync(d => d.CartId == id);

            return View(await applicationDbContext);
        }
        public IActionResult AddToCart(int id)
        {
            var test = _cartService.AddToCart(HttpContext, id);
            return RedirectToAction("Index");
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Dish)
                .SingleOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId");
            ViewData["DishId"] = new SelectList(_context.Dishes, "DishId", "DishId");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartItemId,CartId,DishId,Quantity,DishsName,DishPrice")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            ViewData["DishId"] = new SelectList(_context.Dishes, "DishId", "DishId", cartItem.DishId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems.SingleOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            ViewData["DishId"] = new SelectList(_context.Dishes, "DishId", "DishId", cartItem.DishId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartItemId,CartId,DishId,Quantity,DishsName,DishPrice")] CartItem cartItem, IFormCollection form)
        {
            var cartId = HttpContext.Session.GetInt32("CartSession");
            if (id != cartItem.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _cartService.RemoveIngredientsByDish(id);
                    foreach (var ing in _cartService.AllIngredient())
                    {
                        var cartiteming = new CartItemIngredient
                        {

                            CartItemId = cartItem.CartItemId,
                            IngredientId = ing.IngredientId,
                            IngredeintName = ing.Name,
                            Enabel = form.Keys.Any(k => k == $"Ingredient-{ing.IngredientId}"),
                            CartItemIngredientPrice = +ing.Price,

                        };
                        _context.Add(cartiteming);
                    }

                    //var newPrice = 0;
                    //_context.Update(cartItem);
                    //var oldCartItem = _context.CartItems.Include(d => d.Dish).ThenInclude(x => x.DishIngredient).SingleOrDefault(c => c.CartItemId == id);

                    //foreach (var dishIngredient in oldCartItem.Dish.DishIngredient)
                    //{
                    //    foreach (var ing in cartItem.CartItemIngredient)
                    //    {
                    //        if (ing.IngredientId != dishIngredient.IngredientId)
                    //        {
                    //            newPrice = +5;
                    //        }
                    //    }
                    //}
                    
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.CartItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            ViewData["DishId"] = new SelectList(_context.Dishes, "DishId", "DishId", cartItem.DishId);
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cartItem = await _context.CartItems
               .Include(c => c.Cart)
               .Include(c => c.Dish)
               .SingleOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItems.SingleOrDefaultAsync(m => m.CartItemId == id);
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.CartItemId == id);
        }
    }
}
