using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using OfficeOpenXml;
using WebApplication1.Models;
using System.Data.Entity.Validation;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        EFFirstDatabaseEntities1 db = new EFFirstDatabaseEntities1();
        // GET: Home
        public ActionResult Index()
        {
            
            List<Product> products = db.Products
                .OrderBy(p => p.Id) // Sắp xếp theo ProductId
                .Skip(0) // Bỏ qua 0 bản ghi
                .Take(6) // Lấy 10 bản ghi
                .ToList();
            return View(products);
        }


        public ActionResult ReadExcel()
        {
            // Thiết lập LicenseContext cho EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string filePath = Server.MapPath("~/Content/Uploads/all_specifications.xlsx");

            var products = new List<Product>();
            using (var db = new EFFirstDatabaseEntities1())
            {
                if (System.IO.File.Exists(filePath))
                {
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        var worksheet = package.Workbook.Worksheets.First();
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Bỏ qua dòng đầu tiên (header)
                        {
                            int id;
                            int.TryParse(worksheet.Cells[row, 1].Text, out id);

                            var product = new Product
                            {
                                Id = id,
                                Technology = worksheet.Cells[row, 2].Text,
                                NumberOfCores = worksheet.Cells[row, 3].Text,
                                NumberOfThreads = worksheet.Cells[row, 4].Text,
                                CPUSpeed = worksheet.Cells[row, 5].Text,
                                MaxSpeed = worksheet.Cells[row, 6].Text,
                                CacheSize = worksheet.Cells[row, 7].Text,
                                RAM = worksheet.Cells[row, 8].Text,
                                RAMType = worksheet.Cells[row, 9].Text,
                                RAMBusSpeed = worksheet.Cells[row, 10].Text,
                                MaxRAMSupport = worksheet.Cells[row, 11].Text,
                                Storage = worksheet.Cells[row, 12].Text,
                                Screen = worksheet.Cells[row, 13].Text,
                                Resolution = worksheet.Cells[row, 14].Text,
                                ScreenTechnology = worksheet.Cells[row, 15].Text,
                                GraphicsCard = worksheet.Cells[row, 16].Text,
                                AudioTechnology = worksheet.Cells[row, 17].Text,
                                PortsInfo = worksheet.Cells[row, 18].Text,
                                WirelessConnectivity = worksheet.Cells[row, 19].Text,
                                Webcam = worksheet.Cells[row, 20].Text,
                                DimensionsAndWeight = worksheet.Cells[row, 21].Text,
                                BatteryInfo = worksheet.Cells[row, 22].Text,
                                KeyboardBacklight = worksheet.Cells[row, 23].Text,
                                ProductName = worksheet.Cells[row, 24].Text,
                                Brand = worksheet.Cells[row, 25].Text,
                                CurrentPrice = int.TryParse(worksheet.Cells[row, 26].Text, out int currentPrice) ? (int?)currentPrice : null,
                                OldPrice = int.TryParse(worksheet.Cells[row, 27].Text, out int oldPrice) ? (int?)oldPrice : null,
                                Discount = int.TryParse(worksheet.Cells[row, 28].Text, out int discount) ? (int?)discount : null,
                                Quantity = int.TryParse(worksheet.Cells[row, 29].Text, out int quantity) ? (int?)quantity : null,
                                Description = worksheet.Cells[row, 30].Text,
                                ImageLinks = worksheet.Cells[row, 31].Text,
                                ScanFrequency = worksheet.Cells[row, 32].Text,
                                MemoryCard = worksheet.Cells[row, 33].Text,
                                OtherFunction = worksheet.Cells[row, 34].Text,
                                Material = worksheet.Cells[row, 35].Text,
                                OperatingSystem = worksheet.Cells[row, 36].Text,
                                LaunchTime = worksheet.Cells[row, 37].Text
                            };

                            products.Add(product);
                        }

                        db.Products.AddRange(products);

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var validationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                }
                            }
                            throw;
                        }
                    }
                }
            }

            return View(products);
        }

        public class ProductUpdate
        {
            public int ProductId { get; set; }
            public string Description { get; set; }
        }

        public List<ProductUpdate> ReadExcelFile(string filePath)
        {
            var productUpdates = new List<ProductUpdate>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên

                for (int row = 2; row <= 457; row++) // Bắt đầu từ dòng 2 đến dòng 457
                {
                    var productId = worksheet.Cells[row, 1].GetValue<int>(); // Giả sử ID nằm ở cột A
                    var description = worksheet.Cells[row, 30].GetValue<string>(); // Cột AD là cột số 30

                    productUpdates.Add(new ProductUpdate
                    {
                        ProductId = productId,
                        Description = description
                    });
                }
            }

            return productUpdates;
        }

        public void UpdateProductDescriptions(List<ProductUpdate> productUpdates)
        {
            using (var context = new EFFirstDatabaseEntities1())
            {
                foreach (var update in productUpdates)
                {
                    var product = context.Products.FirstOrDefault(p => p.Id == update.ProductId);
                    if (product != null)
                    {
                        product.Description = update.Description;
                        // Kiểm tra kiểu dữ liệu
                        System.Diagnostics.Debug.WriteLine("Product Description Type: " + product.Description.GetType());
                    }
                }

                context.SaveChanges();
            }
        }

        public void UpdateProductsFromExcel(string excelFilePath)
        {
            var productUpdates = ReadExcelFile(excelFilePath);
            UpdateProductDescriptions(productUpdates);
        }

        public ActionResult UpdateProducts()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string excelFilePath = Server.MapPath("~/Content/uploads/all_specifications.xlsx");
            UpdateProductsFromExcel(excelFilePath);
            return View();
        }
    }

    


}