using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model; // Import the namespace where your models are located
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;
using System;
using System.Threading.Tasks;

public class ChatGroupHub : Hub
{
    public Task JoinGroup(string groupName)
    {

        return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        Console.WriteLine("Group joined: " + groupName + "\n");
        
    }

    public Task LeaveGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        //await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        Console.WriteLine("Group left: " + groupName + "\n");
        
    }

    public Task SendMessage(string groupName, string user, string message) {

        return Clients.Group(groupName).SendAsync(user, message);
        //await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        Console.WriteLine("Send: " + message + "from: " + user + "in: " + groupName + "\n");
       
    }

}
