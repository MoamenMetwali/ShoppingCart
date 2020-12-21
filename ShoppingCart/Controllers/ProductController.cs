using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;
using Store.ViewModels;

namespace Store.Controllers
{
    public class ProductController : Controller
    {
        AppDbContext _db;
        IWebHostEnvironment _webHostEnvironment;
        public ProductController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> ProdList = _db.Products.Include(p=>p.Category).ToList();
            return View(ProdList);
        }

        //GET - UpSert
        public IActionResult UpSert(int? id)
        {
            //var catList = new SelectList(_db.Categories,"Id","Name");
            //ViewBag.catList = catList;
            //var Product = new Product();
            ProductVM productVM = new ProductVM {
                Product = new Product(),
                CatList=new SelectList(_db.Categories,"Id","Name")
            };

            if (id == null)
            {
                //CREATE
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Products.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }

        //POST - UpSert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpSert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + Constant.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _db.Products.Add(productVM.Product);
                }
                else
                {
                    //updating
                    var oldProd = _db.Products.AsNoTracking().FirstOrDefault(p=>p.Id==productVM.Product.Id);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + Constant.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, oldProd.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = oldProd.Image;
                    }
                    _db.Products.Update(productVM.Product);
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM = new ProductVM
            {
                Product = new Product(),
                CatList = new SelectList(_db.Categories, "Id", "Name")
            };
            return View(productVM);
        }

        //GET - DELETE
        public IActionResult Delete(int? id) 
        { 
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product product = _db.Products.Include(p=>p.Category).FirstOrDefault(p=>p.Id==id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Product product)
        {
            var oldProd = _db.Products.Find(product.Id);
            if (oldProd == null)
            {
                return NotFound();
            }
            string upload = _webHostEnvironment.WebRootPath + Constant.ImagePath;
            var oldFile = Path.Combine(upload, oldProd.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
             _db.Products.Remove(oldProd);
             _db.SaveChanges();
             return RedirectToAction("Index");
        }

    }
}
