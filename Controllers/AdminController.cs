using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using ThanhCuongShop.Models;
using System.Linq;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.SignalR;

namespace ThanhCuongShop.Controllers
{
    public class AdminController : Controller
    {
        private static readonly string ProductDataPath = "ProductData.txt";
        private readonly IHubContext<OrderHub> _orderHub;

        public AdminController(IHubContext<OrderHub> orderHub)
        {
            _orderHub = orderHub;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddProduct(string type)
        {
            ViewBag.Type = type;
            return View();
        }
        [HttpPost]
        public IActionResult AddProduct(string type, string ProductName, int Price, string Description, IFormFile ProductImage)
        {
            string imagePath = null;
            if (ProductImage != null && ProductImage.Length > 0)
            {
                var folder = type == "Quan" ? "quan" : "ao";
                var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(ProductImage.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photo", folder, fileName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    ProductImage.CopyTo(stream);
                }
                imagePath = $"/photo/{folder}/{fileName}";
            }
            // Lưu thông tin sản phẩm vào file
            var product = new Product
            {
                Name = ProductName,
                Price = Price,
                Description = Description,
                ImageUrls = imagePath,
                Category = type == "Quan" ? "Quần" : "Áo"
            };
            var json = JsonSerializer.Serialize(product);
            System.IO.File.AppendAllLines(ProductDataPath, new[] { json });
            TempData["SuccessMessage"] = "Đã thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult CustomerList()
        {
            var users = new List<User>();
            if (System.IO.File.Exists("UserData.txt"))
            {
                var lines = System.IO.File.ReadAllLines("UserData.txt");
                users = lines.Select(line => JsonSerializer.Deserialize<User>(line)).Where(u => u != null).ToList();
            }
            return View(users);
        }

        public IActionResult OrderList()
        {
            var orders = new List<Order>();
            if (System.IO.File.Exists(Order.OrderDataPath))
            {
                var lines = System.IO.File.ReadAllLines(Order.OrderDataPath);
                orders = lines.Select(line => JsonSerializer.Deserialize<Order>(line)).Where(o => o != null).ToList();
            }
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var orders = new List<Order>();
            if (System.IO.File.Exists(Order.OrderDataPath))
            {
                var lines = System.IO.File.ReadAllLines(Order.OrderDataPath);
                orders = lines.Select(line => JsonSerializer.Deserialize<Order>(line)).Where(o => o != null).ToList();
            }
            var order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = status;
                // Gửi thông báo realtime qua SignalR khi xác nhận đơn
                if (status == "Đã xác nhận")
                {
                    await _orderHub.Clients.User(order.UserName).SendAsync("OrderConfirmed", order.Id);
                }
                // Lưu lại toàn bộ đơn hàng
                System.IO.File.WriteAllLines(Order.OrderDataPath, orders.Select(o => JsonSerializer.Serialize(o)));
                TempData[$"OrderJustConfirmed_{order.UserName}"] = order.Id;
            }
            return RedirectToAction("OrderList");
        }

        // Báo cáo doanh thu theo thời gian
        public IActionResult RevenueByDate(string fromDate, string toDate)
        {
            var orders = new List<Order>();
            if (System.IO.File.Exists(Order.OrderDataPath))
            {
                var lines = System.IO.File.ReadAllLines(Order.OrderDataPath);
                orders = lines.Select(line => System.Text.Json.JsonSerializer.Deserialize<Order>(line)).Where(o => o != null).ToList();
            }
            DateTime? from = null, to = null;
            if (DateTime.TryParse(fromDate, out var f)) from = f;
            if (DateTime.TryParse(toDate, out var t)) to = t;
            var filtered = orders.Where(o => (!from.HasValue || o.CreatedAt >= from) && (!to.HasValue || o.CreatedAt <= to)).ToList();
            var totalRevenue = filtered.Sum(o => o.Total);
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View();
        }
        // Báo cáo doanh thu theo sản phẩm
        public IActionResult RevenueByProduct(string fromDate, string toDate)
        {
            var orders = new List<Order>();
            if (System.IO.File.Exists(Order.OrderDataPath))
            {
                var lines = System.IO.File.ReadAllLines(Order.OrderDataPath);
                orders = lines.Select(line => System.Text.Json.JsonSerializer.Deserialize<Order>(line)).Where(o => o != null).ToList();
            }
            DateTime? from = null, to = null;
            if (DateTime.TryParse(fromDate, out var f)) from = f;
            if (DateTime.TryParse(toDate, out var t)) to = t;
            var filtered = orders.Where(o => (!from.HasValue || o.CreatedAt >= from) && (!to.HasValue || o.CreatedAt <= to)).ToList();
            var revenueByProduct = filtered
                .SelectMany(o => o.Items)
                .GroupBy(i => i.Name)
                .Select(g => new { ProductName = g.Key, TotalRevenue = g.Sum(x => x.Price * x.Quantity), TotalSold = g.Sum(x => x.Quantity) })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();
            ViewBag.RevenueByProduct = revenueByProduct;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            return View();
        }

        private void SendOrderConfirmationEmail(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ThanhCuongShop", "your-email@gmail.com")); // Đổi thành email của bạn
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("your-email@gmail.com", "YOUR_APP_PASSWORD"); // Đổi thành app password của bạn
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
} 