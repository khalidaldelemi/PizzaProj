using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaOnLine.Models
{
    public class OrderConfirm
    {
        [Key]
        public int ConfirmationId { get; set; }
        public Order Order { get; set; }
        public ApplicationUser User { get; set; }

        public List<CartItem> CartItems { get; set; }
    }
}
