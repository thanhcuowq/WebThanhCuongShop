using Microsoft.AspNetCore.SignalR;

public class SessionUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.GetHttpContext()?.Session.GetString("UserName");
    }
} 