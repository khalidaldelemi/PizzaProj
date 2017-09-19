using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaOnLine.Models
{
    public class OrderViweModel
    {
        public Cart Mycart { get; set; }
        public ApplicationUser User { get; set; }
        public Order Myorder { get; set; }

    }
}
