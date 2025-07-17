using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ThanhCuongShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        
        // Đường dẫn file OrderData.txt duy nhất cho toàn bộ ứng dụng
        public static readonly string OrderDataPath = Path.Combine(Directory.GetCurrentDirectory(), "OrderData.txt");
    }

    public static class OrderExtensions
    {
        public static void SaveOrders(this List<Order> orders)
        {
            File.WriteAllLines(Order.OrderDataPath, orders.Select(o => JsonSerializer.Serialize(o)));
        }
    }
} 