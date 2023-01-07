using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvertisingAgency.Models.ViewModels.Cart
{
    public class CartVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Total
        {
            get { return Quantity * Price; }
        }
        public string Image { get; set; }
    }
}