using StarCitizenTradingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarCitizenTradingApp.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return RedirectToAction("Input");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            
            return View();
        }
        
        [HttpGet]
        public ActionResult Input()
        {
            var ships = db.Ships.ToList();
            List<SelectListItem> shipList = new List<SelectListItem>();

            foreach (var ship in ships)
            {
                var shipListItem = new SelectListItem()
                {
                    Text = ship.Name,
                    Value = ship.Id.ToString()
                };

                shipList.Add(shipListItem);
            }

            ViewBag.Ships = shipList;

            return View();
        }

        [HttpPost]
        public ActionResult Input(InputVM input)
        {
            var shipid = input.ShipId;
            var ship = db.Ships.FirstOrDefault(s => s.Id == shipid);
            var capital = input.Capital;

            var routes = GetRoutes(ship, capital);

            return View("Result", routes);
        }        

        public IEnumerable<RouteVM> GetRoutes(Ship ship, double capital)
        {            
            //get all locations
            var locations = db.Locations.ToList();

            //attach commodities (buy/sell) to locations            
            foreach (var location in locations)
            {
                location.PurchaseCommodities = GetPurchaseCommodities(location);
                location.SellCommodities = GetSellCommodities(location);
            }

            //make list of every route for every commodity
            List<RouteVM> routes = new List<RouteVM>();

            foreach (var location in locations)
            {
                foreach (var purchaseCommodity in location.PurchaseCommodities)
                {                    
                    var sellLocations = GetSellLocations(purchaseCommodity);

                    foreach (var sellLocation in sellLocations)
                    {
                        double quantity;

                        if (capital / purchaseCommodity.PurchaseCost > ship.Capacity)
                        {
                            quantity = ship.Capacity;
                        }
                        else
                        {
                            quantity = capital / purchaseCommodity.PurchaseCost;
                        }

                        RouteVM route = new RouteVM()
                        {                            
                            Commodity = purchaseCommodity,
                            PurchaseLocation = location,
                            SellLocation = sellLocation,
                            Quantity = quantity
                        };                        

                        routes.Add(route);
                    }                    
                }
            }

            //find most profitable route
            var mostProfitableRoutes = GetMostProfitableRoutes(routes);

            return mostProfitableRoutes;
        }

        public List<Commodity> GetPurchaseCommodities(Location location)
        {
            List<Commodity> purchaseCommodities = new List<Commodity>();
            var commodities = db.Commodities.ToList();

            foreach (var commodity in commodities)
            {
                if (commodity.LocationId == location.Id && commodity.PurchaseCost != 0)
                {
                    purchaseCommodities.Add(commodity);
                }
            }

            return purchaseCommodities;
        }

        public List<Commodity> GetSellCommodities(Location location)
        {
            List<Commodity> sellCommodities = new List<Commodity>();
            var commodities = db.Commodities.ToList();

            foreach (var commodity in commodities)
            {
                if (commodity.LocationId == location.Id && commodity.SellPrice != 0)
                {
                    sellCommodities.Add(commodity);
                }
            }

            return sellCommodities;
        }

        public List<Location> GetSellLocations (Commodity commodity)
        {
            var commodities = db.Commodities.ToList();
            List<Location> sellLocations = new List<Location>();

            foreach (var cmdty in commodities)
            {
                if (cmdty.Name == commodity.Name && cmdty.SellPrice != 0)
                {
                    var location = db.Locations.FirstOrDefault(l => l.Id == cmdty.LocationId);
                    sellLocations.Add(location);
                }
            }

            return sellLocations;
        }

        public IEnumerable<RouteVM> GetMostProfitableRoutes(List<RouteVM> routes)
        {
            Dictionary<double, RouteVM> routesWithProfit = new Dictionary<double, RouteVM>();
            List<RouteVM> mostProfitableRoutes = new List<RouteVM>();

            foreach (var route in routes)
            {
                var totalPurchaseCost = route.Commodity.PurchaseCost * route.Quantity;                
                var sellCommodity = route.SellLocation.SellCommodities.FirstOrDefault(c => c.Id == route.Commodity.Id);
                var sellPrice = db.Commodities.FirstOrDefault(c => c.Name == route.Commodity.Name && c.SellPrice != 0).SellPrice;                
                var totalSellPrice = sellPrice * route.Quantity;
                var profit = totalSellPrice - totalPurchaseCost;
                route.Profit = profit;

                routesWithProfit.Add(profit, route);
            }

            var topThree = routesWithProfit.OrderByDescending(r => r.Key).Take(3).ToList();

            foreach (var item in topThree)
            {
                mostProfitableRoutes.Add(item.Value);
            }            

            return mostProfitableRoutes;
        }
        
    }
}