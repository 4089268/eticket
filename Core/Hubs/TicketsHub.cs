using System;
using Microsoft.AspNetCore.SignalR;

namespace eticket.Core.Hubs;

public class TicketsHub : Hub
{
    public async Task SendTicketAssigned(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task SendNewTicket(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}
