using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


// WebSocketHandler.cs
public class WebSocketHandler
{
    private WebSocketManager _webSocketManager;

    public WebSocketHandler(WebSocketManager webSocketManager)
    {
        _webSocketManager = webSocketManager;
    }

    public async Task HandleAsync(HttpContext context, Func<Task> next)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await next.Invoke();
            return;
        }

        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        string socketId = _webSocketManager.AddSocket(webSocket);

        await ReceiveMessage(webSocket, async (result, buffer) =>
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await RouteMessage(socketId, message);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocketManager.RemoveSocketAsync(socketId);
            }
        });
    }

    private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        byte[] buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            handleMessage(result, buffer);
        }
    }

    private async Task RouteMessage(string senderId, string message)
    {
        foreach (var socket in _webSocketManager.GetAllSockets())
        {
            if (socket.Key != senderId) // Forward message to all other clients
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    byte[] msgBuffer = Encoding.UTF8.GetBytes(message);
                    await socket.Value.SendAsync(new ArraySegment<byte>(msgBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}