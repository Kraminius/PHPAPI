using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketMessageHandler
{
    private readonly WebSocketConnectionManager manager;

    public WebSocketMessageHandler(WebSocketConnectionManager manager)
    {
        manager = manager;
    }

    // Take care of things happening over a socket
    public async Task Handle(Guid userId, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        string socketId = manager.AddSocket(webSocket);

        try
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var messageParts = message.Split('|');
                if (messageParts.Length == 2)
                {
                    var targetId = messageParts[0];
                    var content = messageParts[1];
                    var targetSocket = manager.GetSocketById(targetId);
                    if (targetSocket != null && targetSocket.State == WebSocketState.Open)
                    {
                        await targetSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"From {socketId}: {content}")),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            manager.RemoveSocket(socketId);
        }
        catch (Exception ex)
        {
            manager.RemoveSocket(socketId);
            Console.WriteLine($"WebSocket error: {ex.Message}");
        }
    }
}
