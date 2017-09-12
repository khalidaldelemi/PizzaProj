using PizzaOnLine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaOnLine.Services
{
    public class DishService
    {
        private readonly ApplicationDbContext _context;
        public DishService (ApplicationDbContext context)
        {
            _context = context;
        }
        public bool HasIngerdient(int id,int ingredientid)
        {
            var check = _context.DishIngredients.Any(z => z.DishId == id && z.IngredientId == ingredientid);
            return check;
        }
    }
}
