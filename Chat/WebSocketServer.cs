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
                    var clientID = Guid.NewGuid();

                    try
                    {
                        socketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        // For the future we would like to get id from database!!!
                        var webSocket = socketContext.WebSocket;
                        clients.TryAdd(clientID, webSocket);
                        Console.WriteLine("Connected client: " + clientID + "\n");
                        await ReceiveMessageAsync(webSocket, clientID); // Fix this

                    }
                    catch(Exception exception)
                    {
                        Console.WriteLine("Chat client Connection error" + exception + "\n");
                    }
                    finally
                    {
                        if(socketContext != null)
                        {
                            clients.TryRemove(clientID, out _);
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
                    //Recieved a message
                    if(result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine("Recieved message: " + message + "\n");
                        await ForwardMessage(clientID, message);
                    }
                    //Recieved close request
                    else if(result.MessageType == WebSocketMessageType.Close)
                    {

                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);

                    }
                }
                catch(Exception exception) 
                {
                    Console.WriteLine("Error recieving message: " + exception + " from: " + clientID + "\n");
                    break;
                }

            }
        }

        private async Task ForwardMessage(Guid senderID, string message)
        {
            var splitMessage = message.Split(':');

            //Message is not valid
            if(splitMessage.Length != 3) 
            {
                Console.WriteLine("Invalid message: " + message + "\n");
                return;
            }

            //Reciever not valid
            Guid reciverID;
            if (!Guid.TryParse(splitMessage[1], out reciverID))
            {
                Console.WriteLine("Invalid reciever: " + splitMessage[1]);
                return;
            }

            //Send message
            var chatMessage = senderID + ":" + splitMessage[2];

            if(clients.TryGetValue(reciverID, out var recieverSocket) && (recieverSocket.State == WebSocketState.Open))
            {
                var buffer = Encoding.UTF8.GetBytes(chatMessage);
                await recieverSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("Send message to: " + senderID + ". Message is: " + splitMessage[2] + "\n");
            }
            else
            {
                Console.WriteLine("Reciever: " + reciverID + " not found");
            }


        }


    }
}