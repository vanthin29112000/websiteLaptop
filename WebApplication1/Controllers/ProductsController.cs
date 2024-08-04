using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {
        //GET: Products
        public ActionResult Index(string SortColumn = "")
        {
            EFFirstDatabaseEntities1 db = new EFFirstDatabaseEntities1();
            var model = new ProductFilterViewModel
            {
                Products = db.Products.ToList()
            };


            model.SortColumn = SortColumn;

            switch (SortColumn)
            {
                case "disBest":
                    {
                        model.Products = model.Products.OrderByDescending(row => row.Discount).ToList();
                        break;
                    }
                case "ascPrice":
                    {
                        model.Products = model.Products.OrderBy(row => row.CurrentPrice).ToList();
                        break;
                    }
                case "desPrice":
                    {
                        model.Products = model.Products.OrderByDescending(row => row.CurrentPrice).ToList();
                        break;
                    }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Index(ProductFilterViewModel model,int page = 1)
        {
            EFFirstDatabaseEntities1 db = new EFFirstDatabaseEntities1();

            var filteredProducts = new List<Product>();
            ViewBag.SearchQuery = model.SearchQuery;
            // Lọc sản phẩm dựa trên SearchQuery
            filteredProducts = string.IsNullOrEmpty(model.SearchQuery) ?
                db.Products.ToList() :
                db.Products.Where(p => p.ProductName.Contains(model.SearchQuery)).ToList();

            // Lọc sản phẩm dựa trên Brand
            if (!string.IsNullOrEmpty(model.SelectedBrand))
            {
                filteredProducts = filteredProducts.Where(p => p.Brand == model.SelectedBrand).ToList();
            }
            if (model.SelectedSizeScreen == null)
            {
                model.SelectedSizeScreen = new List<string>();
            }

            if (model.IsUnder10MillionChecked)
            {
                filteredProducts=(filteredProducts.Where(p => p.CurrentPrice < 10000000).ToList());
            }
            if (model.Is10To15MillionChecked)
            {
                filteredProducts=(filteredProducts.Where(p => p.CurrentPrice >= 10000000 && p.CurrentPrice <= 15000000).ToList());
            }
            if (model.Is15To20MillionChecked)
            {
                filteredProducts=(filteredProducts.Where(p => p.CurrentPrice >= 15000000 && p.CurrentPrice <= 20000000).ToList());
            }
            if (model.Is20To30MillionChecked)
            {
                filteredProducts=(filteredProducts.Where(p => p.CurrentPrice >= 20000000 && p.CurrentPrice <= 30000000).ToList());
            }
            if (model.Is30To50MillionChecked)
            {
                filteredProducts=(filteredProducts.Where(p => p.CurrentPrice >= 30000000 && p.CurrentPrice <= 50000000).ToList());
            }
            if (model.Is50To100MillionChecked)
            {
                filteredProducts=(filteredProducts.Where(p => p.CurrentPrice >= 50000000 && p.CurrentPrice <= 100000000).ToList());
            }

            // If no checkboxes are selected, display all products
            if (!model.IsUnder10MillionChecked && !model.Is10To15MillionChecked && !model.Is15To20MillionChecked &&
                !model.Is20To30MillionChecked && !model.Is30To50MillionChecked && !model.Is50To100MillionChecked)
            {
                //filteredProducts = db.Products.ToList();
            }
            else
            {
                // Remove duplicates from the filtered products
                filteredProducts = filteredProducts.Distinct().ToList();
            }

            // Apply additional filters if any
            if (model.SelectedSizeScreen.Any())
            {
                filteredProducts = filteredProducts.Where(p => model.SelectedSizeScreen.Any(size => p.Screen.Contains(size))).ToList();
            }
            if (model.SelectedScreenTechnology != null && model.SelectedScreenTechnology.Any())
            {
                filteredProducts = filteredProducts.Where(p => model.SelectedScreenTechnology.Any(tech => p.ScreenTechnology.Contains(tech))).ToList();
            }
            if (model.SelectedResolution != null && model.SelectedResolution.Any())
            {
                filteredProducts = filteredProducts.Where(p => model.SelectedResolution.Any(res => p.Resolution.Split(' ')[0] == res)).ToList();
            }
            if (model.SelectedCPU != null && model.SelectedCPU.Any())
            {
                filteredProducts = filteredProducts.Where(p => model.SelectedCPU.Any(cpu => p.Technology.Contains(cpu))).ToList();
            }
            if (model.SelectedRAM != null && model.SelectedRAM.Any())
            {
                filteredProducts = filteredProducts.Where(p => model.SelectedRAM.Any(ram => p.RAM.Contains(ram))).ToList();
            }

            // Sort products if required
            switch (model.SortColumn)
            {
                case "disBest":
                    filteredProducts = filteredProducts.OrderByDescending(row => row.Discount).ToList();
                    break;
                case "ascPrice":
                    filteredProducts = filteredProducts.OrderBy(row => row.CurrentPrice).ToList();
                    break;
                case "desPrice":
                    filteredProducts = filteredProducts.OrderByDescending(row => row.CurrentPrice).ToList();
                    break;
            }

            // Pagination
            int noRecordOfPage = 16;
            int noOfPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(filteredProducts.Count) / Convert.ToDouble(noRecordOfPage)));
            int noOfRecordToSkip = (page - 1) * noRecordOfPage;



            ViewBag.Page = page;
            ViewBag.noOfPage = noOfPage;

            model.Products = filteredProducts.Skip(noOfRecordToSkip).Take(noRecordOfPage).ToList();
            //model.Products = filteredProducts;

            return View(model);
        }

        public ActionResult Details(int id)
        {
            EFFirstDatabaseEntities1 db = new EFFirstDatabaseEntities1();
            
            Product product = db.Products.Where(row => row.Id == id).FirstOrDefault(); // Phương thức lấy sản phẩm theo id

            if (product == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu sản phẩm không tìm thấy
            }

            return View(product);
        }
    }
}