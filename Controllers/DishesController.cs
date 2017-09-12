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
    public class DishesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IngredientService _ingredient;

        public DishesController(ApplicationDbContext context,IngredientService ingredientService)
        {
            _context = context;
            _ingredient = ingredientService;
        }

        // GET: Dishes
        public async Task<IActionResult> Index()
        {
            var catlist = _context.Categories.ToList();

            var dish = await _context.Categories.Include(c => c.dishes)
                .ThenInclude(d => d.DishIngredient)                
                .ThenInclude(i => i.Ingredient).ToListAsync();

            return View(dish);
        }

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .Include(d => d.DishIngredient)
                .ThenInclude(di => di.Ingredient)
                .SingleOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dishes/Create
        public IActionResult Create()
        {
            //ViewData["ExtraList"] = new SelectList(_context.Ingredients, "IngredientId", "Name");
            //var dish = 0; //= await _context.Dishes.SingleOrDefaultAsync(m => m.DishId == id);
            ViewData["CatList"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Dishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishId,Name,Price, CategoryId")] Dish dish, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                //var newDish = new Dish
                //{
                //    DishId = dish.DishId,
                //    Name = dish.Name,
                //    Price = dish.Price,
                //    CategoryId = category.CategoryId
                foreach (var ingredient in _ingredient.AllIngredient())
                {
                    var dishIngredient = new DishIngredient
                    {
                        Ingredient = ingredient,
                        Dish = dish,
                        Enabel = form.Keys.Any(x => x == $"checkboxes-{ingredient.IngredientId}")
                    };
                    _context.DishIngredients.Add(dishIngredient);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var dish = await _context.Dishes.Include(di=>di.DishIngredient).
                ThenInclude(d=>d.Ingredient).
                SingleOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }
            ViewData["Catlist"] = new SelectList(_context.Categories, "CategoryId", "Name", dish.CategoryId);

            return View(dish);
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DishId,Name,Price,CategoryId")] Dish dish,IFormCollection form)
        {
            if (id != dish.DishId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _ingredient.RemoveIngredientsByDish(id);
                    foreach (var item in _ingredient.AllIngredient())
                    {
                        if (true)
                        {
                            var newDish = new DishIngredient
                            {
                                DishId=id,
                                IngredientId = item.IngredientId,
                                Enabel=form.Keys.Any(k=> k== $"Ingredient-{item.IngredientId}")


                            };
                            _context.Add(newDish);
                        }

                    }
                
                  
                  
                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.DishId))
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
            ViewData["Catlist"] = new SelectList(_context.Categories, "CategoryId", "Name", dish.CategoryId);
         
            return View(dish);
        }

        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .SingleOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.SingleOrDefaultAsync(m => m.DishId == id);
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.DishId == id);
        }
    }
}
