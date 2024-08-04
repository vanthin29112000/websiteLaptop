using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ShoppingCartController : Controller
    {
        private EFFirstDatabaseEntities1 db = new EFFirstDatabaseEntities1();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            // Lấy CartID hiện tại từ session, nếu chưa có thì tạo mới
            var cartId = Session["CartID"] != null ? (int)Session["CartID"] : GetNextCartId();
            Session["CartID"] = cartId;

            // Lấy danh sách CartItem từ cơ sở dữ liệu dựa trên CartID
            var cartItems = db.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToList();

            // Lấy danh sách ProductId từ các CartItem
            var productIds = cartItems.Select(ci => ci.ProductId).ToList();

            // Lấy thông tin sản phẩm từ bảng Products
            var products = db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionary(p => p.Id);

            // Tạo danh sách sản phẩm để hiển thị trong view
            var cartItemViewModels = cartItems.Select(ci =>
            {
                if (products.TryGetValue(ci.ProductId ?? 0, out var product))
                {
                    return new CartItemViewModel
                    {
                        ProductId = ci.ProductId ?? 0,
                        Quantity = ci.Quantity ?? 0,
                        ProductName = product.ProductName,
                        ProductImage = product.ImageLinks.Split(',').FirstOrDefault(),
                        Price = product.CurrentPrice ?? 0,
                        TotalPrice = (ci.Quantity ?? 0) * (product.CurrentPrice ?? 0)
                    };
                }
                else
                {
                    // Xử lý khi không tìm thấy sản phẩm
                    return null; // Loại bỏ các mục null
                }
            }).Where(vm => vm != null).ToList();

            // Tính tổng giá tiền của toàn bộ sản phẩm trong giỏ hàng
            var totalPrice = cartItemViewModels.Sum(vm => vm.TotalPrice);

            // Tạo view model để truyền cho view
            var model = new CartViewModel
            {
                CartItems = cartItemViewModels,
                TotalPrice = totalPrice ?? 0
            };

            return View(model);
        }


        private int GetNextCartId()
        {
            // Kiểm tra xem có tồn tại CartItem nào chưa
            if (db.CartItems.Any())
            {
                // Lấy CartID lớn nhất hiện có
                int maxCartId = db.CartItems.Max(c => c.CartId) ?? 0;

                // Trả về giá trị lớn hơn của maxCartId
                return maxCartId + 1;
            }
            else
            {
                // Nếu chưa có CartItem nào, bắt đầu với 1
                return 1;
            }
        }

        [HttpPost]
        public ActionResult AddToCart(int productId,int quantity)
        {
            // Lấy CartID hiện tại từ session, nếu chưa có thì tạo mới
            var cartId = Session["CartID"] != null ? (int)Session["CartID"] : GetNextCartId();
            Session["CartID"] = cartId;

            if (quantity <= 0)
            {
                TempData["Error"] = "Invalid quantity.";
                return RedirectToAction("Index");
            }

            try
            {
                var cartItem = db.CartItems.FirstOrDefault(ci => ci.CartId == cartId && ci.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {

                    Product p = db.Products.Where(row => row.Id == productId).FirstOrDefault();
                    cartItem = new CartItem
                    {
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = quantity,
                        price = p.CurrentPrice
                    };

                    db.CartItems.Add(cartItem);
                }

                db.SaveChanges();

                TempData["Success"] = "Sản phẩm đã được thêm vào giỏ hàng.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi không thể thêm sản phẩm vào giỏ hàng: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int productId)
        {
            // Lấy CartID hiện tại từ session
            int cartId = Session["CartID"] != null ? (int)Session["CartID"] : 0;

            if (cartId == 0)
            {
                TempData["Error"] = "Không tìm thấy giỏ hàng.";
                return RedirectToAction("Index");
            }

            // Tìm CartItem cần xóa
            var cartItem = db.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId && ci.CartId == cartId);

            if (cartItem != null)
            {
                // Xóa CartItem khỏi giỏ hàng
                db.CartItems.Remove(cartItem);
                db.SaveChanges();
                TempData["Success"] = "Sản phẩm đã được xóa khỏi giỏ hàng.";
            }
            else
            {
                TempData["Error"] = "Sản phẩm không tìm thấy trong giỏ hàng.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            // Lấy CartID hiện tại từ session
            int cartId = Session["CartID"] != null ? (int)Session["CartID"] : 0;

            if (cartId == 0)
            {
                TempData["Error"] = "Không tìm thấy giỏ hàng.";
                return RedirectToAction("Index");
            }

            if (quantity <= 0)
            {
                TempData["Error"] = "Số lượng sản phẩm không hợp lệ.";
                return RedirectToAction("Index");
            }

            // Tìm CartItem cần cập nhật
            var cartItem = db.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId && ci.CartId == cartId);

            if (cartItem != null)
            {
                // Cập nhật số lượng
                cartItem.Quantity = quantity;
                db.Entry(cartItem).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Số lượng sản phẩm đã được cập nhật.";
            }
            else
            {
                TempData["Error"] = "Sản phẩm không tìm thấy trong giỏ hàng.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CreateInvoice(string CustomerName, string PhoneNumber, string ShippingAddress, string PaymentMethod)
        {
            // Lấy CartID từ session
            var cartId = Session["CartID"] != null ? (int)Session["CartID"] : 0;

            if (cartId == 0)
            {
                // Xử lý khi không có giỏ hàng
                TempData["Error"] = "Không tìm thấy giỏ hàng.";
                return RedirectToAction("Index");
            }

            // Tạo hóa đơn
            var invoice = new Invoice
            {
                DateCreated = DateTime.Now,
                CustomerName = CustomerName,
                TotalAmount = CalculateTotalAmount(cartId), // Tính tổng số tiền từ giỏ hàng
                PhoneNumber = PhoneNumber,
                ShippingAddress = ShippingAddress,
                PaymentMethod = PaymentMethod,// Lưu các thông tin khác từ form
            };

            db.Invoices.Add(invoice);
            db.SaveChanges();

            // Thêm các chi tiết hóa đơn từ giỏ hàng
            var cartItems = db.CartItems.Where(ci => ci.CartId == cartId).ToList();
            foreach (var item in cartItems)
            {
                var product = db.Products.Find(item.ProductId);
                var invoiceDetail = new InvoiceDetail
                {
                    InvoiceId = invoice.Id,
                    ProductId = item.ProductId ?? 0,
                    Quantity = item.Quantity ?? 0,
                    UnitPrice = product.CurrentPrice ?? 0
                };
                db.InvoiceDetails.Add(invoiceDetail);
            }

            db.SaveChanges();

            // Xóa giỏ hàng sau khi thanh toán thành công
            db.CartItems.RemoveRange(cartItems);
            db.SaveChanges();

            TempData["Success"] = "Thanh toán thành công! (Mã đơn hàng của bạn là : #" + invoice.Id + ")";
            return RedirectToAction("Index");
        }

        private decimal CalculateTotalAmount(int cartId)
        {
            var cartItems = db.CartItems.Where(ci => ci.CartId == cartId).ToList();
            return cartItems.Sum(ci => (ci.Quantity ?? 0) * (ci.price ?? 0));
        }
    }
}