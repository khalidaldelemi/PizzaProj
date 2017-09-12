using System.Collections.Generic;

namespace PizzaOnLine.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<DishIngredient> DishIngredients { get; set; }
        public List<CartItemIngredient> cartItemIngredient { get; set; }

    }
}