using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

public class WebSocketConnectionManager
{
    private ConcurrentDictionary<string, WebSocket> sockets = new ConcurrentDictionary<string, WebSocket>();

    public string AddSocket(WebSocket socket)
    {
        string connectionId = Guid.NewGuid().ToString();
        sockets.TryAdd(connectionId, socket);
        return connectionId;
    }

    public ConcurrentDictionary<string, WebSocket> GetAllSockets()
    {
        return sockets;
    }

    public WebSocket GetSocketById(string id)
    {
        sockets.TryGetValue(id, out var socket);
        return socket;
    }

    public void RemoveSocket(string id)
    {
        sockets.TryRemove(id, out var socket);
        if (socket != null)
        {
            socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: "Closed by the ConnectionManager",
                cancellationToken: CancellationToken.None);
        }
    }
}
