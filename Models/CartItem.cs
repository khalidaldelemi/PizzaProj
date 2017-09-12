using System;
using System.Collections.Generic;

namespace PizzaOnLine.Models
{
    public class CartItem
    {
        public Guid CartItemId { get; set; }
        public Cart Cart { get; set; }
        public int CartId { get; set; }
        public Dish Dish { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }
        public List<CartItemIngredient> CartItemIngredient { get; set; }
        public string DishsName { get; set; }
        public int DishPrice { get; set; }



    }
}