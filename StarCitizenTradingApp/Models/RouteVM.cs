using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarCitizenTradingApp.Models
{
    public class RouteVM
    {
        [Key]
        public int Id { get; set; }
        public Location PurchaseLocation { get; set; }
        public Location SellLocation { get; set; }
        public Commodity Commodity { get; set; }
        public double Quantity { get; set; }
        public double Profit { get; set; }
    }
}