using StarCitizenTradingApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StarCitizenTradingApp
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<Commodity> Commodities { get; set; }
        public DbSet<Ship> Ships { get; set; }

        public ApplicationDbContext() 
            : base("StarCitizenTradingApp")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<StarCitizenTradingApp.Models.InputVM> InputVMs { get; set; }

        public System.Data.Entity.DbSet<StarCitizenTradingApp.Models.RouteVM> RouteVMs { get; set; }
    }
}