using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PeopleHelpPeople.Model
{
    public class WebSocketServer2
    {
        // The variables used through out our chat and websocket
        private string serverUrl;
        private ConcurrentDictionary<Guid, WebSocket> clients = new ConcurrentDictionary<Guid, WebSocket>();
        private HttpListener httpListener;

        //The websocket setup
        public WebSocketServer2(string serverUrl)
        {
            serverUrl = serverUrl;
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(serverUrl);
        }

        public async Task StartUp()
        {
            httpListener.Start();
            Console.WriteLine("WebSocket is running on: " + serverUrl + "\n");

            //infinite loop to keep service running
            while (true)
            {
                //Handling requests
                var context = await httpListener.GetContextAsync();
                if(context.Request.IsWebSocketRequest)
                {
                    //Setup the client
                    var clientID = Guid.NewGuid();
                    //Setup their connection
                    var webContext = await context.AcceptWebSocketAsync(null);
                    var webSocket = webContext.WebSocket;
                    //Save them
                    clients.TryAdd(clientID, webSocket);

                    //Handle the client request
                    _ = Task.Run(() => handleClient(clientID, webSocket));

                }
                //Error
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleClient(Guid clientID, WebSocket webSocket)
        {
            try
            {
                var buffer = new byte[1024];
                while(webSocket.State == WebSocketState.Open)
                {
                    var request = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);


                    if(request.MessageType == WebSocketMessageType.Text)
                    {
                        var messageString = Encoding.UTF8.GetString(buffer, 0, request.Count);

                        //Reading JSON
                        Message message = JsonSerializer.Deserialize<Message>(messageString);

                        //Getting the different parts of the message
                        string reciever = message.reciever;
                        string messageContent = message.content;

                        //Send the message out to another client
                        await SendMessage(reciever, messageContent);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error recieving message: " + ex + " from: " + clientID + "\n");
            }
            finally
            {
                clients.TryRemove(clientID, out _);
                webSocket.Dispose();
            }
        }

        //Send the mnessage to other client
        private async Task SendMessage(Guid reciever, string message)
        {
            if(clients.TryGetValue(reciever, out WebSocket recieverSocket))
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await recieverSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            }
            else
            {
                Console.WriteLine("Not able to send message!");
            }
        }


        //Define Message object
        public class Message
        {
            public string reciever { get; set; }
            public string content { get; set; }
        }
    }
}