using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model; // Import the namespace where your models are located
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace PeopleHelpPeople.ChatGroupHub;
{
    public interface IEventHub
    {
    Task SendNoticeEventToClient(string message);

    Task SendMessageToGroup(string group, string message);
    }

    public class EventHub : Hub<IEventHub>
    {
        public Task JoinOrLeaveGroup(string group, bool check)
        {
            if (check)
            {
                return Groups.AddToGroupAsync(Context.ConnectionId, group);
            }
            else
            {
                return Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
            }
        }
    public Task SendToGroup(string group, string message)
    {
        Clients.OthersInGroup(group).addChatMessage(message);
    }
    }
}