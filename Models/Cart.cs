
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaOnLine.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public ApplicationUser applicationUser { get; set; }
        public int applicationId { get; set; }
        public List<CartItem> Cartitems { get; set; } = new List<CartItem>();
      
    }
}
