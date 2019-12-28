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
            List<SelectListItem> stops = new List<SelectListItem>();
            SelectListItem one = new SelectListItem() { Text = "One", Value = "1" };
            SelectListItem two = new SelectListItem() { Text = "Two", Value = "2" };
            SelectListItem three = new SelectListItem() { Text = "Three", Value = "3" };
            stops.Add(one);
            stops.Add(two);
            stops.Add(three);

            ViewBag.Stops = stops;

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
            if (input.Stops == 1)
            {
                var shipid = input.ShipId;
                var ship = db.Ships.FirstOrDefault(s => s.Id == shipid);
                var capital = input.Capital;
                var routes = GetRoutes(ship, capital);
                var mostProfitableRoutes = GetMostProfitableRoutes(routes);

                return View("Result", mostProfitableRoutes);
            }

            return RedirectToAction("GetLoops", input);
        }        

        public ActionResult GetLoops(InputVM input)
        {            
            var shipid = input.ShipId;
            var ship = db.Ships.FirstOrDefault(s => s.Id == shipid);
            var capital = input.Capital;
            var loops = GetLoopList(ship, capital, input.Stops);
            var mostProfitableLoops = GetMostProfitableLoops(loops);

            return View("LoopList", mostProfitableLoops);
        }

        public IEnumerable<LoopVM> GetLoopList(Ship ship, double capital, int stops)
        {
            List<LoopVM> loops = new List<LoopVM>();
            var routes = GetRoutes(ship, capital);

            foreach (var route in routes)
            {                                
                var availableSecondaryCommodities = GetPurchaseCommodities(route.SellLocation);
                
                foreach (var secondaryCommodity in availableSecondaryCommodities)
                {                                        
                    var secondaryPurchaseLocations = GetPurchaseLocations(secondaryCommodity);                    
                    var secondaryRoutes = GetRoutesFromLocations
                        (secondaryPurchaseLocations, ship, capital + route.Profit);

                    foreach (var secondaryRoute in secondaryRoutes)
                    {
                        List<RouteVM> routeList = new List<RouteVM>();

                        if (stops == 2)
                        {
                            if (route.SellLocation == secondaryRoute.PurchaseLocation &&
                            secondaryRoute.SellLocation == route.PurchaseLocation)
                            {
                                routeList.Clear();
                                routeList.Add(route);
                                routeList.Add(secondaryRoute);

                                LoopVM loop = new LoopVM()
                                {
                                    Routes = routeList,
                                    Profit = Math.Round(route.Profit + secondaryRoute.Profit)
                                };
                                
                                if (loops.Count == 0 ||
                                    !loops.Any(l =>
                                    l.Routes.ElementAt(0).Commodity.Id == route.Commodity.Id &&
                                    l.Routes.ElementAt(1).Commodity.Id == secondaryRoute.Commodity.Id) &&
                                    !loops.Any(l =>
                                    l.Routes.ElementAt(1).Commodity.Id == route.Commodity.Id &&
                                    l.Routes.ElementAt(0).Commodity.Id == secondaryRoute.Commodity.Id))
                                {
                                    loops.Add(loop);
                                }
                            }
                        }

                        if (stops == 3)
                        {
                            var availableTertiaryCommodities = GetPurchaseCommodities(secondaryRoute.SellLocation);

                            foreach (var tertiaryCommodity in availableTertiaryCommodities)
                            {
                                var tertiaryPurchaseLocations = GetPurchaseLocations(tertiaryCommodity);
                                var tertiaryRoutes = GetRoutesFromLocations
                                    (tertiaryPurchaseLocations, ship, capital + route.Profit + secondaryRoute.Profit);

                                foreach (var tertiaryRoute in tertiaryRoutes)
                                {                                    
                                    if (route.SellLocation == secondaryRoute.PurchaseLocation &&
                                        secondaryRoute.SellLocation == tertiaryRoute.PurchaseLocation &&
                                        tertiaryRoute.SellLocation == route.PurchaseLocation)
                                    {
                                        routeList.Clear(); //this seems to be necessary
                                        routeList.Add(route);
                                        routeList.Add(secondaryRoute);
                                        routeList.Add(tertiaryRoute);

                                        LoopVM loop = new LoopVM()
                                        {
                                            Routes = routeList,
                                            Profit = Math.Round(route.Profit + secondaryRoute.Profit + tertiaryRoute.Profit)
                                        };
                                        
                                        if (loops.Count == 0 ||
                                            !loops.Any(l =>
                                            l.Routes.ElementAt(0).Commodity.Id == route.Commodity.Id &&
                                            l.Routes.ElementAt(1).Commodity.Id == secondaryRoute.Commodity.Id &&
                                            l.Routes.ElementAt(2).Commodity.Id == tertiaryRoute.Commodity.Id) &&
                                            !loops.Any(l =>
                                            l.Routes.ElementAt(1).Commodity.Id == route.Commodity.Id &&
                                            l.Routes.ElementAt(2).Commodity.Id == secondaryRoute.Commodity.Id &&
                                            l.Routes.ElementAt(0).Commodity.Id == tertiaryRoute.Commodity.Id) &&
                                            !loops.Any(l =>
                                            l.Routes.ElementAt(2).Commodity.Id == route.Commodity.Id &&
                                            l.Routes.ElementAt(0).Commodity.Id == secondaryRoute.Commodity.Id &&
                                            l.Routes.ElementAt(1).Commodity.Id == tertiaryRoute.Commodity.Id))
                                        {
                                            loops.Add(loop);
                                        }
                                    }
                                }
                            }
                        }                                                                        
                    }                    
                }                
            }

            return loops;
        }

        public List<RouteVM> GetRoutes(Ship ship, double capital)
        {                        
            var locations = db.Locations.ToList();                        
            locations = GetLocationData(locations);            
            var routes = GetRoutesFromLocations(locations, ship, capital);                        

            return routes;
        }

        public List<Location> GetLocationData(List<Location> locations)
        {
            foreach (var location in locations)
            {
                location.PurchaseCommodities = GetPurchaseCommodities(location);
                location.SellCommodities = GetSellCommodities(location);
            }

            return locations;
        }

        public List<RouteVM> GetRoutesFromLocations(List<Location> locations, Ship ship, double capital)
        {
            List<RouteVM> routes = new List<RouteVM>();
            var sellLocationsWithData = GetLocationData(locations);

            foreach (var location in sellLocationsWithData)
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

                        var totalPurchaseCost = route.Commodity.PurchaseCost * route.Quantity;
                        var sellPrice = GetSellCommodities(sellLocation)
                            .FirstOrDefault(c => c.Name == route.Commodity.Name).SellPrice;
                        var totalSellPrice = sellPrice * route.Quantity;
                        var profit = totalSellPrice - totalPurchaseCost;
                        route.Profit = profit;

                        routes.Add(route);
                    }
                }
            }

            return routes;
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

        public List<Location> GetPurchaseLocations (Commodity commodity)
        {
            var commodities = db.Commodities.ToList();
            List<Location> purchaseLocations = new List<Location>();

            foreach (var cmdty in commodities)
            {
                if (cmdty.Name == commodity.Name && cmdty.PurchaseCost != 0)
                {
                    var location = db.Locations.FirstOrDefault(l => l.Id == cmdty.LocationId);
                    purchaseLocations.Add(location);
                }
            }

            return purchaseLocations;
        }

        public IEnumerable<RouteVM> GetMostProfitableRoutes(List<RouteVM> routes)
        {
            Dictionary<RouteVM, double> routesWithProfit = new Dictionary<RouteVM, double>();
            List<RouteVM> mostProfitableRoutes = new List<RouteVM>();

            foreach (var route in routes)
            {                
                routesWithProfit.Add(route, route.Profit);
            }

            var topThree = routesWithProfit.OrderByDescending(r => r.Value).Take(3).ToList();

            foreach (var item in topThree)
            {
                mostProfitableRoutes.Add(item.Key);
            }            

            return mostProfitableRoutes;
        }          
        
        public IEnumerable<LoopVM> GetMostProfitableLoops(IEnumerable<LoopVM> loops)
        {
            Dictionary<LoopVM, double> loopsWithProfit = new Dictionary<LoopVM, double>();
            List<LoopVM> mostProfitableLoops = new List<LoopVM>();

            foreach (var loop in loops)
            {
                loopsWithProfit.Add(loop, loop.Profit);
            }

            var topTree = loopsWithProfit.OrderByDescending(l => l.Value).Take(3).ToList();

            foreach (var item in topTree)
            {
                mostProfitableLoops.Add(item.Key);
            }

            return mostProfitableLoops;
        }
    }
}