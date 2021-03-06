﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaOnLine.Data;
using PizzaOnLine.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace PizzaOnLine.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Order.Include(o => o.Cart);
            //return View(await applicationDbContext.ToListAsync());
            var cartId = HttpContext.Session.GetInt32("CartSession");
            var aUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("User"))
            {
                var newUser = new OrderViweModel
                {
                    User = new ApplicationUser
                    {
                        FirstName = aUser.FirstName,
                        LastName = aUser.LastName,
                        Address = aUser.Address,
                        PostalCode = aUser.PostalCode,
                        City = aUser.City,
                        Email = aUser.Email,
                        PhoneNumber = aUser.PhoneNumber

                    }
                };
                return View(newUser);

            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> OrderPayment()
        {
            //var applicationDbContext = _context.Order.Include(o => o.Cart);
            //return View(await applicationDbContext.ToListAsync());
            var cartId = HttpContext.Session.GetInt32("CartSession");
            var aUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                var newUser = new OrderViweModel
                {
                    User = new ApplicationUser
                    {
                        FirstName = aUser.FirstName,
                        LastName = aUser.LastName,
                        Address = aUser.Address,
                        PostalCode = aUser.PostalCode,
                        City = aUser.City,
                        Email = aUser.Email,
                        PhoneNumber = aUser.PhoneNumber

                    }
                };
                return View(newUser);

            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> OrderPayment(OrderViweModel orderviewmodel)
        {
            var cartId =(int) HttpContext.Session.GetInt32("CartSession");
            Cart cart;
            List<CartItem> cartItems;

            cart = _context.Carts
                .Include(i => i.Cartitems)
                .ThenInclude(x => x.CartItemIngredient)
                .ThenInclude(ig => ig.Ingredient)
                .Include(i => i.Cartitems)
                .ThenInclude(ci => ci.Dish)
                .SingleOrDefault(x => x.CartId == cartId);

            cartItems = cart.Cartitems;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                var newModel = new OrderViweModel
                {
                    User = new ApplicationUser
                    {
                        FirstName = orderviewmodel.User.FirstName,
                        LastName = orderviewmodel.User.LastName,
                        Address = orderviewmodel.User.Address,
                        PostalCode = orderviewmodel.User.PostalCode,
                        City = orderviewmodel.User.City,
                        Email = orderviewmodel.User.Email,
                        PhoneNumber = orderviewmodel.User.PhoneNumber

                    },
                    Myorder = new Order()
                    {
                        CartId = cartId,
                        Cart= cart,
                        CartItem = cartItems,
                        CardName = orderviewmodel.Myorder.CardName,
                        CardNumber = orderviewmodel.Myorder.CardNumber,
                        MMYY = orderviewmodel.Myorder.MMYY,
                        CVC = orderviewmodel.Myorder.CVC
                    }
                };
                return RedirectToAction("OrderConfirmation", newModel);
            }
            else
            {
                var newPayment = new Order()
                {
                    OrderId = orderviewmodel.Myorder.OrderId,
                    FirstName = orderviewmodel.Myorder.FirstName,
                    LastName = orderviewmodel.Myorder.LastName,
                    ShippingAddress = orderviewmodel.Myorder.ShippingAddress,
                    PostalCode = orderviewmodel.Myorder.PostalCode,
                    Email = orderviewmodel.Myorder.Email,
                    PhoneNumber = orderviewmodel.Myorder.PhoneNumber,
                    City = orderviewmodel.Myorder.City,
                    CartId = cartId,
                    Cart = cart,
                    CartItem = cartItems,
                    CardName = orderviewmodel.Myorder.CardName,
                    CardNumber = orderviewmodel.Myorder.CardNumber,
                    MMYY = orderviewmodel.Myorder.MMYY,
                    CVC = orderviewmodel.Myorder.CVC
                };

                await _context.Order.AddAsync(newPayment);
                await _context.SaveChangesAsync();

                return RedirectToAction("OrderConfirmation", newPayment);
            }
        }

        public async Task<ActionResult> OrderConfirmation(int orderId, OrderConfirm orderConfirm)
        {
            var cartId = HttpContext.Session.GetInt32("CartSession");
            var user = await _userManager.GetUserAsync(User);

            if (User.Identity.IsAuthenticated)
            {

                var currentOrder = await _context.Carts

                    .Include(x => x.Cartitems)
                    .ThenInclude(x => x.CartItemIngredient)
                    .ThenInclude(x => x.Ingredient)
                    .Include(x => x.Cartitems)
                    .ThenInclude(x => x.Dish)
                    .SingleOrDefaultAsync(x => x.CartId == cartId);

                var order = new Order()
                {
                    OrderId = orderId
                };


                var newOrder = new OrderConfirm
                {
                    Order = order,
                    User = user,
                    CartItems = currentOrder.Cartitems

                };

                var session = HttpContext.Session;
                session.Remove("CartSession");
                return View(newOrder);
            }
            else
            {
                var currentGuest = await _context.Order
                    .Include(x => x.Cart)
                    .ThenInclude(x => x.Cartitems)
                    .ThenInclude(x => x.CartItemIngredient)
                    .ThenInclude(x => x.Ingredient)
                    .Include(x => x.CartItem)
                    .ThenInclude(x => x.Dish)
                    .SingleOrDefaultAsync(x => x.OrderId == orderId && x.CartId == cartId);

                var currentCart = currentGuest.CartItem.ToList();

                var newOrder = new OrderConfirm
                {
                    Order = currentGuest
                };

                var session = HttpContext.Session;
                session.Remove("CartSession");

                return View(newOrder);
            }
        }



        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Cart)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,FirstName,LastName,Email,ShippingAddress,City,PostalCode,PhoneNumber,CardName,CardNumber,MMYY,CVC,UserId,CartId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", order.CartId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", order.CartId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,FirstName,LastName,Email,ShippingAddress,City,PostalCode,PhoneNumber,CardName,CardNumber,MMYY,CVC,UserId,CartId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", order.CartId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Cart)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.SingleOrDefaultAsync(m => m.OrderId == id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
