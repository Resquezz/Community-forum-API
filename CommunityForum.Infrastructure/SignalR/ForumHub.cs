using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Infrastructure.SignalR
{
    public class ForumHub : Hub
    {
        public async Task SendMessageToAll(EventType eventType, object payload)
        {
            await Clients.All.SendAsync(eventType.ToString(), payload);
        }
        public async Task SendMessageToUser(EventType eventType, string userId, object payload)
        {
            await Clients.User(userId).SendAsync(eventType.ToString(), payload);
        }
    }
}
