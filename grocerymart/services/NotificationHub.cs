using Microsoft.AspNetCore.SignalR;

namespace grocerymart.services;

public class NotificationHub : Hub
{
    public async Task UpdateLikedProducts(int count)
    {
        await Clients.All.SendAsync("ReceiveLikedProducts", count);
    }
}