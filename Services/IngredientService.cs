using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PizzaOnLine.Data;
using PizzaOnLine.Models;

namespace PizzaOnLine.Services
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;
        public IngredientService (ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Ingredient> AllIngredient()
        {
            return _context.Ingredients.ToList();
        }
        public string IngredentByDish(int id)
        {
            var ing = _context.DishIngredients.Include(In => In.Ingredient).Where(In => In.DishId == id && In.Enabel);
            var dishIngredents = "";
            foreach (var item in ing)
            {
                dishIngredents += item.Ingredient.Name + " ";
            }
            return dishIngredents;
        }
    }
}
