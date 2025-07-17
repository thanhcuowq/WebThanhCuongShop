using Microsoft.AspNetCore.Mvc;
using ThanhCuongShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Text.Json;

namespace ThanhCuongShop.Controllers
{
    public class ProductController : Controller
    {
        // Dữ liệu mẫu, bạn có thể thay bằng đọc từ file hoặc database
        public static List<Product> products = InitProducts();

        private static List<Product> InitProducts()
        {
            var products = new List<Product>();
            var photoDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photo");
            if (Directory.Exists(photoDir))
            {
                var imageFiles = Directory.GetFiles(photoDir, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                             || f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                int id = 1;
                var rand = new Random();
                
                // Dữ liệu mẫu cho các thuộc tính
                var colors = new[] { "Đen", "Trắng", "Xanh dương", "Xanh mint", "Hồng nhạt" };
                var sizes = new[] { "S", "M", "L", "XL" };
                var genders = new[] { "Nam", "Nữ", "Unisex" };
                var forms = new[] { "Slim", "Regular", "Relax", "Croptop", "Aline", "Oversize" };
                var categories = new[] { "Áo", "Quần", "Áo khoác", "Váy", "Đồ đôi" };
                
                foreach (var file in imageFiles)
                {
                    var relPath = file.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "").Replace("\\", "/");
                    // Bỏ qua ảnh trong thư mục photo/qr
                    if (relPath.StartsWith("/photo/qr/")) continue;
                    var name = Path.GetFileNameWithoutExtension(file);
                    var price = rand.Next(100, 1000) * 500; // Giá ngẫu nhiên từ 100,000 đến 1,000,000
                    var originalPrice = price + rand.Next(50, 200) * 500;
                    var discount = (int)Math.Round(100.0 * (originalPrice - price) / originalPrice);
                    
                    // Tạo dữ liệu mẫu ngẫu nhiên
                    var allColors = "Đen,Trắng,Xanh dương,Xanh mint,Hồng nhạt";
                    var allSizes = "S,M,L,XL";
                    var randomForm = forms[rand.Next(forms.Length)];
                    
                    // Xử lý đặc biệt cho thư mục ao - tạo cả Áo Nam và Áo Nữ
                    if (relPath.Contains("/ao/"))
                    {
                        // Tạo sản phẩm Áo Nam
                        products.Add(new Product {
                            Id = id++,
                            Name = "Áo thun nam basic",
                            Description = $"Sản phẩm Áo thun nam basic - Áo Nam",
                            Price = price,
                            OriginalPrice = originalPrice,
                            DiscountPercent = discount,
                            Tags = "Auto",
                            Colors = allColors,
                            Sizes = allSizes,
                            Gender = "Nam",
                            Form = randomForm,
                            Category = "Áo Nam",
                            ImageUrls = relPath,
                            // Giữ lại các thuộc tính cũ cho tương thích
                            Size = sizes[rand.Next(sizes.Length)],
                            Color = colors[rand.Next(colors.Length)]
                        });
                        
                        // Tạo sản phẩm Áo Nữ với giá khác
                        var priceNu = rand.Next(100, 1000) * 500;
                        var originalPriceNu = priceNu + rand.Next(50, 200) * 500;
                        var discountNu = (int)Math.Round(100.0 * (originalPriceNu - priceNu) / originalPriceNu);
                        
                        products.Add(new Product {
                            Id = id++,
                            Name = "Áo thun nữ basic",
                            Description = $"Sản phẩm Áo thun nữ basic - Áo Nữ",
                            Price = priceNu,
                            OriginalPrice = originalPriceNu,
                            DiscountPercent = discountNu,
                            Tags = "Auto",
                            Colors = allColors,
                            Sizes = allSizes,
                            Gender = "Nữ",
                            Form = randomForm,
                            Category = "Áo Nữ",
                            ImageUrls = relPath,
                            // Giữ lại các thuộc tính cũ cho tương thích
                            Size = sizes[rand.Next(sizes.Length)],
                            Color = colors[rand.Next(colors.Length)]
                        });
                    }
                    else if (relPath.Contains("/quan/"))
                    {
                        // Tạo sản phẩm Quần Nam
                        products.Add(new Product {
                            Id = id++,
                            Name = "Quần short thể thao",
                            Description = $"Sản phẩm Quần short thể thao - Quần Nam",
                            Price = price,
                            OriginalPrice = originalPrice,
                            DiscountPercent = discount,
                            Tags = "Auto",
                            Colors = allColors,
                            Sizes = allSizes,
                            Gender = "Nam",
                            Form = randomForm,
                            Category = "Quần Nam",
                            ImageUrls = relPath,
                            // Giữ lại các thuộc tính cũ cho tương thích
                            Size = sizes[rand.Next(sizes.Length)],
                            Color = colors[rand.Next(colors.Length)]
                        });
                        
                        // Tạo sản phẩm Quần Nữ với giá khác
                        var priceNu = rand.Next(100, 1000) * 500;
                        var originalPriceNu = priceNu + rand.Next(50, 200) * 500;
                        var discountNu = (int)Math.Round(100.0 * (originalPriceNu - priceNu) / originalPriceNu);
                        
                        products.Add(new Product {
                            Id = id++,
                            Name = "Quần short thể thao nữ",
                            Description = $"Sản phẩm Quần short thể thao nữ - Quần Nữ",
                            Price = priceNu,
                            OriginalPrice = originalPriceNu,
                            DiscountPercent = discountNu,
                            Tags = "Auto",
                            Colors = allColors,
                            Sizes = allSizes,
                            Gender = "Nữ",
                            Form = randomForm,
                            Category = "Quần Nữ",
                            ImageUrls = relPath,
                            // Giữ lại các thuộc tính cũ cho tương thích
                            Size = sizes[rand.Next(sizes.Length)],
                            Color = colors[rand.Next(colors.Length)]
                        });
                    }
                    else if (relPath.Contains("/quanvaao/"))
                    {
                        // Tạo sản phẩm Đồ bộ Nam
                        products.Add(new Product {
                            Id = id++,
                            Name = "Bộ đồ nam",
                            Description = $"Sản phẩm Bộ đồ nam - Đồ bộ Nam",
                            Price = price,
                            OriginalPrice = originalPrice,
                            DiscountPercent = discount,
                            Tags = "Auto",
                            Colors = allColors,
                            Sizes = allSizes,
                            Gender = "Nam",
                            Form = randomForm,
                            Category = "Đồ bộ Nam",
                            ImageUrls = relPath,
                            // Giữ lại các thuộc tính cũ cho tương thích
                            Size = sizes[rand.Next(sizes.Length)],
                            Color = colors[rand.Next(colors.Length)]
                        });
                        
                        // Tạo sản phẩm Đồ bộ Nữ với giá khác
                        var priceNu = rand.Next(100, 1000) * 500;
                        var originalPriceNu = priceNu + rand.Next(50, 200) * 500;
                        var discountNu = (int)Math.Round(100.0 * (originalPriceNu - priceNu) / originalPriceNu);
                        
                        products.Add(new Product {
                            Id = id++,
                            Name = "Bộ đồ nữ",
                            Description = $"Sản phẩm Bộ đồ nữ - Đồ bộ Nữ",
                            Price = priceNu,
                            OriginalPrice = originalPriceNu,
                            DiscountPercent = discountNu,
                            Tags = "Auto",
                            Colors = allColors,
                            Sizes = allSizes,
                            Gender = "Nữ",
                            Form = randomForm,
                            Category = "Đồ bộ Nữ",
                            ImageUrls = relPath,
                            // Giữ lại các thuộc tính cũ cho tương thích
                            Size = sizes[rand.Next(sizes.Length)],
                            Color = colors[rand.Next(colors.Length)]
                        });
                    }
                    else if (relPath.Contains("/dep/"))
                    {
                        // Tạo sản phẩm Dép
                        products.Add(new Product {
                            Id = id++,
                            Name = "Dép thời trang",
                            Description = $"Sản phẩm Dép thời trang - Phụ kiện",
                            Price = price,
                            OriginalPrice = originalPrice,
                            DiscountPercent = discount,
                            Tags = "Auto",
                            Colors = allColors,
                            Sizes = allSizes,
                            Gender = "Unisex",
                            Form = randomForm,
                            Category = "Dép",
                            ImageUrls = relPath,
                            Size = sizes[rand.Next(sizes.Length)],
                            Color = colors[rand.Next(colors.Length)]
                        });
                    }
                    else
                    {
                        // Xử lý các thư mục khác như cũ
                        string category = "Khác";
                        string gender = "Unisex";
                        
                        if (relPath.Contains("/aokhoacnam/")) 
                        {
                            category = "Áo Khoác Nam";
                            gender = "Nam";
                            name = "Áo Khoác Nam";
                        }
                        else if (relPath.Contains("/aokhoacnu/")) 
                        {
                            category = "Áo Khoác Nữ";
                            gender = "Nữ";
                            name = "Áo Khoác Nữ";
                        }
                        else if (relPath.Contains("/quanshortnam/")) 
                        {
                            category = "Quần Short Nam";
                            gender = "Nam";
                            name = "Quần Short Nam";
                        }
                        else if (relPath.Contains("/quanshortnu/")) 
                        {
                            category = "Quần Short Nữ";
                            gender = "Nữ";
                            name = "Quần Short Nữ";
                        }
                        else if (relPath.Contains("/dodoi/")) 
                        {
                            category = "Đồ Đôi";
                            gender = "Unisex";
                            name = "Đồ Đôi";
                        }
                        else if (relPath.Contains("/khoac/")) 
                        {
                            category = "Áo khoác";
                            gender = "Unisex";
                            name = "Áo khoác";
                        }
                        else if (relPath.Contains("/vay/")) 
                        {
                            category = "Váy";
                            gender = "Nữ";
                            name = "Váy";
                        }
                        
                        // Sau khi xác định gender cho từng sản phẩm, chuẩn hóa lại giá trị
                        if (gender != "Nam" && gender != "Nữ" && gender != "Unisex")
                        {
                            gender = "Unisex";
                        }
                        
                        products.Add(new Product {
                            Id = id++,
                            Name = name,
                            Description = $"Sản phẩm {name} - {category}",
                            Price = price,
                            OriginalPrice = originalPrice,
                            DiscountPercent = discount,
                            Tags = "Auto",
                            Colors = allColors,
                            Sizes = allSizes,
                            Gender = gender,
                            Form = randomForm,
                            Category = category,
                            ImageUrls = relPath,
                            // Giữ lại các thuộc tính cũ cho tương thích
                            Size = sizes[rand.Next(sizes.Length)],
                            Color = colors[rand.Next(colors.Length)]
                        });
                    }
                }
            }
            return products;
        }

        [HttpGet]
        public IActionResult Index(string? category)
        {
            var filteredProducts = products.AsQueryable();
            if (!string.IsNullOrEmpty(category))
            {
                if (category == "Áo Khoác Nam")
                {
                    filteredProducts = filteredProducts.Where(p => p.ImageUrls.Contains("/aokhoacnam/"));
                }
                else if (category == "Áo Khoác Nữ")
                {
                    filteredProducts = filteredProducts.Where(p => p.ImageUrls.Contains("/aokhoacnu/"));
                }
                else if (category == "Quần Short Nam")
                {
                    filteredProducts = filteredProducts.Where(p => p.ImageUrls.Contains("/quanshortnam/"));
                }
                else if (category == "Quần Short Nữ")
                {
                    filteredProducts = filteredProducts.Where(p => p.ImageUrls.Contains("/quanshortnu/"));
                }
                else if (category == "Đồ Đôi")
                {
                    filteredProducts = filteredProducts.Where(p => p.ImageUrls.Contains("/dodoi/"));
                }
                else
                {
                    filteredProducts = filteredProducts.Where(p => p.Category == category);
                }
            }
            // Nếu không có filter, trả về toàn bộ sản phẩm
            var filterModel = new ProductFilterViewModel
            {
                Products = filteredProducts.ToList(),
                TotalCount = products.Count,
                FilteredCount = filteredProducts.Count()
            };
            PopulateFilterOptions(filterModel);
            return View(filterModel);
        }

        [HttpGet]
        public IActionResult Filter(string? color, string? size, string? gender, string? form, 
                                  string? category, decimal? minPrice, decimal? maxPrice, string? searchTerm)
        {
            var filteredProducts = products.AsQueryable();
            bool isAll = (string.IsNullOrEmpty(color) || color == "Tất cả màu")
                && (string.IsNullOrEmpty(size) || size == "Tất cả size")
                && (string.IsNullOrEmpty(gender) || gender == "Unisex" || gender == "Tất cả")
                && (string.IsNullOrEmpty(form) || form == "Tất cả")
                && (string.IsNullOrEmpty(category) || category == "Tất cả danh mục")
                && !minPrice.HasValue && !maxPrice.HasValue && string.IsNullOrEmpty(searchTerm);
            if (isAll)
            {
                var allProductsModel = new ProductFilterViewModel
                {
                    Gender = gender,
                    Color = color,
                    Size = size,
                    Form = form,
                    Category = category,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SearchTerm = searchTerm,
                    Products = products.ToList(),
                    TotalCount = products.Count,
                    FilteredCount = products.Count
                };
                PopulateFilterOptions(allProductsModel);
                return View("Index", allProductsModel);
            }
            
            // Apply filters
            if (!string.IsNullOrEmpty(color))
            {
                filteredProducts = filteredProducts.Where(p => p.Colors.Contains(color));
            }
            
            if (!string.IsNullOrEmpty(size))
            {
                filteredProducts = filteredProducts.Where(p => p.Sizes.Contains(size));
            }
            
            if (!string.IsNullOrEmpty(gender))
            {
                filteredProducts = filteredProducts.Where(p => p.Gender == gender);
            }
            
            if (!string.IsNullOrEmpty(form))
            {
                filteredProducts = filteredProducts.Where(p => p.Form == form);
            }
            
            if (!string.IsNullOrEmpty(category))
            {
                filteredProducts = filteredProducts.Where(p => p.Category == category);
            }
            
            if (minPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= minPrice.Value);
            }
            
            if (maxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= maxPrice.Value);
            }
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            var filterModel = new ProductFilterViewModel
            {
                Gender = gender,
                Color = color,
                Size = size,
                Form = form,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SearchTerm = searchTerm,
                Products = filteredProducts.ToList(),
                TotalCount = products.Count,
                FilteredCount = filteredProducts.Count()
            };
            
            // Populate available options
            PopulateFilterOptions(filterModel);
            
            return View("Index", filterModel);
        }

        private void PopulateFilterOptions(ProductFilterViewModel filterModel)
        {
            // Get unique values from products
            filterModel.AvailableColors = products
                .SelectMany(p => p.Colors?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0])
                .Select(c => c.Trim())
                .Distinct()
                .OrderBy(c => c)
                .ToList();
                
            filterModel.AvailableSizes = products
                .SelectMany(p => p.Sizes?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0])
                .Select(s => s.Trim())
                .Distinct()
                .OrderBy(s => s)
                .ToList();
                
            filterModel.AvailableGenders = products
                .Select(p => p.Gender)
                .Where(g => !string.IsNullOrEmpty(g))
                .Distinct()
                .OrderBy(g => g)
                .ToList();
                
            filterModel.AvailableForms = products
                .Select(p => p.Form)
                .Where(f => !string.IsNullOrEmpty(f))
                .Distinct()
                .OrderBy(f => f)
                .ToList();
                
            filterModel.AvailableCategories = products
                .Select(p => p.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        public IActionResult Details(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                // Giả sử admin upload ảnh và nhập đúng đường dẫn ảnh vào ImageUrl
                model.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
                products.Add(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Gallery()
        {
            var photoDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photo");
            var imageFiles = Directory.GetFiles(photoDir, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Replace(Directory.GetCurrentDirectory() + "\\wwwroot", "").Replace("\\", "/"))
                .ToList();
            return View(imageFiles);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, string color, string size, int quantity)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return NotFound();
            
            var cart = new List<CartItem>();
            var userId = HttpContext.Session.GetString("UserId");
            
            // Nếu user đã đăng nhập, sử dụng giỏ hàng của user
            if (!string.IsNullOrEmpty(userId))
            {
                var userCartJson = HttpContext.Session.GetString($"UserCart_{userId}");
                if (!string.IsNullOrEmpty(userCartJson))
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(userCartJson);
            }
            else
            {
                // Nếu chưa đăng nhập, sử dụng giỏ hàng session thông thường
                var cartJson = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(cartJson))
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            }
            
            var existing = cart.FirstOrDefault(c => c.ProductId == productId && c.Color == color && c.Size == size);
            if (existing != null)
                existing.Quantity += quantity;
            else
                cart.Add(new CartItem {
                    ProductId = product.Id,
                    Name = product.Name,
                    ImageUrl = product.ImageUrls?.Split(',')[0] ?? product.ImageUrl,
                    Color = color,
                    Size = size,
                    Price = product.Price,
                    Quantity = quantity
                });
            
            // Lưu giỏ hàng vào session tương ứng
            if (!string.IsNullOrEmpty(userId))
            {
                HttpContext.Session.SetString($"UserCart_{userId}", System.Text.Json.JsonSerializer.Serialize(cart));
            }
            HttpContext.Session.SetString("Cart", System.Text.Json.JsonSerializer.Serialize(cart));
            HttpContext.Session.SetInt32("CartCount", cart.Sum(c => c.Quantity));
            
            return Json(new { success = true, cartCount = cart.Sum(c => c.Quantity) });
        }

        [HttpGet]
        public IActionResult Cart()
        {
            var cart = new List<CartItem>();
            var userId = HttpContext.Session.GetString("UserId");
            
            // Nếu user đã đăng nhập, lấy giỏ hàng của user
            if (!string.IsNullOrEmpty(userId))
            {
                var userCartJson = HttpContext.Session.GetString($"UserCart_{userId}");
                if (!string.IsNullOrEmpty(userCartJson))
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(userCartJson);
            }
            else
            {
                // Nếu chưa đăng nhập, lấy giỏ hàng session thông thường
                var cartJson = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(cartJson))
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            }
            
            return View(cart);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId, string color, string size)
        {
            var cart = new List<CartItem>();
            var userId = HttpContext.Session.GetString("UserId");
            
            // Lấy giỏ hàng tương ứng
            if (!string.IsNullOrEmpty(userId))
            {
                var userCartJson = HttpContext.Session.GetString($"UserCart_{userId}");
                if (!string.IsNullOrEmpty(userCartJson))
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(userCartJson);
            }
            else
            {
                var cartJson = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(cartJson))
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            }
            
            // Xóa item khỏi giỏ hàng
            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId && c.Color == color && c.Size == size);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                
                // Lưu giỏ hàng đã cập nhật
                if (!string.IsNullOrEmpty(userId))
                {
                    HttpContext.Session.SetString($"UserCart_{userId}", System.Text.Json.JsonSerializer.Serialize(cart));
                }
                HttpContext.Session.SetString("Cart", System.Text.Json.JsonSerializer.Serialize(cart));
                HttpContext.Session.SetInt32("CartCount", cart.Sum(c => c.Quantity));
            }
            
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            
            // Xóa giỏ hàng
            if (!string.IsNullOrEmpty(userId))
            {
                HttpContext.Session.Remove($"UserCart_{userId}");
            }
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("CartCount");
            
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult Checkout(int? productId, string color, string size, int? quantity, int? price)
        {
            decimal total = 0;
            string userPhoneOrName = "";
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var users = System.IO.File.Exists("UserData.txt") ? System.IO.File.ReadAllLines("UserData.txt") : null;
                if (users != null)
                {
                    var user = users.Select(line => System.Text.Json.JsonSerializer.Deserialize<ThanhCuongShop.Models.User>(line)).FirstOrDefault(u => u != null && u.Id.ToString() == userId);
                    if (user != null)
                    {
                        if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                            userPhoneOrName = user.PhoneNumber;
                        else if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@"))
                            userPhoneOrName = user.Email.Split('@')[0];
                    }
                }
            }
            ViewBag.UserPhoneOrName = userPhoneOrName;
            if (productId.HasValue && price.HasValue && quantity.HasValue)
            {
                // Mua ngay 1 sản phẩm
                total = price.Value;
                ViewBag.SingleProduct = true;
                ViewBag.ProductId = productId;
                ViewBag.Color = color;
                ViewBag.Size = size;
                ViewBag.Quantity = quantity;
                // Lưu thông tin mua ngay vào session
                var buyNowList = new List<CartItem> {
                    new CartItem {
                        ProductId = productId.Value,
                        Name = products.FirstOrDefault(p => p.Id == productId.Value)?.Name ?? "",
                        ImageUrl = products.FirstOrDefault(p => p.Id == productId.Value)?.ImageUrls?.Split(',')[0] ?? products.FirstOrDefault(p => p.Id == productId.Value)?.ImageUrl,
                        Color = color,
                        Size = size,
                        Price = price.Value / quantity.Value, // đơn giá
                        Quantity = quantity.Value
                    }
                };
                HttpContext.Session.SetString("BuyNow", System.Text.Json.JsonSerializer.Serialize(buyNowList));
            }
            else
            {
                // Lấy giỏ hàng như cũ
                var cart = new List<CartItem>();
                if (!string.IsNullOrEmpty(userId))
                {
                    var userCartJson = HttpContext.Session.GetString($"UserCart_{userId}");
                    if (!string.IsNullOrEmpty(userCartJson))
                        cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(userCartJson);
                }
                else
                {
                    var cartJson = HttpContext.Session.GetString("Cart");
                    if (!string.IsNullOrEmpty(cartJson))
                        cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson);
                }
                total = cart.Sum(x => x.Price * x.Quantity);
                ViewBag.Count = cart.Sum(x => x.Quantity);
                ViewBag.Cart = cart;
                // Xóa session BuyNow nếu vào checkout từ giỏ hàng
                HttpContext.Session.Remove("BuyNow");
            }
            ViewBag.Total = total;
            return View();
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
            // Nếu không có giỏ hàng, thử lấy từ BuyNow
            if (cart.Count == 0)
            {
                var buyNowJson = HttpContext.Session.GetString("BuyNow");
                debugLog += $"buyNowJson={buyNowJson}\n";
                if (!string.IsNullOrEmpty(buyNowJson))
                {
                    cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(buyNowJson);
                }
            }
            debugLog += $"cart.Count={cart.Count}\n";
            System.IO.File.AppendAllText("debug_payment.txt", debugLog);

            if (cart.Count > 0)
            {
                var orders = new List<Order>();
                if (System.IO.File.Exists(Order.OrderDataPath))
                {
                    var lines = System.IO.File.ReadAllLines(Order.OrderDataPath);
                    orders = lines.Select(line => System.Text.Json.JsonSerializer.Deserialize<Order>(line)).Where(o => o != null).ToList();
                }
                var newOrder = new Order
                {
                    Id = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1,
                    UserName = userName,
                    UserEmail = userEmail,
                    UserPhone = userPhone,
                    Items = cart,
                    Total = cart.Sum(x => x.Price * x.Quantity),
                    CreatedAt = DateTime.Now,
                    Status = "Chờ xác nhận"
                };
                orders.Add(newOrder);
                System.IO.File.WriteAllLines(Order.OrderDataPath, orders.Select(o => System.Text.Json.JsonSerializer.Serialize(o)));
                if (!string.IsNullOrEmpty(userId))
                {
                    HttpContext.Session.Remove($"UserCart_{userId}");
                }
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("CartCount");
                HttpContext.Session.Remove("BuyNow"); // Xóa luôn session mua ngay
                return View();
            }
            // Nếu không có sản phẩm trong giỏ, vẫn trả về view (hoặc có thể redirect về trang giỏ hàng tuỳ ý)
            return View();
        }
    }
} 