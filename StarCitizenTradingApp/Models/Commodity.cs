using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StarCitizenTradingApp.Models
{
    public class Commodity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double PurchaseCost { get; set; }
        public double SellPrice { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }
    }
}