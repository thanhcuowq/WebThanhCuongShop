namespace ThanhCuongShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public string Size { get; set; } // Size đơn lẻ (giữ lại cho tương thích)
        public string Color { get; set; } // Màu đơn lẻ (giữ lại cho tương thích)
        public string ImageUrls { get; set; } // Danh sách đường dẫn ảnh, cách nhau bởi dấu phẩy
        public decimal OriginalPrice { get; set; } // Giá gốc
        public int DiscountPercent { get; set; } // Phần trăm giảm giá
        public string Tags { get; set; } // Tag sản phẩm, cách nhau bởi dấu phẩy
        public string Colors { get; set; } // Danh sách màu, cách nhau bởi dấu phẩy (mã hex)
        
        // Các thuộc tính mới cho filter
        public string Sizes { get; set; } // Danh sách size có sẵn, cách nhau bởi dấu phẩy (S,M,L,XL,XXL,S27,S28,...)
        public string Gender { get; set; } // Giới tính: "Nam", "Nữ", "Unisex"
        public string Form { get; set; } // Form dáng: "Slim", "Regular", "Relax", "Croptop", "Aline", "Oversize"
        
        // Thuộc tính tính toán
        public bool IsOnSale => OriginalPrice > Price;
        public decimal DiscountAmount => OriginalPrice - Price;
    }
} 