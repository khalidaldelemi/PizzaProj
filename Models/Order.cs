using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaOnLine.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }

        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string MMYY { get; set; }
        public string CVC { get; set; }

        public int UserId { get; set; }
        // public ApplicationUser User { get; set; }
        public Cart Cart { get; set; }
        public int CartId { get; set; }

       

        public List<CartItem> CartItem { get; set; }


    }
}
