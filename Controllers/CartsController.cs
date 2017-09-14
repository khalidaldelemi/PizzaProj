using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaOnLine.Data;
using PizzaOnLine.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PizzaOnLine.Services;
using System.Net.Http;

namespace PizzaOnLine.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartservice;

        public CartsController(ApplicationDbContext context,CartService cartService)
        {
            _context = context;
            _cartservice = cartService;
        }

        // GET: Carts
        public async Task<IActionResult> Index(int id)
        {
            var test = _cartservice.AddToCart(HttpContext, id);

            //var toSession = JsonConvert.SerializeObject(test.ToString());
            //HttpContext.Session.SetString("Cart", toSession);
            //return View(await _context.Carts.ToListAsync());
            //var carList = new List<Cart>();
            //carList.Add(test);
            return View(test);
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .SingleOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }
      

        // GET: Carts/Create
        public IActionResult Create()
        {
            
           
            




                return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,applicationId,DishId")] Cart cart,int id)
        {
            if (ModelState.IsValid)
            {
                _cartservice.AddToCart(HttpContext, id);
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.CartItems.SingleOrDefaultAsync(m => m.CartItemId == id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CartItem cartItem,IFormCollection form)
        {
            Cart cart;
            var cartId = HttpContext.Session.GetInt32("CartSession");
            if (id != cartItem.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _cartservice.RemoveIngredientsByDish(id);
                    foreach (var item in _cartservice.AllIngredient())
                    {
                        if (true)
                        {
                            var cartiteming = new CartItemIngredient
                            {
                                
                                CartItemId= cartItem.CartItemId,
                                IngredientId = item.IngredientId,
                                Enabel = form.Keys.Any(k => k == $"Ingredient-{item.IngredientId}")


                            };
                            _context.Update(cartItem);
                           
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cartItem.CartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit));
            }
            return View();
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .SingleOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.SingleOrDefaultAsync(m => m.CartId == id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }
    }
}
