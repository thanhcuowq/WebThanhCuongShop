namespace ThanhCuongShop.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } // Có thể là Gmail hoặc null
        public string PhoneNumber { get; set; } // Có thể là SĐT hoặc null
        public string Password { get; set; }
    }
} 