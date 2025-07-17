using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class OrderHub : Hub
{
    // Gửi thông báo xác nhận đơn hàng cho user cụ thể
    public async Task SendOrderConfirmed(string userName, int orderId)
    {
        await Clients.User(userName).SendAsync("OrderConfirmed", orderId);
    }
} 