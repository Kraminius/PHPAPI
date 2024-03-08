using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model; // Import the namespace where your models are located
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace PeopleHelpPeople.ChatHub;

public class Chathub : Hub
{
    public async Task SendMessage(string id, string user, string message)
    {
        Clients.User(id).SendAsync("RecieveMessage", user, DateTime.Now, message);
    }
}