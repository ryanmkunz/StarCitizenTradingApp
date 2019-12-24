using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarCitizenTradingApp.Models
{
    public class InputVM
    {
        [Key]
        public int Id { get; set; }
        public int ShipId { get; set; }
        public double Capital { get; set; }
    }
}