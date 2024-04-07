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

                    WebSocketContext socketContext = null;

                    try
                    {
                        socketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        //For the future we would like to get id from database!!!
                        var clientID = Guid.NewGuid();
                        var webSocket = socketContext.WebSocket;
                        clients.TryAdd(clientID, webSocket);
                        Console.WriteLine("Connected client: " + clientID + "\n");
                        await ReceiveMessagesAsync(webSocket, clientID); //Fix this

                    }
                    catch(Exception exception)
                    {
                        Console.WriteLine("Chat client Connection error" + exception + "\n");
                    }
                    finally
                    {
                        if(socketContext != null)
                        {
                            clients.TryRemove(Guid.NewGuid(), out _);
                        }
                    }

                }
                //Message is not a web socket so we cant work with it, we send a 400 response
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        //Recieve message
        private async Task RecieveMessage(WebSocket webSocket, Guid clientID)
        {
            var buffer = new byte[1024];
            while(webSocket.State == WebSocketState.Open)
            {

                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if(result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    }
                }

            }
        }


    }
}