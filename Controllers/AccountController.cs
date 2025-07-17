using Microsoft.AspNetCore.Mvc;
using ThanhCuongShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ThanhCuongShop.Controllers
{
    public class AccountController : Controller
    {
        private static readonly string UserDataPath = "UserData.txt";

        public AccountController()
        {
            // Tạo tài khoản admin mặc định nếu chưa có
            var users = LoadUsers();
            if (!users.Any(u => (u.Email == "admin" || u.PhoneNumber == "admin")))
            {
                var adminUser = new User
                {
                    Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1,
                    Email = "admin",
                    Password = "123"
                };
                SaveUser(adminUser);
            }
        }

        private List<User> LoadUsers()
        {
            if (!System.IO.File.Exists(UserDataPath))
                return new List<User>();
            var lines = System.IO.File.ReadAllLines(UserDataPath);
            return lines.Select(line => JsonSerializer.Deserialize<User>(line)).Where(u => u != null).ToList();
        }

        private void SaveUser(User user)
        {
            var json = JsonSerializer.Serialize(user);
            System.IO.File.AppendAllLines(UserDataPath, new[] { json });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var users = LoadUsers();
                if (users.Any(u => u.Email == model.EmailOrPhone || u.PhoneNumber == model.EmailOrPhone))
                {
                    ModelState.AddModelError("", "Email hoặc số điện thoại đã tồn tại");
                    return View(model);
                }
                var user = new User
                {
                    Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1,
                    Password = model.Password
                };
                if (model.EmailOrPhone.Contains("@"))
                    user.Email = model.EmailOrPhone;
                else
                    user.PhoneNumber = model.EmailOrPhone;
                SaveUser(user);
                TempData["SuccessMessage"] = "Tạo tài khoản thành công! Bạn có thể đăng nhập ngay.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var users = LoadUsers();
                var user = users.FirstOrDefault(u =>
                    (u.Email == model.EmailOrPhone || u.PhoneNumber == model.EmailOrPhone)
                    && u.Password == model.Password);
                if (user != null)
                {
                    // Lưu thông tin user chi tiết vào session
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserName", user.Email ?? user.PhoneNumber);
                    HttpContext.Session.SetString("UserEmail", user.Email ?? "");
                    HttpContext.Session.SetString("UserPhone", user.PhoneNumber ?? "");
                    HttpContext.Session.SetString("IsAdmin", (user.Email != null && user.Email.Contains("admin")) ? "true" : "false");
                    
                    // Lưu thời gian đăng nhập để tracking
                    HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    // Merge giỏ hàng từ session cũ (nếu có) với giỏ hàng của user
                    MergeUserCart(user.Id.ToString());
                    
                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    if (HttpContext.Session.GetString("IsAdmin") == "true")
                        return RedirectToAction("Index", "Admin");
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng");
            }
            return View(model);
        }

        private void MergeUserCart(string userId)
        {
            // Lấy giỏ hàng hiện tại từ session
            var currentCartJson = HttpContext.Session.GetString("Cart");
            var currentCart = new List<CartItem>();
            if (!string.IsNullOrEmpty(currentCartJson))
            {
                currentCart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(currentCartJson);
            }
            
            // Lấy giỏ hàng đã lưu của user (nếu có)
            var userCartJson = HttpContext.Session.GetString($"UserCart_{userId}");
            var userCart = new List<CartItem>();
            if (!string.IsNullOrEmpty(userCartJson))
            {
                userCart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(userCartJson);
            }
            
            // Merge giỏ hàng: thêm items từ session hiện tại vào user cart
            foreach (var item in currentCart)
            {
                var existingItem = userCart.FirstOrDefault(c => c.ProductId == item.ProductId && c.Color == item.Color && c.Size == item.Size);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    userCart.Add(item);
                }
            }
            
            // Lưu giỏ hàng đã merge vào session của user
            HttpContext.Session.SetString($"UserCart_{userId}", System.Text.Json.JsonSerializer.Serialize(userCart));
            HttpContext.Session.SetString("Cart", System.Text.Json.JsonSerializer.Serialize(userCart));
            HttpContext.Session.SetInt32("CartCount", userCart.Sum(c => c.Quantity));
        }

        public IActionResult Logout()
        {
            // Lưu giỏ hàng hiện tại vào user cart trước khi logout
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var cartJson = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(cartJson))
                {
                    HttpContext.Session.SetString($"UserCart_{userId}", cartJson);
                }
            }
            
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null) return RedirectToAction("Login");
            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(string Email, string PhoneNumber)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null) return RedirectToAction("Login");
            user.Email = Email;
            user.PhoneNumber = PhoneNumber;
            // Lưu lại toàn bộ danh sách user
            System.IO.File.WriteAllLines(UserDataPath, users.Select(u => JsonSerializer.Serialize(u)));
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null) return RedirectToAction("Login");
            if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }
            if (user.Password != OldPassword)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng.";
                return View();
            }
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu mới và xác nhận không khớp.";
                return View();
            }
            user.Password = NewPassword;
            System.IO.File.WriteAllLines(UserDataPath, users.Select(u => JsonSerializer.Serialize(u)));
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }

        public IActionResult OrderHistory()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");
            var userName = HttpContext.Session.GetString("UserName");
            var orders = new List<Order>();
            if (System.IO.File.Exists(Order.OrderDataPath))
            {
                var lines = System.IO.File.ReadAllLines(Order.OrderDataPath);
                orders = lines.Select(line => System.Text.Json.JsonSerializer.Deserialize<Order>(line)).Where(o => o != null && o.UserName == userName).ToList();
            }
            if (TempData[$"OrderJustConfirmed_{userName}"] != null)
            {
                ViewBag.OrderJustConfirmed = TempData[$"OrderJustConfirmed_{userName}"];
            }
            return View(orders);
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userPhone = HttpContext.Session.GetString("UserPhone");
            var cart = new List<CartItem>();
            string debugLog = $"userId={userId}, userName={userName}, userEmail={userEmail}, userPhone={userPhone}\n";
            if (!string.IsNullOrEmpty(userId))
            {
                var userCartJson = HttpContext.Session.GetString($"UserCart_{userId}");
                debugLog += $"userCartJson={userCartJson}\n";
                if (!string.IsNullOrEmpty(userCartJson))
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(userCartJson);
            }
            else
            {
                var cartJson = HttpContext.Session.GetString("Cart");
                debugLog += $"cartJson={cartJson}\n";
                if (!string.IsNullOrEmpty(cartJson))
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            }
            debugLog += $"cart.Count={cart.Count}\n";
            System.IO.File.AppendAllText("debug_payment.txt", debugLog);

            // ... phần còn lại giữ nguyên ...
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string emailOrPhone)
        {
            var users = System.IO.File.ReadAllLines(UserDataPath)
                .Select(line => {
                    try { return System.Text.Json.JsonSerializer.Deserialize<User>(line); } catch { return null; }
                })
                .Where(u => u != null)
                .ToList();
            var input = emailOrPhone?.Trim();
            var user = users.FirstOrDefault(u =>
                (!string.IsNullOrEmpty(u.Email) && u.Email.Trim().Equals(input, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.Trim().Equals(input, StringComparison.OrdinalIgnoreCase))
            );
            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản với thông tin đã nhập!";
                return View();
            }
            var newPassword = GenerateRandomPassword();
            user.Password = newPassword;
            System.IO.File.WriteAllLines(UserDataPath, users.Select(u => System.Text.Json.JsonSerializer.Serialize(u)));
            ViewBag.Message = $"Mật khẩu mới đã được đặt lại: {newPassword}.";
            return View();
        }

        private string GenerateRandomPassword(int length = 8)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
} 