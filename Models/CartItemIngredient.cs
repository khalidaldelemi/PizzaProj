using System;

namespace PizzaOnLine.Models
{
    public class CartItemIngredient
    {
        public Guid CartItemId { get; set; }
        public CartItem CartItem { get; set; }
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public bool Enabel { get; set; }
        public string IngredeintName { get; set; }
        public decimal CartItemIngredientPrice { get; set; }




    }
}