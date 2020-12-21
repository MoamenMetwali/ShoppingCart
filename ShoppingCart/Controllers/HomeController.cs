using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Store.Data;
using Store.Models;
using Store.ViewModels;
using Store.Utility;

namespace Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        AppDbContext _db;


        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Products = _db.Products.Include(p=>p.Category),
                Categories = _db.Categories
            };
            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constant.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constant.SessionCart);
            }

            DetailsVM detailsVM = new DetailsVM
            {
                Product = _db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id),
                ExistsInCart = false
            };

            foreach(var item in ShoppingCartList)
            {
                if (item.ProductId == id)
                {
                    detailsVM.ExistsInCart = true;
                }
            }
            return View(detailsVM);
        }
        
        [HttpPost]
        public IActionResult Details(DetailsVM detailsVM) 
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constant.SessionCart).Count()>0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constant.SessionCart);
            }
            ShoppingCartList.Add(new ShoppingCart { ProductId = detailsVM.Product.Id });
            HttpContext.Session.Set(Constant.SessionCart,ShoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constant.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constant.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constant.SessionCart);
            }

            var removedItem = ShoppingCartList.SingleOrDefault(i=>i.ProductId==id);
            if (removedItem != null)
            {
                ShoppingCartList.Remove(removedItem);
            }
            HttpContext.Session.Set(Constant.SessionCart, ShoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
