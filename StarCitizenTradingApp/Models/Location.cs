using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarCitizenTradingApp.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Commodity> PurchaseCommodities { get; set; }
        public List<Commodity> SellCommodities { get; set; }

    }
}