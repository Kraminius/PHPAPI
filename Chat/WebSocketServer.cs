namespace PeopleHelpPeople.Model
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class WebSocketServer
    {

        private HttpListener httpListener;
        private ConcurrentDictionary<Guid, WebSocket> clients;


        //Setup the websocket
        public WebSocketServer(string prefix)
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(prefix);
            clients = new ConcurrentDictionary<Guid, WebSocket>();
        }

        //Start up asynchonosly
        public async Task Startup()
        {
            //Starting the http listener up
            httpListener.Start();
            Console.WriteLine("Chat websocket is running!");

            //handling incomming client messages
            while (true)
            {
                var context = await httpListener.GetContextAsync();
                //Message is a web socket request we can handle 
                if (context.Request.IsWebSocketRequest)
                {



                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

    }
}