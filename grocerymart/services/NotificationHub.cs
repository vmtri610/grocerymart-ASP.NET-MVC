using Microsoft.AspNetCore.SignalR;

namespace grocerymart.services;

public class NotificationHub : Hub
{
    public async Task UpdateLikedProducts(int count)
    {
        await Clients.All.SendAsync("ReceiveLikedProducts", count);
    }

    public async Task UpdateCartProducts(int count)
    {
        await Clients.All.SendAsync("ReceiveCartProducts", count);
    }

    public async Task NotifyLikedProductsChanged()
    {
        await Clients.All.SendAsync("LikedProductsChanged");
    }

    public async Task NotifyCartProductsChanged()
    {
        await Clients.All.SendAsync("CartProductsChanged");
    }

    // add new address
    public async Task NotifyAddressAdded()
    {
        await Clients.All.SendAsync("AddressAdded");
    }
}