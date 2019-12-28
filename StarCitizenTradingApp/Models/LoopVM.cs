using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarCitizenTradingApp.Models
{
    public class LoopVM
    {
        [Key]
        public int Id { get; set; }
        public IEnumerable<RouteVM> Routes { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0}")]
        public double Profit { get; set; }
    }
}