using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model; // Import the namespace where your models are located
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

public class ChatGroupHub : Hub
{
    public async Task JoinGroup(string groupName)
    {

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        Console.WriteLine("Group joined: " + groupName + "\n");
        
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        Console.WriteLine("Group left: " + groupName + "\n");
        
    }

    public async Task SendMessage(string groupName, string user, string message) {

        await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        Console.WriteLine("Send: " + message + "from: " + user + "in: " + groupName + "\n");
       
    }

}
